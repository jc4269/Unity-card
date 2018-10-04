using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardStack))]
public class CardStackView : MonoBehaviour 
{
	CardStack cardStack;
	List<int> fetchedCards;
	int lastCount;

	public Vector3 startPosition;
	public float cardOffset;
	public GameObject cardPrefab;


	void Start(){
		cardStack = GetComponent<CardStack> ();
		fetchedCards = new List<int>();
		showCards ();
		lastCount = cardStack.cardsCount();
	}

	void Update(){
		if(lastCount != cardStack.cardsCount()){
			showCards ();
			lastCount = cardStack.cardsCount();
		}
	}

	//show a deck of cards setting position with offset and showing face up, also dealing with render order.
	void showCards(){
		int cardCount = 0;
		float co;
		if (cardStack.hasCards) {
			foreach (int i in cardStack.getCards()) {
				co = cardOffset * cardCount;
				Vector3 temp = startPosition + (new Vector3 (co, 0f, 0f));
				addCard (temp, i, cardCount);
				cardCount++;
			}
		}
	}

	void addCard(Vector3 position, int cardIndex, int renderIndex){

		if (fetchedCards.Contains (cardIndex)) {
			return;
		}

		GameObject cardCopy = (GameObject)Instantiate (cardPrefab);
		cardCopy.transform.position = position;
		CardModel cardModel = cardCopy.GetComponent<CardModel> ();
		cardModel.cardIndex = cardIndex;
		cardModel.toggleFace (true);

		SpriteRenderer spriteRenderer = cardCopy.GetComponent<SpriteRenderer> ();
		spriteRenderer.sortingOrder = renderIndex; //left to right put down first to last.
		//spriteRenderer.sortingOrder = 51 - renderIndex; //right to left put down first to last.

		//keep track of added cards
		fetchedCards.Add(cardIndex);
	}
}
