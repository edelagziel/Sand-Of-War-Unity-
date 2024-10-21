using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public  class DataPerbs:MonoBehaviour
{
   public Dictionary<unitType, MonsterType> MonsterInfo=new();
    public enum unitType
    {
        Non,
        Player,
        Mummy_Mon,
        Skeleton1,
        Skeleton2,
        Skeleton3,
        FootmanHP,
        GolemPolyart,
        GruntHP,
        GruntPBR,
        HP_Golem,
        PBR_Golem,
        

    };
    public GameObject Player_Obj;
    public GameObject []MonsterObj;
    public MonsterType Non;
    public MonsterType player;
    public MonsterType Mummy_Mon;
    public MonsterType Skeleton1;
    public MonsterType Skeleton2;
    public MonsterType Skeleton3;
    public MonsterType FootmanHP;
    public MonsterType GolemPolyart;
    public MonsterType GruntHP;
    public MonsterType GruntPBR;
    public MonsterType HP_Golem;
    public MonsterType PBR_Golem;

    private static DataPerbs instance;   // Class DataPerbs made singleton for easy accessing
    public static DataPerbs Instance
  {
      get
      {
          if (instance == null)
              instance = GameObject.Find("Data Perbs").GetComponent<DataPerbs>();
          return instance;
      }
  }
    public void Awake()
    {
        Non = new MonsterType(unitType.Non.ToString(), 0, 0, MonsterObj[(int)unitType.Non], 0,null);
        MonsterInfo.Add(unitType.Non, Non);
        player = new MonsterType(unitType.Player.ToString(), 20, 1, MonsterObj[(int)unitType.Player], 1, null);
        MonsterInfo.Add(unitType.Player, player);
        Mummy_Mon = new MonsterType(unitType.Mummy_Mon.ToString(), 3, 2, MonsterObj[(int)unitType.Mummy_Mon], 2, null);
        MonsterInfo.Add(unitType.Mummy_Mon,Mummy_Mon);
        Skeleton1 = new MonsterType(unitType.Skeleton1.ToString(), 4, 2, MonsterObj[(int)unitType.Skeleton1], 3,null);
        MonsterInfo.Add(unitType.Skeleton1, Skeleton1);
        Skeleton2 = new MonsterType(unitType.Skeleton2.ToString(), 2, 4, MonsterObj[(int)unitType.Skeleton2], 3, null);
        MonsterInfo.Add (unitType.Skeleton2, Skeleton2);
        Skeleton3 = new MonsterType(unitType.Skeleton3.ToString(), 3, 4, MonsterObj[(int)unitType.Skeleton3], 3,null);
        MonsterInfo.Add(unitType.Skeleton3, Skeleton3);

        FootmanHP = new MonsterType(unitType.FootmanHP.ToString(), 1, 5, MonsterObj[(int)unitType.FootmanHP], 2, null);
        MonsterInfo.Add(unitType.FootmanHP, FootmanHP);

        GolemPolyart = new MonsterType(unitType.GolemPolyart.ToString(), 7, 3, MonsterObj[(int)unitType.GolemPolyart], 1, null);
        MonsterInfo.Add(unitType.GolemPolyart, GolemPolyart);

        GruntHP = new MonsterType(unitType.GruntHP.ToString(), 8, 2, MonsterObj[(int)unitType.GruntHP], 2, null);
        MonsterInfo.Add(unitType.GruntHP, GruntHP);

        GruntPBR = new MonsterType(unitType.GruntPBR.ToString(), 4, 5, MonsterObj[(int)unitType.GruntPBR], 2, null);
        MonsterInfo.Add(unitType.GruntPBR, GruntPBR);

        HP_Golem = new MonsterType(unitType.HP_Golem.ToString(), 8, 8, MonsterObj[(int)unitType.HP_Golem], 1, null);
        MonsterInfo.Add(unitType.HP_Golem, HP_Golem);

        PBR_Golem = new MonsterType(unitType.PBR_Golem.ToString(), 2, 1, MonsterObj[(int)unitType.PBR_Golem], 1, null);
        MonsterInfo.Add(unitType.PBR_Golem, PBR_Golem);

    }
}
