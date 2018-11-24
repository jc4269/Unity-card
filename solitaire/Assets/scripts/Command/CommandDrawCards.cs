using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//action: moves card(s) from one stack to another.
public class CommandDrawCards : ICommand
{
    int CardsToDraw;
    GameObject FromCardStack;
    GameObject ToCardStack;

    public CommandDrawCards(int cardsToDraw, GameObject fromCardStack, GameObject toCardStack)
    {
        CardsToDraw = cardsToDraw;
        FromCardStack = fromCardStack;
        ToCardStack = toCardStack;
    }

    public void Execute()
    {
        //take cards from the fromCardStack (old owner) and put them into toCardStack (new owner) in same order.
        Debug.Log("Executing CommandDrawCards action");
        DrawCards(true, FromCardStack, ToCardStack);
    }

    //Helper function to allow easier implementation of reversing the stack to and from. 
    //Does this in a true stack fashion drawing or putting back one at a time from the top of either pile.. this is a one a time deal and not a list like CommandMoveCards class 
    void DrawCards(bool faceUp, GameObject fromCardStack, GameObject toCardStack)
    {
        //Debug.Log("CardsToDraw="+ CardsToDraw);
        for (int i = 0; i < CardsToDraw; i++)
        {
            if (fromCardStack.GetComponent<CardStackNew>().Cards.Count > 0)
            {
                GameObject card = fromCardStack.GetComponent<CardStackNew>().Pop();
                card.GetComponent<CardViewNew>().toggleFace(faceUp);
                toCardStack.GetComponent<CardStackNew>().Push(card);
            }
            else
            { // deck is empty no need to continue

                break;
            }
        }
    }

    public void Undo()
    {
        //take cards from the toCardStack (new owner) and put them into fromCardStack (old owner) in same order.
        Debug.Log("Undoing CommandDrawCards action");
        DrawCards(false, ToCardStack, FromCardStack);

    }
}
