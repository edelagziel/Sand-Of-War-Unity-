using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using UnityEditor;
using com.shephertz.app42.gaming.multiplayer.client.events;
using TMPro;
using System.Globalization;
using UnityEngine.UI;
using Unity.VisualScripting;
using AssemblyCSharp;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.Playables;


public class MultiplayerManu : MonoBehaviour
{
    // Start is called before the first frame update
    #region variables
    private string apiKey = "21249cecc7ae0cbea1d1e73369205b510136d254b423a27adc88837dc585afa4";
    private string SecretKey = "81f292e1382d8a869df4143707129beb296f49e14ff3c882865cb7e5130a1f7b";
    private Listener MyListener;
    private Dictionary<string, GameObject> UnityObjects;
    public TextMeshProUGUI PlayCurRoom;
    public TextMeshProUGUI PlayCurUserId;
    private TextMeshProUGUI CurStatus;
    private TextMeshProUGUI CurRooomId;
    private TextMeshProUGUI CurUserId;

    private Button PlayButton;
    private Slider PasswordSlider;
    private List<string> roomIds;
    private int MaxUsers=2;
    private string roomId = string.Empty;
    private Dictionary<string, object> matchRoomData;
    private int roomIndex = 0;
    public GameObject CavasPlay;

    public TextMeshProUGUI RoomText;
    public TextMeshProUGUI UserIdText;
    #endregion
    #region MonoBehaviour
    private void OnEnable()
    {
        Listener.OnConnect += OnConnecting;
        Listener.OnRoomsInRange += OnRoomsInRange;
        Listener.OnCreateRoom += OnCreateRoom;
        Listener.OnJoinRoom += OnJoinRoom;// When I enter the room.
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        Listener.OnUserJoinRoom += OnUserJoinRoom;// When somebody else enters the room.
        Listener.OnGameStarted += OnGameStarted;
        //Listener.OnLeaveRoom += OnLeaveRoom;
        Listener.onUserLeftedRoom += OnUserLeftRoom;
        Listener.OnLeaveRoom += OnLeaveRoom;

        GameManager.EndMUltiplayerSesion += MultiplayerOFF;

    }
    private void OnDisable()
    {
        Listener.OnConnect -= OnConnecting;
        Listener.OnRoomsInRange -= OnRoomsInRange;
        Listener.OnCreateRoom -= OnCreateRoom;
        Listener.OnJoinRoom -= OnJoinRoom;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        Listener.OnUserJoinRoom += OnUserJoinRoom;// When somebody else enters the room.
        Listener.OnGameStarted -= OnGameStarted;

        Listener.OnLeaveRoom -= OnLeaveRoom;
        Listener.onUserLeftedRoom -= OnUserLeftRoom;

        GameManager.EndMUltiplayerSesion -= MultiplayerOFF;

    }
    private void Awake()
    {
        MyListener = new Listener();// We are creating a new Listener instance.       
        // WarpClient -> This is a class inside our DLL that gives us functions to communicate with the server.
        // WarpClient is a singleton class. 
        WarpClient.initialize(apiKey, SecretKey);// We are creating a new instance of WarpClient in order to start working with the server
        WarpClient.GetInstance().AddConnectionRequestListener(MyListener);
        WarpClient.GetInstance().AddChatRequestListener(MyListener);
        WarpClient.GetInstance().AddUpdateRequestListener(MyListener);
        WarpClient.GetInstance().AddLobbyRequestListener(MyListener);
        WarpClient.GetInstance().AddNotificationListener(MyListener);
        WarpClient.GetInstance().AddRoomRequestListener(MyListener);
        WarpClient.GetInstance().AddZoneRequestListener(MyListener);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(MyListener);
        // We are registering for all above events that are called from the server. 

        UnityObjects = new Dictionary<string, GameObject>();
        GameObject[] FoundObjects = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject obj in FoundObjects)
        {
            UnityObjects.Add(obj.name, obj);
        }

        CurStatus = UnityObjects["Current Status txt"].GetComponent<TextMeshProUGUI>();
        CurRooomId = UnityObjects["Room Id txt"].GetComponent<TextMeshProUGUI>();
        CurUserId = UnityObjects["User Id txt "].GetComponent<TextMeshProUGUI>();

        PlayButton = UnityObjects["Btn_Play"].GetComponent<Button>();
        PasswordSlider = UnityObjects["Slider_num "].GetComponent<Slider>();
        matchRoomData = new Dictionary<string, object>();//object -> means that the value can be any data type in C# (int, string, float, custom classes,
    }
    /// <summary>
    /// Function will be in charge of the System Name Connection.
    /// </summary>
    /// <returns>string Name</returns>
    private string CreateUserId()
    {
        GlobalVariables.UserId = System.Guid.NewGuid().ToString();//Create a unique name for every new joining player.
        return GlobalVariables.UserId;
    }
    void Start()
    {
        WarpClient.GetInstance().Connect(CreateUserId());//This is the stage where the user tries to connect to the appwrap server.
        UpdateStatus("Connecting...");
        CurUserId.text = "UserId: "+GlobalVariables.UserId;



    }
    #endregion
    #region Events
    private void OnConnecting(bool IsSuccess)
    {
        if (IsSuccess == true)
            UpdateStatus("Connected!");
        else
            UpdateStatus("couldnt Connect ...");

        PlayButton.interactable = IsSuccess;// We want the play button to be interactable only when we are connected.
    }
    private void OnRoomsInRange(bool _IsSuccess, MatchedRoomsEvent eventObj)
    {
        // _IsSuccess -> is a boolean parameter that indicates whether it succeeded in bringing me the data or failed in the process.
        if (_IsSuccess == true)
        {
            UpdateStatus("Parsing Rooms");
            roomIds = new List<string>();
            foreach (var roomData in eventObj.getRoomsData())// We are retrieving the data of all open rooms we have.
            {
                roomIds.Add(roomData.getId());
            }

        }
        else
        {
            UpdateStatus("Error Fetching Rooms in Range");
        }
        roomIndex = 0;
        DoRoomSearchLogic();
    }

    private void OnCreateRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess == true)
        {
            UpdateStatus("Room was Created (" + _RoomId + "),wating for an opponent");
            JoinRoomLogic(_RoomId, "Room was Created (" + _RoomId + "),wating for an opponent");          
        }

    }


    private void OnJoinRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess == true)
        {
            UpdateStatus("joined Room" + _RoomId);
            CurRooomId.text="Room id: "+ _RoomId;
        }
        else
        {
            UpdateStatus("Failed to joined Room" + _RoomId);
        }
    }

    private void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
    {
        if(eventObj!=null&& eventObj.getProperties()!=null)
        {
            Dictionary<string,object> Properties=eventObj.getProperties();  
            if(Properties.ContainsKey("password")&& Properties["password"].ToString() == matchRoomData["password"].ToString())
            // We are checking if the dictionary contains a password and also if the password is matching our password.
            {
                roomId=eventObj.getData().getId();// Getting the ID number of the room.
                JoinRoomLogic(roomId, "Ressived Room info joining room: " + roomId);
               int a= eventObj.getJoinedUsers().Length;
               Debug.Log("a= "+a);
            }
            else
            {
                roomIndex++;
                DoRoomSearchLogic();
            }
        }
    }
    private void OnUserJoinRoom(RoomData eventObj, string joinedUserId)// In order to receive this event, we must be subscribed, or we will not get the event.
    {
        if(eventObj.getRoomOwner()==GlobalVariables.UserId &&GlobalVariables.UserId != joinedUserId)
        // In order to start the game, we want to allow only the Owner to start, so we need to check if the RoomOwner ID equals our ID.
        // Also, we need to check if Subscribe was not called before Join. If UserId == joinedUserId, it isn’t good
        // because only when we are joined do we wait for one more player before starting the game.
        {
            UpdateStatus("Starting Game...");
            WarpClient.GetInstance().startGame();
        }
    }
    private void OnGameStarted(string _Sender, string _RoomId, string _CurTurn)
    {
        UpdateStatus("Game Started, Current Turn:" + _CurTurn);
        CavasPlay.SetActive(true);
        RoomText.gameObject.SetActive(true);
        UserIdText.gameObject.SetActive(true);
        PlayStatus("Rooom Id: "+_RoomId, PlayCurRoom);
        PlayStatus("User id: "+GlobalVariables.UserId, PlayCurUserId);
        UnityObjects["Canva_Game_Screens"].SetActive(false);
    }

    private void OnUserLeftRoom(RoomData eventObj, string _UserName)
    {
        Debug.Log("user left the room hidaa");
        
        WarpClient.GetInstance().stopGame();
        WarpClient.GetInstance().LeaveRoom(eventObj.getId());
       

    }
    private void OnLeaveRoom(bool _IsSuccess, string _RoomId)
    {
        Debug.Log("Event OnLeaveRoom");
        WarpClient.GetInstance().DeleteRoom(roomId);

    }
    private void MultiplayerOFF()
    {
        Debug.Log("Multiplayer Off");
        Debug.Log(roomId);
        UpdateStatus("Connected!");
        CurRooomId.text = "";
        WarpClient.GetInstance().LeaveRoom(roomId);
    }


    #endregion
    #region Logic

    private void UpdateStatus(string NewString)
    {
        CurStatus.text = NewString;
    }
    private void PlayStatus(string NewString,TextMeshProUGUI Text)
    {
        Text.text = NewString;
    }
    public void BtnPlay()
    {
        matchRoomData.Clear();  
        matchRoomData.Add("password", ((int)(PasswordSlider.value * 10)).ToString());
        WarpClient.GetInstance().GetRoomsInRange(1, 2);
        PlayButton.interactable = false;
        UpdateStatus("Searching for a room... ");
    }
    private void JoinRoomLogic(string newRoomId,string message)
    {
        roomId = newRoomId;
        UpdateStatus(message);
        WarpClient.GetInstance().JoinRoom(roomId);
        WarpClient.GetInstance().SubscribeRoom(roomId);
       // When we are talking to the server, it's not like I can dictate the action; it will not necessarily follow the order in which we send requests.
       // So, it can execute SubscribeRoom before JoinRoom, and this isn't good. We will get the OnUserJoinRoom event twice and start the game without joining,
        // so we do some checks in OnUserJoinRoom to handle this. 

        // JoinRoom -> You become a member of the room, so you are considered an active participant.
        // SubscribeRoom -> It doesn't make you a member of the room, so you are not considered an active participant,
        // but you receive all events happening in the room so you can watch. If you also join the room, you can react. if not, you can't.
    }
    private void DoRoomSearchLogic()
    {
        if (roomIndex < roomIds.Count)//we are checking if we have more rooms
        {
            WarpClient.GetInstance().GetLiveRoomInfo(roomIds[roomIndex]);// Sending another request to the server to get the info on the next room in the room IDs list.
        }
        else// If we don't have one, we will create a new room.
        {
            UpdateStatus("Creating Room ...");
            WarpClient.GetInstance().CreateTurnRoom("Eden Room", GlobalVariables.UserId, MaxUsers, matchRoomData, GlobalVariables.TurnTime);
        }
    }





    #endregion
}
