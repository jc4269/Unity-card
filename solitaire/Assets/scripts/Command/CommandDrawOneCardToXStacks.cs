using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//action: draw one card from deck giving out to each stack in turn or no cards left.
public class CommandDrawOneCardToXStacks : ICommand
{
    //int CardsToDraw;
    GameObject FromCardStack;
    List<GameObject> ToCardStacks;

    public CommandDrawOneCardToXStacks(GameObject fromCardStack, List<GameObject> toCardStacks)
    {
        //CardsToDraw = cardsToDraw;
        FromCardStack = fromCardStack;
        ToCardStacks = toCardStacks;
    }

    public void Execute()
    {
        //take cards from the fromCardStack (old owner) and put them into toCardStack (new owner) in same order.
        Debug.Log("Executing CommandDrawCards action");
        //DrawCards(true, FromCardStack, ToCardStack);
        CardStackNew cardStackNew;
        CardStackViewNew cardStackViewNew;
        for (int i = 0; i < ToCardStacks.Count; i++)
        {
            cardStackNew = ToCardStacks[i].GetComponent<CardStackNew>();
            GameObject card = FromCardStack.GetComponent<CardStackNew>().Pop();
            card.GetComponent<CardViewNew>().toggleFace(true);
            card.GetComponent<BoxCollider>().enabled = true;
            cardStackNew.GetComponent<CardStackNew>().Push(card);


        }
    }

    public void Undo()
    {
        //take cards from the toCardStack (new owner) and put them into fromCardStack (old owner) in same order.
        Debug.Log("Undoing CommandDrawCards action");
        CardStackNew cardStackNew;
        CardStackViewNew cardStackViewNew;
        for (int i = ToCardStacks.Count - 1; i >= 0; i--)
        {
            cardStackNew = FromCardStack.GetComponent<CardStackNew>();
            GameObject card = ToCardStacks[i].GetComponent<CardStackNew>().Pop();
            card.GetComponent<CardViewNew>().toggleFace(false);
            card.GetComponent<BoxCollider>().enabled = false;
            cardStackNew.GetComponent<CardStackNew>().Push(card);


        }

    }
}
