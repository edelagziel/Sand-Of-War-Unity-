using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardManger : MonoBehaviour
{
    #region Variables
    public List <Cards> Deck=new List<Cards>();
    public List<Cards> GraveGard = new List<Cards>();

    public cardSlots[] CardSlot;
    public TextMeshProUGUI CountDecktxt;
    public TextMeshProUGUI CountDiscardtxt;
    int CountDiscard = 0;
    public UnityEngine.UI.Button DrawButton;
    [System.Serializable]
    public struct cardSlots
    {
        public bool AviableCardSlot;
        public Transform PositionCardSlots;
        public GameObject Card;
        public CardDisplay CardRefrance;
        public CardBehavior CardBehaviorRefrance;
        public cardSlots(bool AviableCardSlot, Transform PositionCardSlots, GameObject Card, CardDisplay CardRefrance,CardBehavior CardBehaviorRefrance)
        {
           this.AviableCardSlot= AviableCardSlot;
           this.PositionCardSlots= PositionCardSlots;
           this.Card = Card;
           this.CardRefrance= CardRefrance;
           this.CardBehaviorRefrance = CardBehaviorRefrance;
        }
    };
    #endregion
    #region Logic
    public void DrawCard()
    {
        if (Deck.Count >= 1&&GameManager._MatcIsOver==false)
        {
            DrawButton.interactable = false;
            StartCoroutine(DrawCardTime());
        }
    }
    public void CardPlayAction(int SlotIndex)
    {
        CardSlot[SlotIndex].AviableCardSlot = true;
        CountDiscard++;
        CountDiscardtxt.text= CountDiscard.ToString();
        CardSlot[SlotIndex].Card.gameObject.SetActive(false);
    }
    public void ResetDeck()
    {
        foreach(Cards ReturnCard in GraveGard)
        {
            Deck.Add(ReturnCard);
        }
        GraveGard.Clear();
        CountDiscard = 0;
        CountDecktxt.text = Deck.Count.ToString();
        CountDiscardtxt.text = GraveGard.Count.ToString();
        for (int i = 0; i < CardSlot.Length; i++)
        {
            CardSlot[i].AviableCardSlot= true;
            CardSlot[i].Card.gameObject.SetActive(false);
        }
    }
   

    private IEnumerator DrawCardTime()
    {

        if (Deck.Count == 0)
        {
            Debug.LogError("The deck is empty! Cannot draw a card.");
            yield break; 
        }

        int RadomCardIndex = Random.Range(0, Deck.Count);
        Cards RandomCard = Deck[RadomCardIndex];
        for (int i = 0; i < CardSlot.Length; i++)
        {
            if (CardSlot[i].AviableCardSlot == true)
            {
                CardSlot[i].CardRefrance.card = RandomCard;
                CardSlot[i].Card.gameObject.SetActive(true);
                CardSlot[i].AviableCardSlot = false;
               // yield return new WaitForSeconds(0.1f); // We need awaiting time because we have some actions that use the index.
                CardSlot[i].CardBehaviorRefrance.HandIndex = i;
                yield return new WaitForSeconds(0.1f);// We need awaiting time because we have some actions that use the index.
                GraveGard.Add(RandomCard);
               // yield return new WaitForSeconds(0.2f);
                Deck.RemoveAt(RadomCardIndex);// Remove by index
                CountDecktxt.text = Deck.Count.ToString();
                DrawButton.interactable = true;
                break;
            }
        }
        DrawButton.interactable = true;
    }
    #endregion
    #region MonoBehaviour
    private void OnEnable()
    {
        CardBehavior.CardExit += CardPlayAction;
        GameManager.ReasetGame += ResetDeck;
    }
    private void OnDisable()
    {
        CardBehavior.CardExit -= CardPlayAction;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void Awake()
    {
        CountDecktxt.text = Deck.Count.ToString();
    }
    #endregion
}
