using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Multiplayer_Manger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Slider Slider_Multiplayer;
    [SerializeField] private TextMeshProUGUI Multiplayer_num;
    [SerializeField] private int Max_room = 10;
    
    public void Chage_Connection_Pasword()
    {
        Multiplayer_num.text = "$"+((int)(Slider_Multiplayer.value * Max_room)).ToString();
    }
    private void Awake()
    {
        Chage_Connection_Pasword();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
