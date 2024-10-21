using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Audio_Manger: MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private AudioSource _audioSource;
    [SerializeField]private Slider _slider_Music;
    [SerializeField]private TextMeshProUGUI audio_num;
    [SerializeField]private int Volum_Size=100;
    [SerializeField]private Slider _slider_Sfx;
    [SerializeField]private TextMeshProUGUI Sfx_num;
   
    public void Chage_Audio()
    {
        _audioSource.volume = _slider_Music.value;
        audio_num.text = ((int)(_slider_Music.value* Volum_Size)).ToString();
        Sfx_num.text= ((int)(_slider_Sfx.value * Volum_Size)).ToString();
    }


    private void Awake()
    {
        Chage_Audio();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
