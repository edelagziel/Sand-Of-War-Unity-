using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="new Card",menuName ="Card")]//Used to create a custom Asset that can be created through the Unity interface.

public class Cards :ScriptableObject
{
    public string CardName;
    public string Description;
    public string Movment;

    public Sprite artwork;

    public int ManaCost;
    public int Attack;
    public int health;
}
