using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//action: moves card(s) from one stack to another and checks if last card on column is face down and flips if so. 
public class CommandMoveCardsAndFlipCard : ICommand
{
    List<GameObject> CardsToMove; // can be just one card.
    GameObject FromCardStack;
    GameObject ToCardStack;
    bool wasCardFlipped; //if card(s) is moved and last card is facedown, it gets flipped and this bool will store that such a card was flipped. used for undo.

    public CommandMoveCardsAndFlipCard(List<GameObject> cardsToMove, GameObject fromCardStack, GameObject toCardStack)
    {
        //CardsToMove = cardsToMove;
        CardsToMove = new List<GameObject>(cardsToMove);
        FromCardStack = fromCardStack;
        ToCardStack = toCardStack;
        wasCardFlipped = false;
    }

    public void Execute()
    {
        //take cards from the fromCardStack (old owner) and put them into toCardStack (new owner) in same order.
        Debug.Log("Executing CommandMoveCards action");

        //Debug.Log("CardsToMove.Count="+ CardsToMove.Count);
        //Debug.Log("cardStackInCardStackNew.Cards.Count=" + cardStackInCardStackNew.Cards.Count);
        //Debug.Log("cardStackHitCardStackNew.Cards.Count=" + cardStackHitCardStackNew.Cards.Count);
        for (int i = 0; i < CardsToMove.Count; i++)
        {
            SwapCard(CardsToMove[i], FromCardStack, ToCardStack);
        }

        //needs to flip last card over if its face down.
        CheckAndFlipLastCardOfColumnCardStack(FromCardStack.GetComponent<CardStackNew>());
        //Debug.Log("CardsToMove.Count=" + CardsToMove.Count);
        //Debug.Log("cardStackInCardStackNew.Cards.Count=" + cardStackInCardStackNew.Cards.Count);
        //Debug.Log("cardStackHitCardStackNew.Cards.Count=" + cardStackHitCardStackNew.Cards.Count);
    }

    //Helper function to allow easier implementation of reversing the stack to and from. 
    //swaps card from a card stack to another.
    void SwapCard(GameObject card, GameObject fromCardStack, GameObject toCardStack)
    {
        //CardStackNew cardStackInCardStackNew = FromCardStack.GetComponent<CardStackNew>();
        //CardStackNew cardStackHitCardStackNew = ToCardStack.GetComponent<CardStackNew>();
        //cardStackInCardStackNew.Cards.Remove(card);
        //cardStackHitCardStackNew.Push(card);
        //Debug.Log("Inside Swap");
        //Debug.Log("FromCardStack.GetComponent<CardStackNew>().Cards.Count=" + FromCardStack.GetComponent<CardStackNew>().Cards.Count);
        //Debug.Log("ToCardStack.GetComponent<CardStackNew>().Cards.Count=" + ToCardStack.GetComponent<CardStackNew>().Cards.Count);
        fromCardStack.GetComponent<CardStackNew>().Cards.Remove(card);
        toCardStack.GetComponent<CardStackNew>().Push(card);
        //Debug.Log("FromCardStack.GetComponent<CardStackNew>().Cards.Count=" + FromCardStack.GetComponent<CardStackNew>().Cards.Count);
        //Debug.Log("ToCardStack.GetComponent<CardStackNew>().Cards.Count=" + ToCardStack.GetComponent<CardStackNew>().Cards.Count);
    }

    public void Undo()
    {
        //take cards from the toCardStack (new owner) and put them into fromCardStack (old owner) in same order.
        Debug.Log("Undoing CommandMoveCards action");
        //Debug.Log("CardsToMove.Count="+ CardsToMove.Count);
        //Debug.Log("ToCardStack.tag="+ ToCardStack.tag);
        //Debug.Log("FromCardStack.tag=" + FromCardStack.tag);

        //flip last card back down from old stack if it was flipped originally.
        if(wasCardFlipped){
            FromCardStack.GetComponent<CardStackNew>().Cards[FromCardStack.GetComponent<CardStackNew>().Cards.Count - 1].GetComponent<CardViewNew>().toggleFace(false);
        }
        for (int i = 0; i < CardsToMove.Count; i++)
        {
            SwapCard(CardsToMove[i], ToCardStack, FromCardStack);
        }
        //Debug.Log("CardsToMove.Count=" + CardsToMove.Count);

    }

    //if column card was moved check if last card is face down, and if so flip it face up.
    void CheckAndFlipLastCardOfColumnCardStack(CardStackNew cardStackInCardStackNew)
    {
        if (cardStackInCardStackNew.Cards.Count > 0 && cardStackInCardStackNew.Cards[cardStackInCardStackNew.Cards.Count - 1].GetComponent<CardModelNew>().faceUp == false)
        {
            cardStackInCardStackNew.Cards[cardStackInCardStackNew.Cards.Count - 1].GetComponent<CardViewNew>().toggleFace(true);
            //also store that the card was flipped so undo can flip it back down if needed.
            wasCardFlipped = true;
        }
    }
}
