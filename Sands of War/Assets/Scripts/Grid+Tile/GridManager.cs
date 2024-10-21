using com.shephertz.app42.gaming.multiplayer.client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
/// <summary>
/// Mange The gried Map 
/// </summary>
public class GridManager : MonoBehaviour
{
    
    
    #region Variables
    [SerializeField] private GameObject Tile_obj;
    public int White, heigat;
    [SerializeField] private float Tile_Space;
    [SerializeField] private Transform Camra_Transform;
    private BoardTile Tile_Script;
    public TileData[,] TileStates;
  
    public GameManager gameManager;
    List<PosibleTile> currentTileList = new List<PosibleTile>();          // To store the currently highlighted blue optional tiles
    List<PosibleTile> currentTileAttack = new List<PosibleTile>();       // To store the currently highlighted orange optional tiles
    private bool isShowingTiles = false;                                // If there is a MarkTile on the battlefield
    public PlayerMonster MarkMonster;
    [SerializeField]private float MovmentSpeed;
    [SerializeField] private int BotSpwaneMinX=7, BotSpwaneMinY=1, BotSpwaneMaxX=8, BotSpwaneMaxY=3;
    List<PlayerMonster> ButObjList = new List<PlayerMonster>();          
    public List <Vector2> HighlightSpawn;
    public static Action<int> CardTorch;

    public struct TileData
    {
        public SlotState SlotState;
        public MonsterType UnitType;
        public GameObject Monster;
        public TileData(SlotState slotState, MonsterType unitType, GameObject Monster)
        {
            SlotState = slotState;
            UnitType = unitType;
            this.Monster = Monster;
        }
    };
    public struct PosibleTile
    {
        public int IndX, IndY;
        public BoardTile TielReferance;                                 //probley chage to class type 
        public PosibleTile(int IndX, int IndY, BoardTile TielReferance)
        {
            this.IndX = IndX;
            this.IndY = IndY;
            this.TielReferance = TielReferance;
        }
    };
    public struct PlayerMonster
    {
        public int IndX, IndY;
        public MonsterType MonsterType;
        public GameObject MonsterObj;
        public PlayerMonster(int IndX, int IndY, MonsterType MonsterType, GameObject MonsterObj)
        {
            this.IndX = IndX;
            this.IndY = IndY;
            this.MonsterType = MonsterType;
            this.MonsterObj = MonsterObj;
        }
    };

    public struct CanAttack
    {
        public bool YouCan;
        public BoardTile TielReferance;                                 
        public CanAttack(bool YouCan,BoardTile TielReferance)
        {
            this.YouCan = YouCan;
            this.TielReferance = TielReferance;
        }
    };

    public enum SlotState
    {
        Empty,
        Player1,
        Player2
    };
    public enum HighlightsColor
    {
        Defult,
        Green,
        Red,
        Blue,
        Orange,
    };
    //singlton if need
    /* private static GridManager instance;
     public static GridManager Instance
     {
         get
         {
             if (instance == null)
                 instance = GameObject.Find("Grid Manager").GetComponent<GridManager>();
             return instance;
         }
     }*/
    #endregion
    #region Logic
    private void GenerateGrid()
    {
        if (Tile_obj == null) return;
        for (int x = 0; x < White; x++)
        {
            for (int y = 0; y < heigat; y++)
            {
                GameObject Spawned_Tile = Instantiate(Tile_obj, new Vector2(x, y) * Tile_Space, Quaternion.identity);
                Spawned_Tile.name = "BoardTile" + x.ToString() + y.ToString();
                Tile_Script = Spawned_Tile.GetComponent<BoardTile>();
                if (Tile_Script != null)
                {
                    Tile_Script.Id_x = x;
                    Tile_Script.Id_y = y;
                }
            }
        }
        if (Camra_Transform == null) return;
        Camra_Transform.position = new Vector3(((White - 1) * Tile_Space) / 2, heigat / 2 - 0.5f, Camra_Transform.position.z);
    }

    private void InitializeTileStates()
    {
        for (int x = 0; x < White; x++)
        {
            for (int y = 0; y < heigat; y++)
            {
                InitializeOneTile(x, y);
            }
        }
    }
     private void InitializeOneTile(int x,int y)
     {
        TileStates[x, y].SlotState = SlotState.Empty;
        TileStates[x, y].UnitType = DataPerbs.Instance.Non;
        if (TileStates[x, y].Monster != null)
        {
            Destroy(TileStates[x, y].Monster);
            TileStates[x, y].Monster = null;
        }
     }
    private void CreateTileStates()
    {
        TileStates = new TileData[White, heigat];
    }
    /// <summary>
    /// Get reference to specific tile component BoardTile
    /// </summary>
    /// <param name="Coordinates"></param>
    /// <returns>BoardTile</returns>
    private BoardTile TileReference(BoardTile.TileCoordinates Coordinates)
    {
        int Temp_x = Coordinates.Idx, Temp_y = Coordinates.Idy;
        GameObject TempObj = gameManager.FindTile(Temp_x, Temp_y);
        if (TempObj == null) return null;
        BoardTile BoardTile = TempObj.GetComponent<BoardTile>();
        return BoardTile;
    }
    public BoardTile TileReferenceIdx(int Temp_x, int Temp_y)
    {
        GameObject TempObj = gameManager.FindTile(Temp_x, Temp_y);
        if (TempObj == null) return null;
        BoardTile BoardTile = TempObj.GetComponent<BoardTile>();
        return BoardTile;
    }
    /// <summary>
    /// Called from event: This function controls the highlights of the tile according to SlotState: green, red, or off.
    /// </summary>
    /// <param name="Coordinates"></param>
    private void TileHighlight(BoardTile.TileCoordinates Coordinates)
    {
        BoardTile BoardTile = TileReference(Coordinates);

        if (BoardTile.IsHighlights == true)
        {
            if (TileStates[Coordinates.Idx, Coordinates.Idy].SlotState == SlotState.Player1)
                BoardTile.Highlights[(int)HighlightsColor.Green].SetActive(true);
            else
            {
                Debug.Log("Red");
                if(BoardTile.Highlights[(int)HighlightsColor.Orange].activeSelf==false)
                BoardTile.Highlights[(int)HighlightsColor.Red].SetActive(true);
            }
        }
        else
        {
            OffHighlights(BoardTile);
        }
    }

    private void OffHighlights(BoardTile BoardTile)
    {
        for (int i = 0; i <= 2; i++)
            BoardTile.Highlights[i].SetActive(false);
    }

   

    // Checking if the spawn press is in the spawn position place.
    public bool IsSpawnPlace(int x,int y)
    {
       Vector2[] SpawnOpsions = { new Vector2(0, 1), new Vector2(0, 2), new Vector2(0, 3), new Vector2(1, 1), new Vector2(1, 3) };
       Vector2 CurrentPressPos=new Vector2(x,y);
        for (int i = 0; i < SpawnOpsions.Length; i++)
        {
            if (SpawnOpsions[i]== CurrentPressPos)
            {
                return true;
            }
        }
        return false;
    }
    
  
    /// <summary>
    /// Spawn Monster Object if Tile is Empty
    /// </summary>
    /// <param name="Coordinates"></param>
    public void Spawn_Obj(BoardTile.TileCoordinates Coordinates)
    {
        if(gameManager.CardSelected==false)return;
        if(TurnManger.Instance.CanSpawnMonster()==false)
        {
            HighlightSOffTiles();
            return;
        }
        if(IsSpawnPlace(Coordinates.Idx,Coordinates.Idy)==false)
        {
            HighlightSOffTiles();
            return;
        }
        // MonsterType MonsterObj= gameManager.RandomMonster();
            MonsterType MonsterObj = gameManager.CardMonser;
        if (MonsterObj==null)return;
        if (TileStates[Coordinates.Idx, Coordinates.Idy].SlotState == SlotState.Empty)
        {
            //ClearSpwnToSmaenCard();
            GameObject CopyObject = Instantiate(MonsterObj.MonsterObj, Coordinates.TilePos, Quaternion.Euler(0, 90, 0));
            gameManager.MonsterText(CopyObject, MonsterObj);
            TileStates[Coordinates.Idx, Coordinates.Idy] = new(SlotState.Player1, MonsterObj, CopyObject);
            BoardTile BoardTile = TileReference(Coordinates);
            BoardTile.TileFull = true;
            BoardTile.MouseEnter?.Invoke(Coordinates);
            TurnManger.Instance.MarkMonsterAsSpawend();
            CardTorch?.Invoke(gameManager.CurrentSelectedHandIndex);
            HighlightSOffTiles();
            if(TurnManger.Instance.Multiplayer==true)
            gameManager.MultiplayerSendMove("Spawn_Obj", Coordinates.Idx, Coordinates.Idy, MonsterObj, 0,0);
        }

    }
    //mark the tile player can walk and also let him to walk


    private IEnumerator MoveandAttack(PosibleTile PosibleTile)
    {
        PlayerMonster Monster = new(PosibleTile.IndX, PosibleTile.IndY,TileStates[PosibleTile.IndX, PosibleTile.IndY].UnitType, TileStates[PosibleTile.IndX, PosibleTile.IndY].Monster);
        ButObjList.Remove(Monster);
        PosibleTile NewPos = PosibleTile;
        NewPos.IndX = PosibleTile.IndX - 1;
        PlayerMovment(MarkMonster, NewPos);
        yield return new WaitForSeconds(1.5f);
        AttackPlayer(PosibleTile);
        if (TurnManger.Instance.Multiplayer == true)
        {
            gameManager.MultiplayerSendMove("MoveandAttack", PosibleTile.IndX, PosibleTile.IndY, Monster.MonsterType, 0, 0);
        }

    }
    private void AttackMonster(PlayerMonster MonsterData, PosibleTile PosibleTile)
    {
        if (TurnManger.Instance.CanMoveMonster(MonsterData.MonsterObj) == false)
        {
            if(Mathf.Abs(MonsterData.IndX - PosibleTile.IndX) == 1|| Mathf.Abs(MonsterData.IndY - PosibleTile.IndY) == 1)
            {
                if (Mathf.Abs(MonsterData.IndX - PosibleTile.IndX) == 1 && Mathf.Abs(MonsterData.IndY - PosibleTile.IndY) == 1)return;                
                Debug.Log("can attack ");
                StartCoroutine(MoveandAttack(PosibleTile));

            }
            else
            Debug.Log("cant attack this turn");
        }
       else
        {
            Debug.Log("can attack +culd move ");
            StartCoroutine(MoveandAttack(PosibleTile));          
        }
           
    }
    private void WalkOpsions(BoardTile.TileCoordinates Coordinates)
    {
        if (isShowingTiles == true)
        {
            foreach (PosibleTile PosibleTile in currentTileList)
            {
                if (Coordinates.Idx == PosibleTile.IndX && Coordinates.Idy == PosibleTile.IndY)
                {
                    PlayerMovment(MarkMonster, PosibleTile);
                    break;
                }
            }
            foreach (PosibleTile PosibleTile in currentTileAttack)
            {
                if (Coordinates.Idx == PosibleTile.IndX && Coordinates.Idy == PosibleTile.IndY)
                {
                    Debug.Log("attack");
                    AttackMonster(MarkMonster,PosibleTile);

                    break;
                    //PlayerMovment(MarkMonster, PosibleTile);
                    //break;
                }
            }
            ClearMark(currentTileList, HighlightsColor.Blue);
            if (currentTileAttack.Count > 0)
                ClearMark(currentTileAttack, HighlightsColor.Orange);
        }
        if (TileStates[Coordinates.Idx, Coordinates.Idy].SlotState == SlotState.Player1)
        {
            TileData CurrentTile = TileStates[Coordinates.Idx, Coordinates.Idy];
            int TempMovment = CurrentTile.UnitType.movment;
            List<PosibleTile> TileList = MarkTile(TempMovment, Coordinates);
            MarkMonster = new(Coordinates.Idx, Coordinates.Idy, CurrentTile.UnitType, CurrentTile.Monster);
        }
    }
    /// <summary>
    /// This function marks only the tiles we can step on.
    /// </summary>
    /// <param name="TempMovment"></param>
    /// <param name="Coordinates"></param>
    private List<PosibleTile> MarkTile(int TempMovment, BoardTile.TileCoordinates Coordinates)
    {
        Vector2[] directions = new Vector2[]       // Some Vector2 array contains the basic movement for all directions.
        {
        new Vector2(0, 1),    // Up
        new Vector2(0, -1),   // Down
        new Vector2(1, 0),    // Right
        new Vector2(-1, 0),   // Left
        new Vector2(1, 1),    // up-right
        new Vector2(1, -1),   // down-right
        new Vector2(-1, 1),   // up-left
        new Vector2(-1, -1)   // down-left
        };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2 direction = directions[i];

            for (int step = 1; step <= TempMovment; step++)
            {
                int newX = Coordinates.Idx + (int)(direction.x * step);                                          // Calculate the new X coordinate by adding the movement in the X direction (direction.x * step) to the current X (Coordinates.Idx)
                int newY = Coordinates.Idy + (int)(direction.y * step);                                         // Calculate the new Y coordinate by adding the movement in the Y direction (direction.y * step) to the current Y (Coordinates.Idy)
                if (Mathf.Abs(newX - Coordinates.Idx) + Mathf.Abs(newY - Coordinates.Idy) <= TempMovment)       // Mathf.Abs calculates the absolute value. We use it because we want to check without negative direction. The absolute value of x + t is no more than the max movement.
                {
                    GameObject TempObj = gameManager.FindTile(newX, newY);
                    if (TempObj != null)
                    {
                        BoardTile MyTile = TempObj.GetComponent<BoardTile>();
                        if (MyTile != null)                                           // If we find the BoardTile component and the tile is empty, we can move to it.
                        {
                            if (MyTile.TileFull == false)
                            {
                                MyTile.Highlights[(int)HighlightsColor.Blue].SetActive(true);
                                currentTileList.Add(new PosibleTile(newX, newY, MyTile));
                                isShowingTiles = true;
                            }
                            if (MyTile.TileFull == true && TileStates[newX, newY].SlotState == SlotState.Player2)
                            {
                                MyTile.Highlights[(int)HighlightsColor.Orange].SetActive(true);
                                currentTileAttack.Add(new PosibleTile(newX, newY, MyTile));
                            }

                        }
                    }
                }
            }
        }
        return currentTileList;
    }
    public void ClearMark(List<PosibleTile> TempList, HighlightsColor MyColor)
    {
        if (TempList == null) return;
        foreach (PosibleTile MarkTile in TempList)
        {
            MarkTile.TielReferance.Highlights[(int)MyColor].SetActive(false);
        }
        TempList.Clear();  //remove all obj from the list 
    }
    public void PlayerMovment(PlayerMonster PlayerMonster, PosibleTile Target)
    {       
       if (TurnManger.Instance.CanMoveMonster(PlayerMonster.MonsterObj) == false) return;
        MyMovment(PlayerMonster, Target);
    }
    private IEnumerator MoveMonster(PlayerMonster monster, Vector2 TargetPos)
    {
        while (monster.MonsterObj.transform.position.x!= TargetPos.x)
        {
            Vector2 monsterPos = monster.MonsterObj.transform.position;
            monster.MonsterObj.transform.position = Vector2.MoveTowards(monsterPos, new Vector2(TargetPos.x, monsterPos.y), MovmentSpeed * Time.deltaTime);
            yield return null; 
        }
        while (monster.MonsterObj.transform.position.y != TargetPos.y)
        {
            Vector2 monsterPos = monster.MonsterObj.transform.position;
            monster.MonsterObj.transform.position = Vector2.MoveTowards(monsterPos, new Vector2(monsterPos.x, TargetPos.y), MovmentSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private Vector2 BotDefultRandomSpawn()
    {
       int Xrange= UnityEngine. Random.Range(BotSpwaneMinX,BotSpwaneMaxX+1);// This function doesn't include the maximum itself in the range, so we add 1 for clarity and to include the maximum.
        int Yrange = UnityEngine.Random.Range(BotSpwaneMinY, BotSpwaneMaxY+1);// This function doesn't include the maximum itself in the range, so we add 1 for clarity and to include the maximum.
        return new Vector2(Xrange, Yrange);
    }
    private void BotSpawn_Obj()
    {
        Vector2 SpwanPos = BotDefultRandomSpawn();
        int TryToSwpned = 0;

        while (TileStates[(int)SpwanPos.x, (int)SpwanPos.y].SlotState != SlotState.Empty)
        {
            SpwanPos = BotDefultRandomSpawn();
            TryToSwpned++;
            if(TryToSwpned>20)// We don't want the game to be stuck in a while loop, so we allow it to continue without spawning this turn.
            {
                return;
            }
        }
            MonsterType MonsterObj = gameManager.RandomMonster();
            if (MonsterObj == null) return;
        PlaceMonsterOnBoard(SpwanPos, MonsterObj);


    }

    private void PlaceMonsterOnBoard(Vector2 SpwanPos, MonsterType monsterType)
    {
        Vector2 TilePos = gameManager.FindTile((int)SpwanPos.x, (int)SpwanPos.y).transform.position;
        GameObject CopyObject = Instantiate(monsterType.MonsterObj, TilePos, Quaternion.Euler(0, -90, 0));
        gameManager.MonsterText(CopyObject, monsterType);
        TileStates[(int)SpwanPos.x, (int)SpwanPos.y] = new(SlotState.Player2, monsterType, CopyObject);
        PlayerMonster NewBotMonster = new((int)SpwanPos.x, (int)SpwanPos.y, monsterType, CopyObject);
        if(TurnManger.Instance.Multiplayer==false)
        ButObjList.Add(NewBotMonster);
        BoardTile BoardTile = TileReferenceIdx((int)SpwanPos.x, (int)SpwanPos.y);
        BoardTile.TileFull = true;
        BoardTile.Coordinates = new((int)SpwanPos.x, (int)SpwanPos.y, TilePos);
        BoardTile.MouseEnter?.Invoke(BoardTile.Coordinates);
    }






    public void BotMovment()
    {
        if (ButObjList == null) return;

        for (int i = 0; i < ButObjList.Count; i++)
        {
            PlayerMonster playerMonster = ButObjList[i];

            int maxAttempts = 10;
            int attempts = 0;
            Vector2 target = new Vector2();
            int x, y;
            int maxValu = playerMonster.MonsterType.movment;
            while (attempts < maxAttempts)
            {
                int RandomAtempt = 0;
                do
                {
                     x = UnityEngine.Random.Range(-maxValu-1, maxValu+1);
                     y = UnityEngine.Random.Range(-maxValu-1, maxValu+1);
                    RandomAtempt++;
                    if(RandomAtempt>30)
                        break;
                }
                while (playerMonster.MonsterType.movment <= Mathf.Abs(x)+ Mathf.Abs(y));
                
                    target = new Vector2(playerMonster.IndX + x, playerMonster.IndY + y);
                if (target.x >= 0 && target.x < White && target.y >= 0 && target.y < heigat && TileStates[(int)target.x, (int)target.y].SlotState == SlotState.Empty)
                {
                    break; 
                }

                attempts++;
            }
            
            if (attempts == maxAttempts)
            {
                continue;
            }            
            BoardTile targetBoardTile = TileReferenceIdx((int)target.x, (int)target.y);
            BoardTile currentBoardTile = TileReferenceIdx(playerMonster.IndX, playerMonster.IndY);

            if (targetBoardTile == null || currentBoardTile == null)
            {
                continue; 
            }

            TileData currentData = TileStates[playerMonster.IndX, playerMonster.IndY];
            TileStates[(int)target.x, (int)target.y] = new(currentData.SlotState, currentData.UnitType, currentData.Monster);
            TileStates[playerMonster.IndX, playerMonster.IndY] = new(SlotState.Empty, DataPerbs.Instance.Non, null);

            targetBoardTile.TileFull = true;
            currentBoardTile.TileFull = false;

            GameObject targetObj = gameManager.FindTile((int)target.x, (int)target.y);
            if (targetObj != null)
            {
                StartCoroutine(MoveMonster(playerMonster, new Vector2(targetObj.transform.position.x, targetObj.transform.position.y)));
            }

            playerMonster.IndX = (int)target.x;
            playerMonster.IndY = (int)target.y;
            ButObjList[i] = playerMonster;

            BoardTile BoardTile = TileReferenceIdx((int)target.x, (int)target.y);
            Vector2 TilePos = gameManager.FindTile((int)target.x, (int)target.y).transform.position;
            BoardTile.Coordinates = new((int)target.x, (int)target.y, TilePos);
            BoardTile.MouseEnter?.Invoke(BoardTile.Coordinates);
        }
    }
    private CanAttack CanBotAttack(PlayerMonster playerMonster)
    {
        if (playerMonster.IndX - 1 >= 0 && playerMonster.IndX - 1 < White&& TileStates[playerMonster.IndX - 1, playerMonster.IndY].SlotState == SlotState.Player1)
        {
            Debug.Log("Bot attack");
            CanAttack youCan = new(true, TileReferenceIdx(playerMonster.IndX - 1, playerMonster.IndY));
            return youCan;
        }
        else if(playerMonster.IndY - 1 >= 0 && playerMonster.IndY - 1 < heigat && TileStates[playerMonster.IndX, playerMonster.IndY-1].SlotState == SlotState.Player1)
        {
            Debug.Log("Bot attack");
            CanAttack youCan = new(true, TileReferenceIdx(playerMonster.IndX, playerMonster.IndY-1));
            return youCan;
        }
        else
        {
            Debug.Log("Bot cant attack");
            CanAttack youCan = new(false, null);
            return youCan;
        }
    }
    private void BotAttack()
    {
        foreach (PlayerMonster playerMonster in ButObjList)
        {
            CanAttack AttackAllowed = CanBotAttack(playerMonster);
            if (AttackAllowed.YouCan==true )
            {
                Debug.Log("Bot can kill you ");
                AttackAllowed.TielReferance.TileFull = false;
                if (TileStates[AttackAllowed.TielReferance.Id_x, AttackAllowed.TielReferance.Id_y].UnitType == DataPerbs.Instance.player)
                    gameManager.GameIsOVER();
                InitializeOneTile(AttackAllowed.TielReferance.Id_x, AttackAllowed.TielReferance.Id_y);
            }
        }
    }

      private void PlayerTurnEnd()
      {
        ClearMark(currentTileList, HighlightsColor.Blue);
        if (currentTileAttack.Count > 0)
            ClearMark(currentTileAttack, HighlightsColor.Orange);
      }


    public void AiEnemyTurn()
    {
        Debug.Log("Enemy Turn");
        StartCoroutine(AiTurn());
    }

    private IEnumerator AiTurn()
    {
        gameManager.MakeButtonUninteractable(gameManager.EndTurn);
        yield return new WaitForSeconds(0.5f);
        BotSpawn_Obj();
        yield return new WaitForSeconds(1);
        BotAttack();
        BotMovment();
        BotAttack();
        yield return new WaitForSeconds(2);
        TurnManger.Instance.switchTurn();
        gameManager.MakeButtonInteractable(gameManager.EndTurn);
    }


    public void PlayerKingsPlacment(int x ,int y, SlotState IsPlayer,int _Quaternion)
    {
        GameObject PlayerOneTile = gameManager.FindTile(x, y);
        GameObject CopyObject = Instantiate(DataPerbs.Instance.player.MonsterObj, PlayerOneTile.transform.position, Quaternion.Euler(0, _Quaternion, 0));
        gameManager.MonsterText(CopyObject, DataPerbs.Instance.player);
        TileStates[x, y] = new(IsPlayer, DataPerbs.Instance.player, CopyObject);
        BoardTile BoardTile= TileReferenceIdx(x, y);
        BoardTile.TileFull = true;
        if(IsPlayer==SlotState.Player2&&TurnManger.Instance.Multiplayer==false)
        {
            ButObjList.Clear();
            ButObjList.Add(new(x,y,DataPerbs.Instance.player,CopyObject));
        }
    }
    public void ResetGame()
    {
        ClearMark(currentTileList, HighlightsColor.Blue);
        if (currentTileAttack.Count > 0)
            ClearMark(currentTileAttack, HighlightsColor.Orange); 
        InitializeTileStates();
        TurnManger.Instance.ResetTurn();
        PlayerKingsPlacment(1, 2, SlotState.Player1, 90);
        PlayerKingsPlacment(7, 2, SlotState.Player2, -90);
        gameManager.CurrentState = GameManager.GameState.NoWinner;
        gameManager.GameOverCanvas.gameObject.SetActive(false);
        GameManager._MatcIsOver=false;
    }

    public void HighlightSpawnTiles(DataPerbs.unitType MonsterUnite,int HendIndex)
    {
        if(TurnManger.Instance.CanSpawnMonster()==false)return;
        Debug.Log("CanCardSpwn Grid manger enter ");
        Vector2[] SpawnOpsions = { new Vector2(0, 1), new Vector2(0, 2), new Vector2(0, 3), new Vector2(1, 1), new Vector2(1, 3) };
        foreach (Vector2 pos in SpawnOpsions)
        {
            if (TileStates[(int)pos.x,(int)pos.y].SlotState == SlotState.Empty)
            {
              BoardTile boardTile=  TileReferenceIdx((int)pos.x,(int)pos.y);
              boardTile.Highlights[(int)HighlightsColor.Defult].SetActive(true);
              HighlightSpawn.Add(new Vector2((int)pos.x, (int)pos.y));
              gameManager.CardSelected=true;
              DataPerbs.Instance.MonsterInfo.TryGetValue(MonsterUnite, out gameManager.CardMonser);
                gameManager.CurrentSelectedHandIndex = HendIndex;
                ClearSpwnToSmaenCard();
            }
        }
    }
    public void HighlightSOffTiles()
    {
        foreach(Vector2 pos in HighlightSpawn)
        {
            BoardTile boardTile = TileReferenceIdx((int)pos.x, (int)pos.y);
            boardTile.Highlights[(int)HighlightsColor.Defult].SetActive(false);
        }
        HighlightSpawn.Clear();
        gameManager.CardSelected = false;        
    }


    public void ClearSpwnToSmaenCard()
    {
        ClearMark(currentTileList, HighlightsColor.Blue);
        if (currentTileAttack.Count > 0)
            ClearMark(currentTileAttack, HighlightsColor.Orange);
    }
    #endregion
    #region Multiplayer
    public void MultiplayerSpawn(int x, int y, MonsterType MonsterObj)
    {  
       PlaceMonsterOnBoard(new Vector2 (x,y), MonsterObj);
    }

    public void MyMovment(PlayerMonster PlayerMonster, PosibleTile Target)
    {
        Debug.Log("Enter MultiplayerMovment");
        BoardTile TargetBoardTile = TileReferenceIdx(Target.IndX, Target.IndY);
        BoardTile CurrentBoardTile = TileReferenceIdx(PlayerMonster.IndX, PlayerMonster.IndY);

        TileData CurrentData = TileStates[PlayerMonster.IndX, PlayerMonster.IndY];
        TileStates[Target.IndX, Target.IndY] = new(CurrentData.SlotState, CurrentData.UnitType, CurrentData.Monster);
        TileStates[PlayerMonster.IndX, PlayerMonster.IndY] = new(SlotState.Empty, DataPerbs.Instance.Non, null);
        TargetBoardTile.TileFull = true;
        CurrentBoardTile.TileFull = false;
        GameObject TargetObj = gameManager.FindTile(Target.IndX, Target.IndY);

        StartCoroutine(MoveMonster(PlayerMonster, new Vector2(TargetObj.transform.position.x, TargetObj.transform.position.y)));

        if (TurnManger.Instance.CurrentState == TurnManger.TurnState.PlayerTurn)
        {
            TurnManger.Instance.MarkMonsterAsMoved(PlayerMonster.MonsterObj);
        }
        if (TurnManger.Instance.Multiplayer == true)
        {
            gameManager.MultiplayerSendMove("PlayerMovment", PlayerMonster.IndX, PlayerMonster.IndY, PlayerMonster.MonsterType, Target.IndX, Target.IndY);
        }
    }

    public void AttackPlayer(PosibleTile PosibleTile)
    {        
        PosibleTile.TielReferance.TileFull = false;
        PosibleTile.TielReferance.Highlights[(int)HighlightsColor.Red].SetActive(false);
        if (TileStates[PosibleTile.IndX, PosibleTile.IndY].UnitType == DataPerbs.Instance.player)
            gameManager.GameIsOVER();
        InitializeOneTile(PosibleTile.IndX, PosibleTile.IndY);

    }


    #endregion
    #region MonoBehaviour

    private void OnEnable()
    {
        BoardTile.Mouse_Clike += Spawn_Obj;
        BoardTile.Mouse_Clike += WalkOpsions;
        BoardTile.MouseEnter += TileHighlight;
        BoardTile.MouseExit += TileHighlight;
        //BoardTile.MouseExit += ClearMark;
        GameManager.ReasetGame += ResetGame;
        TurnManger.BotTurn += AiEnemyTurn;
        TurnManger.NextTurn += PlayerTurnEnd;
        //GameManager.CardSpwn += CanCardSpwn;
        CardBehavior.CardClike += HighlightSpawnTiles;

    }
    private void OnDisable()
    {
        BoardTile.Mouse_Clike -= Spawn_Obj;
        BoardTile.Mouse_Clike -= WalkOpsions;
        BoardTile.MouseEnter -= TileHighlight;
        BoardTile.MouseExit -= TileHighlight;
        GameManager.ReasetGame -= ResetGame;
        TurnManger.BotTurn -= AiEnemyTurn;
        TurnManger.NextTurn -= PlayerTurnEnd;
        //GameManager.CardSpwn -= CanCardSpwn;
        CardBehavior.CardClike -= HighlightSpawnTiles;

    }

    private void Awake()
    {
        GenerateGrid();
        gameManager.White = White;
        gameManager.heigat = heigat;

    }
    void Start()
    {
        CreateTileStates();
        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
}
