using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
	int dealersFirstCard = -1;

	public CardStack player;
	public CardStack dealer;
	public CardStack deck;

	public Button hitButton;
	public Button standButton;

	/***
	 * Deal 2 cards to each player and dealer.
	 * 	Dealers cards have first face down and rest face up.
	 * 	Players cards are all face up.
	 * Players turn is first, choosing to "HIT" and get another card or "STAND" and pass on turn with current hand value.
	 * Dealers turn must HIT if <17 in hand value.
	 * 
	 */

	#region Public methods

	public void hit(){
		player.push(deck.pop());
		//Debug.Log ("hand value = "+player.handValue ());
		if (player.handValue () > 21) {
			//TODO: player bust
			hitButton.interactable = false;
			standButton.interactable = false;
		}
	}
	public void stand(){
		//foreach(int card in dealer.ca)
		StartCoroutine(dealersTurn());
		//TODO: ends players turn with current hand value and dealer turn to reveal cards and keep hitting till handvalue is 17 or more
	}

	#endregion

	#region Unity messages

	void Start(){
		startGame ();
	}

	#endregion

	void startGame(){
		//deal cards to player and dealer
		for(int i = 0; i<2; i++){
			player.push(deck.pop());
			HitDealer (); 
		}
	}

	void HitDealer (){
		int card = deck.pop ();
		dealer.push(card);

		if (dealersFirstCard < 0)
			dealersFirstCard = card;
		
		if (dealer.cardsCount () >= 2) {
			CardStackView view = dealer.GetComponent<CardStackView> ();
			view.Toggle (card, true);
		}
	}

	IEnumerator dealersTurn(){
		CardStackView view = dealer.GetComponent<CardStackView> ();
		view.Toggle (dealersFirstCard, true);
		yield return new WaitForSeconds (1f);
		while (dealer.handValue () < 17) {
			HitDealer ();
			yield return new WaitForSeconds (1f);
		}
	}

}
