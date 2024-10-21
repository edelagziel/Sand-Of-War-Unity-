using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CardBehavior : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler, IPointerExitHandler // I use this interface event because my card has to be on the canvas, and in the canvas, we need special functions to detect the mouse on items.
{
    #region Variables
    private Transform OriginalPos; 
    private bool hasBeenPlayed;
    public int HandIndex;
    public static Action<int>CardExit;
    public static Action <DataPerbs.unitType,int> CardClike;

    private Vector3 TargetPos;
    public Vector3 CureentPos;
    public Vector3 CureentScale;
    public DataPerbs.unitType CardUnit;
    public string CardName;
    #endregion
    #region Logic
    public void OnPointerClick(PointerEventData eventData)// Special function works similar to onMouseClickDown. 
    {
        Debug.Log("CardClike");
        CardName = this.gameObject.GetComponent<CardDisplay>().card.ToString().Trim();//If your string includes unnecessary spaces at the beginning or end, Trim() will help you clean them up.
        CardName = CardName.Split(' ')[0]; //Takes only the first part of the string(before the space).
        Debug.Log("Card name: " + CardName); 
        bool SussesEnum = Enum.TryParse(CardName, out CardUnit);
        Debug.Log("TryParse success: " + SussesEnum);
        if (SussesEnum)
        {
            Debug.Log("Enum value: " + CardUnit);
            CardClike?.Invoke(CardUnit, HandIndex);

        }
        else
        {
            Debug.LogError("Failed to convert card name to enum");
        }      
    }
    /// <summary>
    /// Will happen if the mouse is over the card.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hasBeenPlayed == false)
        {
            Debug.Log("CardEnter");
            this.gameObject.transform.position += Vector3.up * 1.35f;
            this.gameObject.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        }       
    }
    /// <summary>
    /// Will happen if the mouse exits the card area.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hasBeenPlayed == false)
        {
            Debug.Log("CardExit");
            this.gameObject.transform.position -= Vector3.up * 1.35f;
            this.gameObject.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
        }
    }
    /// <summary>
    /// When the card can be summoned, an event from the Grid Manager will call this function, and we will remove the card from hand.
    /// </summary>
    /// <param name="HandCurrentIndex"></param>
    public void CardOverdExit(int HandCurrentIndex)
    {
        if (hasBeenPlayed == false)
        {
            if(HandCurrentIndex!= HandIndex)return;// This function is called for all cards, and we want to activate the function only on what we press, so the index gives us this information.
            StartCoroutine(SmoothExitCard());
            hasBeenPlayed = true;
        }
    }
    /// <summary>
    /// We made it an IEnumerator to make card movement smooth, and we can still play or move other monsters at the same time without waiting for this to finish.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SmoothExitCard()
    {
        float speed = 5f;
        while (Vector3.Distance(this.gameObject.transform.position, TargetPos) > 0.1f)
        {
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, TargetPos, speed * Time.deltaTime);
            yield return null; 
        }
        yield return new WaitForSeconds(0.3f);
        if (CardExit != null)
            CardExit?.Invoke(HandIndex);
        this.gameObject.transform.position = CureentPos;// Return the card to its original position.
        this.gameObject.transform.localScale = CureentScale;// Return the card to its original scale.
        hasBeenPlayed = false;
        // We are only changing the card parameter, but the card prefab/object needs to return to its original transform.
    }
    #endregion
    #region MonoBehaviour
    private void Awake()
    {
        CureentPos=this.gameObject.transform.position;
        CureentScale=this.gameObject.transform.localScale;
        TargetPos =GameObject.Find("DiscardImg").transform.position;// The position where the discard deck is located.
    }
    private void OnEnable()
    {
        GridManager.CardTorch += CardOverdExit;       
    }
    private void OnDisable()
    {
        GridManager.CardTorch -= CardOverdExit;
    }
#endregion


}
