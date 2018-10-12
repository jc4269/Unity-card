using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardStack))]
public class CardStackView : MonoBehaviour 
{
	CardStack cardStack;
	Dictionary<int, CardView> fetchedCards;
	int lastCount;

	public Vector3 startPosition;
	public float cardOffset;
	public bool offsetHorizontal = true; //true for horizontal, false for vertical offset.
	public GameObject cardPrefab;
	public bool faceUp = false;
	public bool reverseLayerOrder = false;

	public void Toggle(int cardIndex, bool isFaceUp){
		fetchedCards [cardIndex].faceUp = isFaceUp;
	}

	void Awake(){
		cardStack = GetComponent<CardStack> ();
		fetchedCards = new Dictionary<int, CardView>();
		showCards ();
		lastCount = cardStack.cardsCount();

		cardStack.cardRemoved += CardStack_cardRemoved;
		cardStack.cardAdded += CardStack_cardAdded;
	}

	void CardStack_cardAdded (object sender, CardEventArgs e)
	{
		float co = cardOffset * cardStack.cardsCount();
		Vector3 temp = startPosition + offsetPositionWithDirection(co);
		addCard (temp, e.cardIndex, cardStack.cardsCount());
	}

	void CardStack_cardRemoved (object sender, CardEventArgs e)
	{
		if (fetchedCards.ContainsKey (e.cardIndex)) {
			Destroy (fetchedCards[e.cardIndex].card);
			fetchedCards.Remove (e.cardIndex);
		}
	}

	void Update(){
		if(lastCount != cardStack.cardsCount()){
			//Debug.Log ("updating updating");
			showCards ();
			lastCount = cardStack.cardsCount();
		}
	}

	public void clear (){
		cardStack.reset ();
		foreach (CardView cv in fetchedCards.Values) {
			Destroy (cv.card);
		}
		fetchedCards.Clear ();
		lastCount = 0;
	}

	//show a deck of cards setting position with offset and showing face up, also dealing with render order.
	public void showCards(){
		int cardCount = 0;
		float co;
		if (cardStack.hasCards) {
			foreach (int i in cardStack.getCards()) {
				co = cardOffset * cardCount;

				Vector3 temp = startPosition + offsetPositionWithDirection(co);

				addCard (temp, i, cardCount);
				cardCount++;
			}
		}
		//Debug.Log("cardStack.cardsCount ()="+cardStack.cardsCount ());

	}

	void addCard(Vector3 position, int cardIndex, int renderIndex){

		if (fetchedCards.ContainsKey (cardIndex)) {

			if (!faceUp) {
				CardModel cModel = fetchedCards [cardIndex].card.GetComponent<CardModel> ();
				cModel.toggleFace (fetchedCards [cardIndex].faceUp);
			}
			return;
		}

		GameObject cardCopy = (GameObject)Instantiate (cardPrefab);
		//Debug.Log ("position="+position);
		cardCopy.transform.position = position;
		CardModel cardModel = cardCopy.GetComponent<CardModel> ();
		cardModel.cardIndex = cardIndex;
		cardModel.toggleFace (faceUp);
		cardCopy.name = "Card"+cardIndex;

		SpriteRenderer spriteRenderer = cardCopy.GetComponent<SpriteRenderer> ();
		if(reverseLayerOrder){
			spriteRenderer.sortingOrder = 51 - renderIndex; //right to left put down first to last.
		}
		else{
			spriteRenderer.sortingOrder = renderIndex; //left to right put down first to last.
		}

		//keep track of added cards
		fetchedCards.Add(cardIndex, new CardView(cardCopy));

		//Debug.Log ("Hand value = " + cardStack.handValue ());
	}

	//private helper function since this code is used twice in script.
	//offsetHorizontal is needed but this function has access and is fine since private.
	Vector3 offsetPositionWithDirection (float co){
		Vector3 offset;
		if (offsetHorizontal) {
			offset = (new Vector3 (co, 0f, 0f));
		} else {
			offset = (new Vector3 (0f, co, 0f));
		}
		return offset;
	}

}
