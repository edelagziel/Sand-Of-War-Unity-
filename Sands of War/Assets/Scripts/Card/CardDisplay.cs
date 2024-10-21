using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour {

	public Cards card;

	public Text nameText;
	public Text descriptionText;

	public Text Movment; 
    public Image artworkImage;

	public Text manaText;
	public Text attackText;
	public Text healthText;

    // Use this for initialization
    void Start()
    {
    }
    public void ParmterToCard()
    {
        nameText.text = card.CardName;
        descriptionText.text = card.Description;

        artworkImage.sprite = card.artwork;

        manaText.text = card.ManaCost.ToString();
        attackText.text = card.Attack.ToString();
        healthText.text = card.health.ToString();
        Movment.text = card.Movment.ToString();
    }
    private void Awake()
    {
        ParmterToCard();
    }
    private void Update()
    {
        ParmterToCard();
    }

}
