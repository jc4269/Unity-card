using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckModel : MonoBehaviour 
{
	List<int> cards;

	public void shuffle(){

		//instantiate card list if not already
		if(cards == null){
			cards = new List<int>();
		}
		else{ // clear list
			cards.Clear();
		}

		//add unshuffled cards to list (deck)
		for(int i = 0; i < 52; i++){
			cards.Add(i);
		}


		int n = cards.Count; 	//number of cards in deck

		int k; 						// random value
		int temp; 					// holds int for swapping
		while (n > 1) {
			n--;
			k = Random.Range(0, n+1);
			temp = cards [k];
			cards [k] = cards [n];
			cards [n] = temp; 
		}
	}

	// Use this for initialization
	void Start () {
		shuffle ();
	}

}
