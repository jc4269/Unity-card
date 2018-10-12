using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCellGameController : MonoBehaviour {

	public CardStack deck;
	public CardStack column;
	// Use this for initialization
	void Start () {
		deck.CreateDeck ();
		for (int i = 0; i < 5; i++) {
			column.push (deck.pop());
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
