using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manfer_Screen : MonoBehaviour
{
    // Start is called before the first frame update
    public static Action<Screen> Buttin_Screen_Press;
    public static Action Buttin_Screen_Press_Back;

    [SerializeField] private Screen Current_Screen;
     private Dictionary<string, GameObject> Dictionary_MyScreen;
    public Stack <Screen> Stack_Screen=new Stack<Screen>();
    public enum Screen
    {
        Main_Manu,
        Opsions,
        Student_Name,
        Multiplayer,
        Single_player,
        Loading
    };
 
    private void Back_Screen()
    {
        Screen Current_use_Screen = Stack_Screen.Peek();//Peek-> To View  the element at the top of the stack without removing it.
        Current_Screen = Current_use_Screen;
        Dictionary_MyScreen["Screen_" + Current_Screen].SetActive(false);
        Stack_Screen.Pop();//To remove and return the element at the top of the stack.
        Current_Screen = Stack_Screen.Peek();
        Dictionary_MyScreen["Screen_" + Current_Screen].SetActive(true);
    }
    private void Change_Screen(Screen New_Screen)
    {
        Dictionary_MyScreen["Screen_"+ Current_Screen].SetActive(false);
        Current_Screen = New_Screen;
        Dictionary_MyScreen["Screen_" + Current_Screen].SetActive(true);
        Stack_Screen.Push(New_Screen);//This operation adds the new screen (New_Screen) to the stack.
    }
    private void Awake()
    {
        Current_Screen = Screen.Main_Manu;
        Stack_Screen.Push(Screen.Main_Manu);
        GameObject[] Screen_Obj = GameObject.FindGameObjectsWithTag("Screens");
        if (Screen_Obj != null)
        {
            Dictionary_MyScreen = new Dictionary<string, GameObject>();
            foreach (GameObject obj in Screen_Obj)
            {
                Dictionary_MyScreen.Add(obj.name, obj);
                obj.SetActive(false);
                if (obj.name== "Screen_Main_Manu")
                {
                    obj.SetActive(true);
                }
            }
        }
      

    }
    private void OnEnable()
    {
        Buttin_Screen_Press += Change_Screen;
        Buttin_Screen_Press_Back += Back_Screen;
    }
    private void OnDisable()
    {
        Buttin_Screen_Press -= Change_Screen;
        Buttin_Screen_Press_Back -= Back_Screen;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
