using System.Collections;
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

		//48 cards dealt over 8 columns
		for (int i = 0; i < 8; i++) {
			tempCS = columns [i].GetComponent<CardStack> ();
			for (int j = 0; j < 6; j++) {
				tempCS.push (deck.pop ());
			}
		}

		//remaining 4 go into the first 4 columns, one each.
		for (int i = 0; i < 4; i++) {
			tempCS = columns [i].GetComponent<CardStack> ();
			tempCS.push (deck.pop ());
		}
//		tempCS = freeCardRow.GetComponent<CardStack> ();
//		for (int j = 0; j < 2; j++) {
//			tempCS.push (deck.pop ());
//		}
//		tempCS = pileStackRow.GetComponent<CardStack> ();
//		for (int j = 0; j < 2; j++) {
//			tempCS.push (deck.pop ());
//		}

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
		return cardIndex / 13; // due to interger division, decimals will get trunicated resulting in 0-3 as return.
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
			//Debug.Log ("dorpped on FreeCardRow");
			//if freecardrow stack size < 4, then can add card.
			//Debug.Log("zone.GetComponent<CardStackView> ().getFetchedCardsSize () = "+zone.GetComponent<CardStackView> ().getFetchedCardsSize ());
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
			Debug.Log ("cardSuit="+cardSuit);
			Debug.Log ("cardRank="+cardRank);
			Debug.Log ("zoneLastCardSuit="+zoneLastCardSuit);
			Debug.Log ("zoneLastCardRank="+zoneLastCardRank);
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

	public bool checkWinGame (){
		//Check each such that the ranks of each card are are in decending order K to A. (if not then return false)
		//		This is the complicated check. simple check is for all cards to be in the pile area. but that means alot more time involved for players 
		//		and I like the way windows freegame does things.
		// freestack area is fine, they dont need to be counted.
		// pile stack area is ok because they can only be in sorted order.

		int lastEntryIndex = -1;
		CardStackView colCardStackView;
//		int lastEntryIndex = -1;
//		int entryIndex = -1;
//		int lastEntryRank = -1;
//		int lastEntrySuit = -1;
//		int entryRank = -1;
//		int entrySuit = -1;
		foreach (GameObject col in columns) {
			lastEntryIndex = -1;
			colCardStackView = col.GetComponent<CardStackView>();
			//if only 0 or 1 cards in column, it is valid to win game condition so its fine to continue on. 
			foreach(int entryIndex in colCardStackView.fetchedCards.Keys){
				
				if (lastEntryIndex == -1) { //first card in column
					lastEntryIndex = entryIndex;
					continue;
				}


//				lastEntryRank = getCardRankFromIndex (lastEntryIndex);
//				lastEntrySuit = getCardSuitFromIndex(lastEntryIndex);
//				entryRank = getCardRankFromIndex(entryIndex);
//				entrySuit = getCardSuitFromIndex(entryIndex);

				if (getCardRankFromIndex(lastEntryIndex) < getCardRankFromIndex(entryIndex)) { // if lastEntry is not higher than current entry, then not a valid win condition so can cancel search.
					return false;
				}


				lastEntryIndex = entryIndex;
			}
		}
		return true; // after getting through the column checks, if everything is ranked in decending order per column, then its a win.
	}
}
