using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeckModel))]
public class DeckView : MonoBehaviour {
	DeckModel deck;
	public Vector3 startPosition;
	public float cardOffset;
	public GameObject cardPrefab;


	void Start(){
		deck = GetComponent<DeckModel> ();
		showCards ();
	}
	//show a deck of cards setting position with offset and showing face up
	void showCards(){
		int cardCount = 0;
		float co;
		foreach (int i in deck.getCards()) {
			co = cardOffset * cardCount;
			GameObject cardCopy = (GameObject)Instantiate (cardPrefab);
			Vector3 temp = startPosition + (new Vector3 (co, 0f, 0f));
			cardCopy.transform.position = temp;
			CardModel cardModel = cardCopy.GetComponent<CardModel> ();
			cardModel.cardIndex = i;
			cardModel.toggleFace(true);
			cardCount++;
		}
	}
}
