using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//action: moves card(s) from one stack to another.
public class CommandMoveCards : ICommand
{
    List<GameObject> CardsToMove; // can be just one card.
    GameObject FromCardStack;
    GameObject ToCardStack;

    public CommandMoveCards(List<GameObject> cardsToMove, GameObject fromCardStack, GameObject toCardStack)
    {
        //CardsToMove = cardsToMove;
        CardsToMove = new List<GameObject>(cardsToMove);
        FromCardStack = fromCardStack;
        ToCardStack = toCardStack;
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
        for (int i = 0; i < CardsToMove.Count; i++)
        {
            SwapCard(CardsToMove[i], ToCardStack, FromCardStack);
        }
        //Debug.Log("CardsToMove.Count=" + CardsToMove.Count);

    }
}
