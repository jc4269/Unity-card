﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCellGameController : MonoBehaviour {

	public GameObject card;
	public CardStack deck;
	//public CardStack column;
	public GameObject column;
	public GameObject pileStackRow;
	public GameObject freeCardRow;
	public List<GameObject> columns;

	// Use this for initialization
	void Start () {
		boardSetup ();

		CardStack tempCS;
		for (int i = 0; i < 8; i++) {
			tempCS = columns [i].GetComponent<CardStack> ();
			for (int j = 0; j < 2; j++) {
				tempCS.push (deck.pop ());
			}
		}
		tempCS = freeCardRow.GetComponent<CardStack> ();
		for (int j = 0; j < 2; j++) {
			tempCS.push (deck.pop ());
		}
		tempCS = pileStackRow.GetComponent<CardStack> ();
		for (int j = 0; j < 2; j++) {
			tempCS.push (deck.pop ());
		}

	}

	void boardSetup(){
		deck.CreateDeck ();

		pileStackRow.transform.position = pileStackRow.GetComponent<CardStackView> ().startPosition;
		freeCardRow.transform.position = freeCardRow.GetComponent<CardStackView> ().startPosition;

		GameObject c;
		BoxCollider bc = column.GetComponent<BoxCollider> ();
		Vector3 sizeTemp = bc.size;
		sizeTemp.z = 0.2f;
		bc.size = sizeTemp;
		

		CardStackView columnCVS = column.GetComponent<CardStackView> ();
		Vector3 startPosition = columnCVS.startPosition;
		column.transform.position = startPosition;
		columns.Add (column);
		//Debug.Log ("startPosition="+startPosition);
		Vector3 temp;
		float columnOffset = 1.0f;
		float cs;
		for(int i = 0; i < 7; i++){
			c = (GameObject)Instantiate (column);
			cs = columnOffset * (i+1);
			temp = startPosition + (new Vector3 (cs, 0f, 0f));
			c.transform.position = temp;
			//bc = c.GetComponent<BoxCollider> ();
			//Debug.Log ("sizez="+bc.size.z);
			CardStackView csv = c.GetComponent<CardStackView> ();
			csv.cardOffset = -0.3f;
			csv.cardPrefab = card;
			csv.faceUp = true;
			csv.offsetHorizontal = false;
			csv.startPosition = temp;
			csv.reverseLayerOrder = false;
			columns.Add (c);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	//0 - hearts
	//1 - diamonds
	//2 - clubs
	//3 - spades
	int getCardSuitFromIndex (int cardIndex){
		return cardIndex / 4; // due to interger division, decimals will get trunicated resulting in 0-3 as return.
	}

	int getCardRankFromIndex (int cardIndex){
		return cardIndex % 13;

	}

	//zone is the column or row where card was dropped on.
	//c is card that was dropped.
	public bool checkIfValidDrop (GameObject zone, GameObject c){
		Debug.Log ("checkIfValidDrop in");
		Debug.Log ("zone.tag ="+ zone.tag);
		//if zone is freecardrow
		if(zone.tag == "FreeCardRow"){
			Debug.Log ("dorpped on FreeCardRow");
			//if freecardrow stack size < 4, then can add card.
			Debug.Log("zone.GetComponent<CardStackView> ().getFetchedCardsSize () = "+zone.GetComponent<CardStackView> ().getFetchedCardsSize ());
			if (zone.GetComponent<CardStackView> ().getFetchedCardsSize () < 4) {
				return true;
			}
		}

		//Free Card Row Zone doesn't need card suit/rank.. so is checked first.
		//other two zones need rank/suit.


		//get rank and suit of card
		int cardIndex = c.GetComponent<CardModel>().cardIndex;
		int cardRank = getCardRankFromIndex(cardIndex);
		int cardSuit = getCardSuitFromIndex(cardIndex);

		//get rank and suit of last card in zone.
		CardStackView lastZoneCardStackView = zone.GetComponent<CardStackView>();
		int zoneLastCardIndex = lastZoneCardStackView.getLastCardIndex();
		int zoneLastCardRank = -1; 
		int zoneLastCardSuit = -1;
		if (zoneLastCardIndex >= 0) {
			zoneLastCardRank = getCardRankFromIndex (zoneLastCardIndex);
			zoneLastCardSuit = getCardSuitFromIndex (zoneLastCardIndex);
		}
		//if zone is column
		if(zone.tag == "Column"){
			Debug.Log ("dorpped on Column");

			// if no cards on column, so add card - > value.
			if(zoneLastCardIndex == -1){ 
				return true;
			}
			//if card rank is one less than zone last card and suit of card is opposite colour to zone last card,
			if (cardSuit < 2 && zoneLastCardSuit >= 2 || cardSuit >= 2 && zoneLastCardSuit < 2) { //make sure suit is different colour
				if(cardRank == zoneLastCardRank-1){ // card needs to be one less than zone card
					//then valid, add card to column.
					return true;
				}
			}

		}

		//if zone is pilestackrow
		if (zone.tag == "PileStackRow") {
			Debug.Log ("dorpped on PileStackRow");
			//if card is 1 rank higher than card in its suit pile. (no cards means pile needs an ace)
			return true;
		}

		return false;
	}
}
