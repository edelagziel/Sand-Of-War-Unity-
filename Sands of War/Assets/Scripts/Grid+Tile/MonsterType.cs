using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterType
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Damage { get; set; }
    public GameObject MonsterObj { get; set; }
    public int movment { get; set; }
   public TextMeshPro MonsterInfoTxt { get; set; }


    // We made the MonsterType constructor for easy modification.
    public MonsterType(string name, int health, int damage, GameObject monsterObj, int Movement, TextMeshPro MonsterInfoTxt)
    {
        Name = name;
        Health = health;
        Damage = damage;
        MonsterObj = monsterObj;
        movment = Movement;
        this.MonsterInfoTxt = MonsterInfoTxt;
    }
}
