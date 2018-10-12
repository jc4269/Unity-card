using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour 
{
	List<int> cards;

	public bool isGameDeck = false;

	public event CardEventHandler cardRemoved;
	public event CardEventHandler cardAdded;

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

	//calculate hand value
	public int handValue(){
		int total = 0;
		int cardRank;
		int aces = 0; // count the number of aces in hand, used later to do 1 or 11 logic
		foreach (int card in getCards()) {
			cardRank = card % 13;
			//Debug.Log ("Cardrank="+cardRank);
			if (cardRank >= 10 && cardRank <= 12) { // J, Q, K
				cardRank = 10; // J,Q,K worth 10 points

			} 
			else if (cardRank == 0) { // ace
				aces++;
				continue; // dont want to add cardrank to total.
			} 
			else { // pip value 2-10
				cardRank += 1; // needs one added to get correct value.
			}

			total += cardRank; // add hand value up.
		}

		for (int i = 0; i < aces; i++) {
			if (total + 11 <= 21) {
				total += 11;
			} else {
				total += 1;
			}
		}
		return total;
	}

	public int pop(){
		//TODO: error checking, bounds checking.
		//remove card from stack and send removal event
		int temp = cards [0];
		cards.RemoveAt (0);
		if (cardRemoved != null) {
			cardRemoved (this, new CardEventArgs (temp));
		}
		return temp;
	}

	public void push (int cardIndex){
		cards.Add (cardIndex);

		if (cardAdded != null) {
			cardAdded (this, new CardEventArgs (cardIndex));
		}

	}

	public void CreateDeck(){

		reset ();

		//add unshuffled cards to list (deck)
		for(int i = 0; i < 52; i++){
			cards.Add(i);
			//Debug.Log (i);
		}


		int n = cards.Count; 		//number of cards in deck
		//Debug.Log("n="+n);
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

	public void reset(){
		cards.Clear ();
	}

	// Use this for initialization
	void Awake () {
		//Debug.Log ("Start");
		cards = new List<int>();
		if (isGameDeck) {
			CreateDeck ();
		}
	}

}
