using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour 
{
	List<int> cards;

	public bool isGameDeck = false;

	public bool hasCards{
		get { return cards != null && cards.Count > 0; }
	}

	public IEnumerable<int> getCards(){
		foreach (int i in cards) {
			yield return i;
		}
	}

	public int cardsCount(){
		if(cards == null){
			return 0;
		}
		//else
		return cards.Count;
	}

	public int pop(){
		//TODO: error checking, bounds checking.
		int temp = cards [0];
		cards.RemoveAt (0);
		return temp;
	}

	public void push (int index){
		cards.Add (index);
	}

	public void CreateDeck(){

		cards.Clear();

		//add unshuffled cards to list (deck)
		for(int i = 0; i < 52; i++){
			cards.Add(i);
			//Debug.Log (i);
		}


		int n = cards.Count; 	//number of cards in deck
		Debug.Log("n="+n);
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
		Debug.Log ("Start");
		cards = new List<int>();
		if (isGameDeck) {
			CreateDeck ();
		}
	}

}
