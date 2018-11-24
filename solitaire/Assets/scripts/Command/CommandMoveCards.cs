using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class CommandMoveCards(List<GameObject> cards, GameObject fromCardStack, GameObject toCardStack)
public class CommandMoveCards : ICommand
{
    List<GameObject> cards; // can be just one card.
    GameObject fromCardStack;
    GameObject toCardStack;

    public void Execute()
    {
        //take cards from the fromCardStack (old owner) and put them into toCardStack (new owner) in same order.
        Debug.Log("Executing CommandMoveCards action");

    }

    public void Undo()
    {
        //take cards from the toCardStack (new owner) and put them into fromCardStack (old owner) in same order.
        Debug.Log("Undoing CommandMoveCards action");
    }
}
