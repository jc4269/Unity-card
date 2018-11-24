using UnityEngine;
using System.Collections;

public class CardModelNew : MonoBehaviour
{
    //index (-1 for facedown and 0-51 for deck of cards index.)
    public int Index;
    //stackIn: belongs to card stack
    public GameObject StackIn { get; set; }
    public bool faceUp = false;
    public int DeckID = -1;


}
