using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CardStackNew))]
public class CardStackViewNew : MonoBehaviour
{

    CardStackNew _CardStackNew;
    int lastCount;

    public Vector3 StartPosition;
    public float Offset;
    public bool OffsetHorizontal = true; //true for horizontal, false for vertical offset.
    //public GameObject cardPrefab;
    public bool ReverseLayerOrder = false;
    public bool IsLastCardClickableOnly = false; // set all cards clickable in stack if false, or just the last card in stack to clickable if true.

    public bool IsAllCardsFaceUp = true;
    public bool IsLastCardOnlyFaceUp = false; // only valid if isAllCardsFaceUp is false

    // background image

    //default stackview update function. custom ones can be implemented outside.
    //will take Cards and display them from startposition shifted by an increasing Offset for each card.
    public void UpdateStackView(){
        int i = 0;
        GameObject lastCard = null;
        CardViewNew cardViewNew;
        CardModelNew cardModelNew;
        SpriteRenderer spriteRenderer;
        Vector3 temp;
        float cardOffsetToApply;
        foreach (GameObject card in _CardStackNew.Cards)
        {

            cardViewNew = card.GetComponent<CardViewNew>();
            cardModelNew = card.GetComponent<CardModelNew>();
            if (card.GetComponent<BoxCollider>() != null)
            {
                if (IsLastCardClickableOnly)
                {
                    card.GetComponent<BoxCollider>().enabled = false;
                }
                else
                {
                    card.GetComponent<BoxCollider>().enabled = true;
                }
            }
            spriteRenderer = card.GetComponent<SpriteRenderer>();
            //Vector3 temp = c.transform.position;
            cardOffsetToApply = Offset * i;

            temp = StartPosition + OffsetPositionWithDirection(cardOffsetToApply);

            card.transform.position = temp;

            if (ReverseLayerOrder)
            {
                spriteRenderer.sortingOrder = _CardStackNew.Cards.Count - i; //right to left put down first to last.
            }
            else
            {
                spriteRenderer.sortingOrder = i; //left to right put down first to last.
            }
            i++;
            cardViewNew.UpdateSprite();
            //if(IsAllCardsFaceUp){
            //    cardViewNew.toggleFace(IsAllCardsFaceUp);
            //}

            lastCard = card;
        }
        //set cardstack collider size to encompass all cards.
        //TODO:see if this dynamicism is still needed?

        //last object processed now gets box colider enabled. Saves using if statement in loop above for now.
        if (lastCard)
        {
            if (lastCard.GetComponent<BoxCollider>() != null)
            {
                lastCard.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }


    public void Toggle(int index, bool isFaceUp)
    {
        _CardStackNew.Cards[index].GetComponent<CardViewNew>().toggleFace(isFaceUp);
    }

    //helper function to reduce some logic of very similar code when deciding if offsetting left/right or up/down
    public Vector3 OffsetPositionWithDirection(float co)
    {
        Vector3 offset;
        if (OffsetHorizontal)
        {
            offset = (new Vector3(co, 0f, 0f));
        }
        else
        {
            offset = (new Vector3(0f, co, 0f));
        }
        return offset;
    }

    void Awake()
    {
        _CardStackNew = GetComponent<CardStackNew>();
    }
}
