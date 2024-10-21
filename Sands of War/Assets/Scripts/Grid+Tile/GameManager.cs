using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
/// <summary>
/// finds references to all GameObjects and calls the relevant events.
/// </summary>

public class GameManager : MonoBehaviour
{
    //public TurnManger TurnManger;
    #region Varibls
    public static Action CardSpwn;
    public static Action ReasetGame;
    public static Action GameOver;
    public Canvas GameOverCanvas;
    public TextMeshProUGUI PlayerWintxt;
    public bool CardSelected=false;
    public MonsterType CardMonser;
    public int CurrentSelectedHandIndex;
    public GameObject MultiplierScript;
    public int White, heigat;
    public static bool _MatcIsOver=false;
    private string CurTurn;
    private float StartTime;
    private bool MultiplayerIsMyTurn;
    public TextMeshProUGUI Timetxt;
    public GridManager GridManager;
    public TextMeshProUGUI RoomText;
    public TextMeshProUGUI UserIdText;
    public UnityEngine.UI.Button EndTurn;
    public UnityEngine.UI.Button ReatartButton;
    public UnityEngine.UI.Button MultiplayerPlayButton;
    public static Action EndMUltiplayerSesion;

    public bool EndGameRestart=false;

    Dictionary<string, GameObject> unityObjectDictionary=new ();
    public enum GameState
    {
        NoWinner,Winner    //need to use later on 
    };
       public GameState CurrentState=GameState.NoWinner;
    #endregion
    #region MonoBehaviour
    private void Awake()
    {
        MultiplierScript.SetActive(false);
        Timetxt.gameObject.SetActive(false);
        RoomText.gameObject.SetActive(false);
        UserIdText.gameObject.SetActive(false);
    }
    void Start()
    {
        GameObject[]Tiels_arr=GameObject.FindGameObjectsWithTag("Unity_Object");
        foreach (GameObject tiel in Tiels_arr)
        {
            unityObjectDictionary.Add(tiel.name, tiel);  
        }
    }


    void Update()
    {
        if (_MatcIsOver == false && Timetxt.gameObject.activeSelf == true)
        {
            int currentTime = GlobalVariables.TurnTime - (int)(Time.time - StartTime);
            if (currentTime > 0)
            {
                Timetxt.text = currentTime.ToString();
            }
            else
            {
                Timetxt.text = "0";
            }
        }

    }
    private void OnEnable()
    {
        CardSpwn += TurnOffGameOverScreen;
        Listener.OnGameStarted += OnGameStarted;
        Listener.OnMoveCompleted += OnMoveCompleted;
        Listener.OnSendChat += OnSendChat;
        Listener.OnGameStopped += OnGameStopped;
        TurnManger.MultiplayerEndTurn += MultiplayerNewTurn;
        Listener.onUserLeftedRoom += OnUserLeftRoom;


    }
    private void OnDisable()
    {
        CardSpwn -= TurnOffGameOverScreen;
        Listener.OnGameStarted -= OnGameStarted;
        Listener.OnMoveCompleted -= OnMoveCompleted;
        Listener.OnSendChat -= OnSendChat;
        Listener.OnGameStopped -= OnGameStopped;
        TurnManger.MultiplayerEndTurn -= MultiplayerNewTurn;
        Listener.onUserLeftedRoom -= OnUserLeftRoom;

    }
    #endregion
    #region Logic

    public GameObject FindTile(int x , int y )
    {
        foreach (GameObject tiel in unityObjectDictionary.Values)
        {
            if(tiel.name=="BoardTile"+x+y)
            {
                return tiel;
            }
        }
        return null;
    }

    public void MakeButtonUninteractable(UnityEngine.UI.Button button)
    {
        button.interactable = false;
    }
    public void MakeButtonInteractable(UnityEngine.UI.Button button)
    {
        button.interactable = true;
    }
    public MonsterType RandomMonster()
    {
        List<DataPerbs.unitType> monsterKeys = new List<DataPerbs.unitType>(DataPerbs.Instance.MonsterInfo.Keys);// Return all types of keys/monstersName from the dictionary and save them in a list in order to use index.
        int randomIndex =UnityEngine.Random.Range(2, monsterKeys.Count);// Random index
        DataPerbs.unitType randomKey = monsterKeys[randomIndex];// Find the MonsterName/key in the list according to a random index.
        return DataPerbs.Instance.MonsterInfo[randomKey];// Return the actual value the name points to.
    }
    public void  MonsterText(GameObject CopyObject, MonsterType MonsterObj)
    {
        
        GameObject textObj = new GameObject("MonsterText");// We are creating a new Empty Game Object
        TextMeshPro textComponent = textObj.AddComponent<TextMeshPro>();// We are adding a text component to our empty object and saving the reference to it in new variables.
        textObj.transform.SetParent(CopyObject.transform);// We are placing our textObj inside our new monster as a child object.

        textComponent.text = "HP: " + MonsterObj.Health + "/ ATK: " + MonsterObj.Damage;// Assign the text component the actual attack and health values according to the chosen monster.
        MonsterObj.MonsterInfoTxt = textComponent;// Changing the text of the copy to the new one.

        textObj.transform.localPosition = new Vector3(0, -0.3f, 0);// localPosition is responsible for the position of the text. We are placing the textObj above our monsters according to the vector position we assign to it.
        textObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); // localScale is responsible for the size of the textObj It affects all inside components all of them will change in proportion to the object change.
        textComponent.fontSize = 10f;// fontSize is responsible only for the text letter size.


        textComponent.alignment = TextAlignmentOptions.Center; // Sets the text to always be centered within the text box.
        textComponent.color = Color.white;// Ensures the color of the text is white may change later on.

        textComponent.sortingLayerID = SortingLayer.NameToID("UI");// We assign our text to the UI layer.
        textComponent.sortingOrder = 10;// In the UI layer, we assign it an index of priority over other objects in the same layer.
    }
    public void GameIsOVER()
    {
        Debug.Log("GameOver");
        _MatcIsOver=true;
        GameOverCanvas.gameObject.SetActive(true);
        CurrentState = GameState.Winner;
        if (TurnManger.Instance.CurrentState == TurnManger.TurnState.PlayerTurn)
        {
            Debug.Log("Player 1 win ");
            PlayerWintxt.text = "Player 1 win ";
        }
        else
        {
            Debug.Log("Player 2 win ");
            PlayerWintxt.text = "Player 2 win ";
        }
    }



  


    public void ResetGameButton()
    {
        ReasetGame?.Invoke();
    }
    public void TurnOffGameOverScreen()
    {
        GameOverCanvas.gameObject.SetActive(false);
    }
    private void CanCardSpwn()
   {
        Debug.Log("Game Manger");
        CardSpwn?.Invoke();
    }

    #endregion
    #region MultiplayerLogic
    public MonsterType TryGetMonsterType(string MonsterName) 
    {
        DataPerbs.unitType MonsterEnum;
        bool SussesEnum = Enum.TryParse<DataPerbs.unitType>(MonsterName, out MonsterEnum);
        if (SussesEnum == true)
        {
            MonsterType MonsterObj = DataPerbs.Instance.MonsterInfo[MonsterEnum];
            Debug.Log("in funk MultiplayerSpawn -> MonsterObj == " + MonsterObj.Name);
            return MonsterObj;
        }
        return null;
    }
    public GridManager.PosibleTile TryGetPosibleTile(int x,int y)
    {
        BoardTile BoardTile= GridManager.TileReferenceIdx(x, y);
        GridManager.PosibleTile posibleTile =new GridManager.PosibleTile(x,y, BoardTile);
        return posibleTile;
    }

    public Vector2 GridCanculaturTransform(int x, int y)
    {
        int symmetricX = (White-1)-x ;
        return new Vector2(symmetricX, y);
    }
    public void MultiplayerNewTurn()
    {
        if (TurnManger.Instance.Multiplayer == false) return;
        if (TurnManger.Instance.CurrentState == TurnManger.TurnState.PlayerTurn)
        {
            MakeButtonInteractable(EndTurn);
        }
        else
        {
            MakeButtonUninteractable(EndTurn);
        }
    }
    public void MultiplayerSendMove(string ActionType, int x, int y , MonsterType MonsterType, int DestinationX, int DestinationY)
    {
        Debug.Log("MultiplayerSendMove->" + "x" + x + "y" + y);
        Dictionary<string,object> toSend = new Dictionary<string,object>();
        {
            toSend.Add("ActionType", ActionType);
            toSend.Add("x", x);
            toSend.Add("y", y);
            toSend.Add("monsterName", MonsterType.Name);
            toSend.Add("DestinationX", DestinationX);
            toSend.Add("DestinationY", DestinationY);
        };
        string json =MiniJSON.Json.Serialize(toSend);
        Debug.Log(json);
        WarpClient.GetInstance().SendChat(json);

    }

    #endregion
    #region Multiplayer_Event

    private void OnSendChat(string _Sender, string _Message)
    {
        if(_Sender!=GlobalVariables.UserId)
        {
            Dictionary<string, object> moveData = (Dictionary<string, object>)MiniJSON.Json.Deserialize(_Message);
            if(moveData.Count>0&& moveData["ActionType"]!=null)
            {
                string ActionType = moveData["ActionType"].ToString();

                string idxSrt = moveData["x"].ToString();
                int idx = int.Parse(idxSrt);

                string idySrt = moveData["y"].ToString();
                int idy = int.Parse(idySrt);

                string monsterName = moveData["monsterName"].ToString();

                string DestidxSrt = moveData["DestinationX"].ToString();
                int Destidx = int.Parse(DestidxSrt);

                string DestidySrt = moveData["DestinationY"].ToString();
                int Destidy = int.Parse(DestidySrt);

                Debug.Log("parmters ::"+ActionType + idx+idy+ monsterName+ Destidx+ Destidy);
                Vector2 Newsymmetric = GridCanculaturTransform(idx, idy);
                MonsterType MonsterType = TryGetMonsterType(monsterName);

                if (ActionType== "Spawn_Obj")
                {
                    Debug.Log("Enemy Spwn");

                    GridManager.MultiplayerSpawn((int)Newsymmetric.x, (int)Newsymmetric.y, MonsterType);
                }
                if(ActionType=="PlayerMovment")
                {
                    Debug.Log("PlayerMovment");
                    Vector2 DestNewsymmetric = GridCanculaturTransform(Destidx, Destidy);
                    GridManager.PosibleTile Target= TryGetPosibleTile((int)DestNewsymmetric.x,(int)DestNewsymmetric.y);
                    Debug.Log("x= "+ Target.IndX +"y= "+ Target.IndY + "TielReferance.name= " + Target.TielReferance.name);
                    GridManager.PlayerMonster monsterPlayed=new GridManager.PlayerMonster((int)Newsymmetric.x, (int)Newsymmetric.y, MonsterType, GridManager.TileStates[(int)Newsymmetric.x, (int)Newsymmetric.y].Monster);
                    if(monsterPlayed.MonsterObj!=null)
                    {
                        Debug.Log(monsterPlayed.MonsterObj.name);
                        GridManager.MyMovment(monsterPlayed, Target);
                    }
                  
                }
                if(ActionType== "MoveandAttack")
                {
                    Debug.Log("MoveandAttack!!!!!!!!!");
                    GridManager.PosibleTile Target = TryGetPosibleTile((int)Newsymmetric.x, (int)Newsymmetric.y);
                    Debug.Log("x= " + Target.IndX + "y= " + Target.IndY + "TielReferance.name= " + Target.TielReferance.name);
                    GridManager.AttackPlayer(Target);
                }
                if(_MatcIsOver==true)
                {
                    WarpClient.GetInstance().stopGame();
                }

            }
        }
    }
    private void OnGameStarted(string _Sender, string _RoomId, string _CurTurn)
    {
        Debug.Log("OnGameStarted -> GameManger");
        _MatcIsOver=false;
        CurTurn = _CurTurn;
        StartCoroutine(MultiplayerStart(CurTurn));
        ReatartButton.interactable = false;
        if(EndGameRestart==true)
        {
            ReasetGame?.Invoke();
            EndGameRestart = false;
        }

    }
    private IEnumerator MultiplayerStart(string _CurTurn)
    {
        yield return new WaitForSeconds(1);
        ReasetGame?.Invoke();
        Timetxt.gameObject.SetActive(true);
        StartTime = Time.time;
        TurnManger.Instance.Multiplayer = true;
        if (GlobalVariables.UserId == _CurTurn)
        {
            MultiplayerIsMyTurn = true;
            TurnManger.Instance.CurrentState = TurnManger.TurnState.PlayerTurn;
            TurnManger.Instance.PrintTextTurn();
        }
        else
        {
            MultiplayerIsMyTurn = false;
            TurnManger.Instance.CurrentState = TurnManger.TurnState.EnemyTurn;
            TurnManger.Instance.PrintTextTurn();
        }
        MultiplayerNewTurn();
    }
    private void OnGameStopped(string _Sender, string _RoomId)
    {
        Debug.Log("OnGameStopped -=> Game Over!!!!! "+ _Sender+ _RoomId);
        ReatartButton.interactable = true;
        EndGameRestart=true;
        MultiplayerPlayButton.interactable=true;  
        _MatcIsOver=true;
        
    }
    private void OnUserLeftRoom(RoomData eventObj, string _UserName)
    {
        ReatartButton.interactable=false;
    }





    // Update is called once per frame
    /// <summary>
    /// Enable the Multiplier script object set on it. The Multiplier Script only works when we are on the Multiplayer screen
    /// (this function is called in the menu from the Multiplayer button).
    /// </summary>
    public void MultiplierOn()
    {
        MultiplierScript.SetActive(true);
    }
   
    private void OnMoveCompleted(MoveEvent _Move)
    {
        StartTime = Time.time;
    }
    public void StartAgineMultiplayerGame()
    {
        if (TurnManger.Instance.Multiplayer == true)
        {
            WarpClient.GetInstance().startGame();
        }
    }
    public void DisableMultiplayer()
    {
        if (TurnManger.Instance.Multiplayer == true)
        {
            Debug.Log("DisableMultiplayer");           
                EndMUltiplayerSesion?.Invoke();
            // WarpClient.GetInstance().Disconnect();
             TurnManger.Instance.Multiplayer = false;
              MultiplierScript.SetActive(false);
        }
    }

    #endregion

}
