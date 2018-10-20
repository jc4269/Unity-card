using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeCellGameController : MonoBehaviour {

	public GameObject card;
	public CardStack deck;
	//public CardStack column;
	public GameObject column;
	public GameObject pileStackRow;
	public GameObject freeCardRow;

	public Text feedBackText;

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
//			Debug.Log ("dorpped on Column");
//			Debug.Log ("cardSuit="+cardSuit);
//			Debug.Log ("cardRank="+cardRank);
//			Debug.Log ("zoneLastCardSuit="+zoneLastCardSuit);
//			Debug.Log ("zoneLastCardRank="+zoneLastCardRank);
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

			int[] piles = suitPileStackVisibleView (lastZoneCardStackView);
			//if card is 1 rank higher than card in its suit pile. (no cards means pile needs an ace)
			// ace case (lowest card): since empty is -1, +1 = 0 therefore ace handled
			// king case (highest card): since only one deck and only one copy of each card, there shouldn't be anything higher so should be handled.
			if(piles[cardSuit]+1 == cardRank){
				return true;
				//lastZoneCardStackView.addCardToFetchedWithIndexAndCard (cardIndex, c);
			}
			//update pile view
			//updatePileStackView(lastZoneCardStackView);
			//return true;
		}

		return false;
	}

	//custom update function for specific implementation pilestackview (a cardstack/cardstackview object)
	void updatePileStackView(CardStackView csv){
		int i = 0;
		GameObject c = null;
		SpriteRenderer csr;
		Vector3 temp;
		float co;
		CardStackView pileStackRowCSV;
		// stores the entries at the top of each suit pile (hearts=0, diamonds=1, clubs=2, spades=3)
		List<KeyValuePair<int, CardView>> suitPile = new List<KeyValuePair<int, CardView>>();
		for(int j = 0; j < 4; j++){
			suitPile.Add(new KeyValuePair<int, CardView>(-1, null));
		}
//		suitPile.Add(null);
//		suitPile.Add(null);
//		suitPile.Add(null);

		int entryCardIndex;
		int entryCardSuit;
		int entryCardRank;
		int suitPileRank;
		foreach(KeyValuePair<int, CardView> entry in csv.fetchedCards){
			c = entry.Value.card;
			c.GetComponent<BoxCollider> ().enabled = false; // set all cards to disabled, will set top of piles back after
			entryCardIndex = entry.Key;
			entryCardSuit = getCardSuitFromIndex(entryCardIndex);
			entryCardRank = getCardRankFromIndex (entryCardIndex);
			Debug.Log("value of suitpile[entrycardindex]="+suitPile [entryCardSuit].Key);
			Debug.Log("entryCardSuit="+entryCardSuit+", entryCardRank="+entryCardRank);
			if (suitPile [entryCardSuit].Key == -1) { // empty
				suitPile [entryCardSuit] = entry;
			} else {
				suitPileRank = getCardRankFromIndex(suitPile[entryCardSuit].Key);
				Debug.Log("suitCardRank="+suitPileRank);
				//if card of same suit is higher than previous recorded, update suitpile column with current card entry.
				if (suitPileRank < entryCardRank) {
					suitPile [entryCardSuit] = entry;
				}
			}





			csr = c.GetComponent<SpriteRenderer> ();
			//Vector3 temp = c.transform.position;
			co = 1.1f * entryCardSuit;

			pileStackRowCSV = pileStackRow.GetComponent<CardStackView> ();
			temp = pileStackRowCSV.startPosition + pileStackRowCSV.offsetPositionWithDirection(co);

			c.transform.position = temp;
			csr.sortingOrder = entryCardRank;

			i++;
		}


		//set cardstack object collider size to encompass all cards.
		//TODO:see if this dynamicism is still needed?

		//set pile top cards to enabled.
		for(int j = 0; j < 4; j++){
			Debug.Log ("j="+j+", rank="+getCardRankFromIndex(suitPile [j].Key));
			if (suitPile [j].Key != -1) {
				suitPile [j].Value.card.GetComponent<BoxCollider> ().enabled = true;
			}
		}
	}

	//return an array of four ints representing the the suit piles with highest rank card on the top of the pile (hearts=0, diamonds=1, clubs=2, spades=3)
	int[] suitPileStackVisibleView(CardStackView csv){
		int[] piles = new int[4];
		for (int i = 0; i < 4; i++) {
			piles [i] = -1; //-1 means there is nothing in the corresponding suit pile
		}
		foreach(KeyValuePair<int, CardView> entry in csv.fetchedCards){
			int cardIndex = entry.Key;
			int cardSuit = getCardSuitFromIndex (cardIndex);
			int cardRank = getCardRankFromIndex (cardIndex);
			//looking for highest rank card for each suit.
			if (piles [cardSuit] < cardRank) {
				piles [cardSuit] = cardRank; 
			}
		}

		return piles;
	}

	public void onMouseUpGameLogic(GameObject gcDragged, Vector3 mousePosition, CardStackView cardStackViewOfCardBeingMoved){
		if(gcDragged != null){
			//hit test to see which column or row its over.
			Ray ray = Camera.main.ScreenPointToRay (mousePosition);
			RaycastHit hit;
			//LayerMask layerMask;
			//layerMask = 1 << LayerMask.NameToLayer ("CardStack"); // only check for collisions with cards
			//hit = raycastFirstHitWithLayerMask(ray, layerMask);
			//Debug.Log("CardStackLayerNumber:"+LayerMask.GetMask("CardStack"));
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, LayerMask.GetMask ("CardStack"))) {
				//Debug.Log ("stackname:" + hit.collider.gameObject.name);
				//Debug.Log ("stacktag:" + hit.collider.gameObject.tag);
				GameObject cardStackHit = hit.collider.gameObject;
				CardStackView cardStackHitCardStackView = cardStackHit.GetComponent<CardStackView> ();
				CardModel cm = gcDragged.GetComponent<CardModel> ();
				//Debug ("card suit="+getCardSuitFromIndex( cm.cardIndex) + ", card rank=" + getCardRankFromIndex( cm.cardIndex));
				GameObject cmzoneIn = cm.zoneIn;
				CardStackView cmzoneInCardStackView = cmzoneIn.GetComponent<CardStackView> (); 
				//Debug.Log("getmouseup.name="+cmzoneIn);
				//FreeCellGameController freeCellGC = GetComponent<FreeCellGameController> ();
				if (checkIfValidDrop (cardStackHit, gcDragged)) {
					cmzoneInCardStackView.removeCardFromFetchedWithIndex (cm.cardIndex);
					cm.zoneIn = cardStackHit; //updating to cards new zone
					cardStackHitCardStackView.addCardToFetchedWithIndexAndCard (cm.cardIndex, gcDragged);

					//update cardStack that Card is going into, the hit stack
					if (cardStackHit.tag == "PileStackRow") {
						updatePileStackView (cardStackHitCardStackView);
					} else {
						cardStackHitCardStackView.updateCardViewStackFetchedCardsVisually ();
					}

					//update cardStack that Card coming from, last zone card was in.
					if (cmzoneIn.tag == "PileStackRow") {
						updatePileStackView (cmzoneInCardStackView);
					} else {
						cmzoneInCardStackView.updateCardViewStackFetchedCardsVisually ();
					}


					//check win condition after every valid action.

					bool didWinGame = checkWinGame ();
					Debug.Log("win="+didWinGame);
					if (didWinGame) {
						feedBackText.text = "YOU WON!!!   Play Again?";
					}

				} else {
					//snap back to card stack if not valid
					if (cmzoneIn.tag == "PileStackRow") {
						updatePileStackView (cmzoneInCardStackView);
					} else {
						cmzoneIn.GetComponent<CardStackView> ().updateCardViewStackFetchedCardsVisually ();
					}
				}
			} else {
				//didn't drop onto stack, so snap back
				if (cardStackViewOfCardBeingMoved.gameObject.tag == "PileStackRow") {
					updatePileStackView (cardStackViewOfCardBeingMoved);
				} else {
					cardStackViewOfCardBeingMoved.updateCardViewStackFetchedCardsVisually ();
				}
			}
		}
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
