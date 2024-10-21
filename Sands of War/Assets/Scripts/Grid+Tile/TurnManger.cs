using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManger : MonoBehaviour
{
    #region Variables
    private static TurnManger instance;   // Class DataPerbs made singleton for easy accessing
    public static TurnManger Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("TurnManger").GetComponent<TurnManger>();
            return instance;
        }
    }
    public static Action NextTurn;
    public static Action BotTurn;
    public static Action MultiplayerEndTurn;

    public bool Multiplayer = false;

    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn
    };
    public TurnState CurrentState;
    public TextMeshProUGUI TurnText;
    private bool hasSpawnedThisTurn = false; // To track if the player has already spawned this turn
    private HashSet<GameObject> MovedMonsters=new HashSet<GameObject>(); // a list of monsters that have already moved this turn
      //HashSet->Automatically prevents duplicates, so there is no need to check if an object already exists before adding it
    #endregion
    #region Logic
    public void PrintTextTurn()
    {
        if (CurrentState== TurnState.PlayerTurn)
            TurnText.text=TurnState.PlayerTurn.ToString();
        else 
            TurnText.text=TurnState.EnemyTurn.ToString();
    }
    public void ButtonEndPress()
    {
        if (GameManager._MatcIsOver == true) return;
        if(Multiplayer==true)
        {
            WarpClient.GetInstance().sendMove("Button Press");
        }
        else
        {
            switchTurn();
        }
    }
    public void switchTurn()
    {
        hasSpawnedThisTurn = false;
        MovedMonsters.Clear();
        if (CurrentState == TurnState.PlayerTurn)
        {
            CurrentState = TurnState.EnemyTurn;
        }
        else
            CurrentState = TurnState.PlayerTurn;
        NextTurn?.Invoke(); 
        if(Multiplayer==false&& CurrentState == TurnState.EnemyTurn)
        {
            BotTurn?.Invoke();           
        }

        if(Multiplayer==true)
        {
            Debug.Log("Next Player Turn");
            MultiplayerEndTurn?.Invoke();
        }
        
    }
    public void ResetTurn()
    {
        hasSpawnedThisTurn = false;
        MovedMonsters.Clear();
        CurrentState = TurnState.PlayerTurn;
    }
    public bool CanSpawnMonster()
    {
        if(hasSpawnedThisTurn==false&&CurrentState==TurnState.PlayerTurn)
        return true;
        else return false;
    }
    public void MarkMonsterAsSpawend()
    {
        hasSpawnedThisTurn = true;
    }
    public bool CanMoveMonster(GameObject MonsterObj)
    {
 // We create a list of all monsters that can move, so every time we want to move a monster, we check if it is not in the list. If not, it can move and we will add it to the list.
        if (CurrentState == TurnState.PlayerTurn&& MovedMonsters.Contains(MonsterObj)==false)
        {
            return true;
        }
        else 
            return false;
    }
    public void MarkMonsterAsMoved(GameObject MonsterObj)
    {
        MovedMonsters.Add(MonsterObj);// We create a list of all monsters that can move, so every time we want to move a monster, we check if it is not in the list. If not, it can move and we will add it to the list.
    }
    #endregion
    #region MonoBehaviour
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnEnable()
    {
        NextTurn += PrintTextTurn;
        Listener.OnMoveCompleted += OnMoveCompleted;

    }
    private void OnDisable()
    {
        NextTurn -= PrintTextTurn;
        Listener.OnMoveCompleted -= OnMoveCompleted;

    }
    private void Awake()
    {
        PrintTextTurn();
    }
    #endregion
    #region MultiplayerEvent

    private void OnMoveCompleted(MoveEvent _Move)
    {
        Debug.Log("Turn Manger-> switchTurn"); 
        switchTurn();
    }

    #endregion
}
