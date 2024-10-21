using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Screen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private Canvas _Manu;
     public Canvas _Play;
    /// <summary>
    /// When the button is pressed, this function will be called. If the string equals a screen enum, we will change the screen with an event.
    /// </summary>
    /// <param name="Screen_Name"></param>
    public void ButtonEvent_Screen(string Screen_Name)
    {
        Game_Manfer_Screen.Screen new_Screen;
        bool Success = Enum.TryParse<Game_Manfer_Screen.Screen>(Screen_Name, out new_Screen);// Convert string to enum. new_Screen ->will have the Enum value.
        if (Success)
            Game_Manfer_Screen.Buttin_Screen_Press?.Invoke(new_Screen);
        else
            Debug.Log("Name_Error");
    }
    /// <summary>
    /// This function will get called if the Back button is pressed.
    /// </summary>
    public void Back_Button()
    {
        Game_Manfer_Screen.Buttin_Screen_Press_Back?.Invoke();
    }
    public void Play_Button()
    {
        Debug.Log("Play_Button");
        _Manu.gameObject.SetActive(false);
        _Play.gameObject.SetActive(true);
    }
    public void SingleToMainManu_Button()
    {
        _Manu.gameObject.SetActive(true);
        _Play.gameObject.SetActive(false);
    }

    void Start()
    {
        _Play.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
