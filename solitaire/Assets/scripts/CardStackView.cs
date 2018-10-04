using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardStack))]
public class CardStackView : MonoBehaviour 
{
	CardStack cardStack;
	Dictionary<int, GameObject> fetchedCards;
	int lastCount;

	public Vector3 startPosition;
	public float cardOffset;
	public GameObject cardPrefab;
	public bool faceUp = false;

	void Start(){
		cardStack = GetComponent<CardStack> ();
		fetchedCards = new Dictionary<int, GameObject>();
		showCards ();
		lastCount = cardStack.cardsCount();

		cardStack.cardRemoved += CardStack_cardRemoved;
	}

	void CardStack_cardRemoved (object sender, CardRemovedEventArgs e)
	{
		//Debug.Log ("cardRemoveEvent i="+e.cardIndex+", name="+fetchedCards[e.cardIndex].name);

		if (fetchedCards.ContainsKey (e.cardIndex)) {
			//Debug.Log ("exists so delete");
			//Debug.Log("is null? "+(fetchedCards [e.cardIndex] == null));
			//Debug.Log("name="+fetchedCards[e.cardIndex].name);
			Destroy (fetchedCards[e.cardIndex]);

			//Debug.Log("is null? "+(fetchedCards [e.cardIndex] == null));
			fetchedCards.Remove (e.cardIndex);
			//Debug.Log ("fetchedCards.Count"+ fetchedCards.Count);
		}
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
				//Debug.Log("card= "+i+"temp="+temp);
				addCard (temp, i, cardCount);
				cardCount++;
			}
			//Destroy (fetchedCards[cardCount-1]);
		}
		Debug.Log("cardStack.cardsCount ()="+cardStack.cardsCount ());

	}

	void addCard(Vector3 position, int cardIndex, int renderIndex){

		if (fetchedCards.ContainsKey (cardIndex)) {
			//Debug.Log ("Card already fetched");
			return;
		}

		GameObject cardCopy = (GameObject)Instantiate (cardPrefab);
		cardCopy.transform.position = position;
		CardModel cardModel = cardCopy.GetComponent<CardModel> ();
		cardModel.cardIndex = cardIndex;
		cardModel.toggleFace (faceUp);
		cardCopy.name = "Card"+cardIndex;

		SpriteRenderer spriteRenderer = cardCopy.GetComponent<SpriteRenderer> ();
		spriteRenderer.sortingOrder = renderIndex; //left to right put down first to last.
		//spriteRenderer.sortingOrder = 51 - renderIndex; //right to left put down first to last.

		//keep track of added cards
		fetchedCards.Add(cardIndex, cardCopy);
	}
}
