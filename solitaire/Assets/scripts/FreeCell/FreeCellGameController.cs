using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FreeCellGameController : MonoBehaviour {

	public GameObject card;
	public CardStack deck;
	//public CardStack column;
	public GameObject column;
	public GameObject pileStackRow;
	public GameObject freeCardRow;

	public Button playAgainButton;
	public Text feedBackText;

    public GameObject rowColBackground;
    public GameObject pileRowBackground;
    public GameObject freeCardBackground;

    public List<GameObject> columns;
    public List<GameObject> cardsToMoveAsColumn;

    public void backToMenu(int menuIndex){
        //clean up memory
        resetBoard();

        //TODO: do i need to get rid of columns?

        //then load menu
        SceneManager.LoadScene(menuIndex);
    }

    void resetBoard(){
        pileStackRow.GetComponent<CardStackView>().clear();
        freeCardRow.GetComponent<CardStackView>().clear();
        for (int i = 0; i < 8; i++)
        {
            columns[i].GetComponent<CardStackView>().clear();
        }
    }

	public void playAgain(){
		Debug.Log ("Play Again Pressed");
        //disable play again button
        //		playAgainButton.interactable = false;	//reset board
        resetBoard();

        gameSetup ();
//		//deck.GetComponent<CardStackView> ().showCards ();
//		dealersFirstCard = -1;
//
//		//enable gameplay buttons
//		hitButton.interactable = true;
//		standButton.interactable = true;
//
//
//		//reset feedback text
//		feedBackText.text = "Playing again!";
//
//		startGame ();
	}

	// Use this for initialization
	void Start () {


        boardSetup ();
		gameSetup ();
	}

	void gameSetup(){
		deck.CreateDeck ();

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
	}

	void boardSetup(){
		

		pileStackRow.transform.position = pileStackRow.GetComponent<CardStackView> ().startPosition;
		freeCardRow.transform.position = freeCardRow.GetComponent<CardStackView> ().startPosition;
        pileRowBackground.transform.position = pileStackRow.GetComponent<CardStackView>().startPosition + new Vector3(2f, 0f, 0f);
        freeCardBackground.transform.position = freeCardRow.GetComponent<CardStackView>().startPosition + new Vector3(2f, 0f, 0f);

        GameObject c;
        GameObject rowColBackgroundTemp;
		BoxCollider bc = column.GetComponent<BoxCollider> ();
		Vector3 sizeTemp = bc.size;
		sizeTemp.z = 0.2f;
		bc.size = sizeTemp;

        Vector3 rowColScale = new Vector3(1, 4, 0);

        CardStackView columnCVS = column.GetComponent<CardStackView> ();
		Vector3 startPosition = columnCVS.startPosition;
		column.transform.position = startPosition;
		columns.Add (column);

        rowColBackgroundTemp = (GameObject)Instantiate(rowColBackground);
        rowColBackgroundTemp.transform.localScale = rowColScale;
        Vector3 backgroundOffset = new Vector3(0f, -2f, 0f);
        rowColBackgroundTemp.transform.position = startPosition + backgroundOffset;
        //Debug.Log ("startPosition="+startPosition);
        Vector3 temp;
		float columnOffset = 1.0f;
		float cs;
		for(int i = 0; i < 7; i++){
			c = (GameObject)Instantiate (column);
            rowColBackgroundTemp = (GameObject)Instantiate(rowColBackground);
            rowColBackgroundTemp.transform.localScale = rowColScale;
            cs = columnOffset * (i+1);
			temp = startPosition + (new Vector3 (cs, 0f, 0f));
			c.transform.position = temp;
            rowColBackgroundTemp.transform.position = temp + backgroundOffset;
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

    public void printCardSuitRankFromIndex(int cardIndex){

        Debug.Log("Card Index="+cardIndex+", Card Suit="+ getCardSuitFromIndex(cardIndex) +", Card Rank="+getCardRankFromIndex(cardIndex));

    }
	//0 - hearts
	//1 - diamonds
	//2 - clubs
	//3 - spades
	public int getCardSuitFromIndex (int cardIndex){
		return cardIndex / 13; // due to interger division, decimals will get trunicated resulting in 0-3 as return.
	}

	public int getCardRankFromIndex (int cardIndex){
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

	public void onMouseUpGameLogic(List<GameObject> gcDraggedList, Vector3 mousePosition, CardStackView cardStackViewOfCardBeingMoved){
		if(gcDraggedList.Count > 0){
            GameObject selectedCardDragged = gcDraggedList[0]; // first card in list is the base card of column being dragged.
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
				CardModel cm = selectedCardDragged.GetComponent<CardModel> ();
				//Debug ("card suit="+getCardSuitFromIndex( cm.cardIndex) + ", card rank=" + getCardRankFromIndex( cm.cardIndex));
				GameObject cmzoneIn = cm.zoneIn;
				CardStackView cmzoneInCardStackView = cmzoneIn.GetComponent<CardStackView> (); 
				//Debug.Log("getmouseup.name="+cmzoneIn);
				//FreeCellGameController freeCellGC = GetComponent<FreeCellGameController> ();
				if (checkIfValidDrop (cardStackHit, selectedCardDragged)) {
                    //valid so update all cards in column being dragged.
                    for (int i = 0; i < gcDraggedList.Count; i++)
                    {
                        cm = gcDraggedList[i].GetComponent<CardModel>();
                        cmzoneInCardStackView.removeCardFromFetchedWithIndex(cm.cardIndex);
                        cm.zoneIn = cardStackHit; //updating to cards new zone
                        cardStackHitCardStackView.addCardToFetchedWithIndexAndCard(cm.cardIndex, gcDraggedList[i]);
                    }
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

    //check if card to pile end is valid to move as a column (descending rank and alternating card colour)
    public bool isCardToColumnEndInValidOrder(GameObject c){
        CardStackView csvOfColumnCardIsIn = c.GetComponent<CardModel>().zoneIn.GetComponent<CardStackView>();
        //go through fetched list to find where selected card is then do check from there
        bool foundSelectedCard = false;
        int previousCardIndex = -1;
        int selectedCardIndex = c.GetComponent<CardModel>().cardIndex;
        foreach (KeyValuePair<int, CardView> entry in csvOfColumnCardIsIn.fetchedCards){
            //find selected card
            if(!foundSelectedCard){
                if(entry.Key == selectedCardIndex){
                    foundSelectedCard = true;
                }
            }
            else{ // start checking for valid column to move all card down from selected card
                if(!isCardValidToBeOnTopOfAnotherCardInColumn(previousCardIndex, entry.Key)){
                    return false;
                }
            }
            previousCardIndex = entry.Key;

        }
        return true;
    }

    //assumes there is a previous card to check. still need to check if previous card is there.
    bool isCardValidToBeOnTopOfAnotherCardInColumn(int previousCardIndex, int currentCardIndex){
        //if current card rank is one less than previous card and suit of current card is opposite colour to previous card,
        int currentCardRank = getCardRankFromIndex(currentCardIndex);
        int currentCardSuit = getCardSuitFromIndex(currentCardIndex);
        int previousCardRank = getCardRankFromIndex(previousCardIndex);
        int previousCardSuit = getCardSuitFromIndex(previousCardIndex);

        if (currentCardSuit < 2 && previousCardSuit >= 2 || currentCardSuit >= 2 && previousCardSuit < 2)
        { //make sure suit is different colour
            if (currentCardRank == previousCardRank - 1)
            { // card needs to be one less than zone card
              //then valid, add card to column.
                return true;
            }
        }
        return false;
    }

    bool isEnoughFreeSpacesToMoveCardColumn(GameObject c){
        CardStackView csvOfColumnCardIsIn = c.GetComponent<CardModel>().zoneIn.GetComponent<CardStackView>();
        //go through fetched list to find where selected card is then do check from there
        bool foundSelectedCard = false;
        int previousCardIndex = -1;
        int selectedCardIndex = c.GetComponent<CardModel>().cardIndex;
        int cardColumnSize = 0;
        foreach (KeyValuePair<int, CardView> entry in csvOfColumnCardIsIn.fetchedCards)
        {
            //find selected card
            if (!foundSelectedCard)
            {
                if (entry.Key == selectedCardIndex)
                {
                    foundSelectedCard = true;
                    cardColumnSize++;
                }
            }
            else
            { // start checking for valid column to move all card down from selected card
                cardColumnSize++;
            }
            previousCardIndex = entry.Key;

        }
        int numberOfFreeSpaces = 4- freeCardRow.GetComponent<CardStackView>().getFetchedCardsSize();
        int numberOfColumnsEmpty = getNumberOfColumnsEmpty();
        Debug.Log("is enough free space: cardColumnSize ="+ cardColumnSize + ", numberOfFreeSpaces="+ numberOfFreeSpaces+ ", numberOfColumnsEmpty="+ numberOfColumnsEmpty);
        float largestColumnSizeMovable = (1 + numberOfFreeSpaces) * (Mathf.Pow(2, numberOfColumnsEmpty));
        Debug.Log("largestColumnSizeMovable="+ largestColumnSizeMovable);
        if (cardColumnSize <= (1+numberOfFreeSpaces)*(Mathf.Pow(2,numberOfColumnsEmpty))){
            return true;
        }

        return false;
    }

    private int getNumberOfColumnsEmpty(){
        GameObject col;
        CardStackView colCSV;
        int count = 0;
        for (int i = 0; i < 8; i++){
            col = columns[i];
            colCSV = col.GetComponent<CardStackView>();
            //Debug.Log("i="+i+ ", colCSV.getFetchedCardsSize()=" + colCSV.getFetchedCardsSize());
            if(colCSV.getFetchedCardsSize() <= 0){
                count++;
            }
        }
        return count;
    }

    public List<GameObject> onMouseDownObjectsToMove(GameObject gcClicked, Vector3 mousePosition, CardStackView cardStackViewOfCardBeingMoved)
    {
        //check if column
        if (cardStackViewOfCardBeingMoved.tag == "Column")
        {
            Debug.Log("is a column");
            //check if card to pile end is valid (descending rank and alternating card colour)
            if (isCardToColumnEndInValidOrder(gcClicked))
            {
                Debug.Log("card till end is valid");
                if(isEnoughFreeSpacesToMoveCardColumn(gcClicked)){
                    Debug.Log("ENOUGH FREE SPACE");
                    //all good, create a list of cards to move in a column.
                    cardsToMoveAsColumn = getCardsFromSelectedToEndOfColumn(gcClicked);
                }
                else{
                    Debug.Log("NOT ENOUGH FREE SPACE");
                }
            }
            else
            {
                Debug.Log("NOT card till end is valid");
            }
        }
        return cardsToMoveAsColumn;
    }

    private List<GameObject> getCardsFromSelectedToEndOfColumn(GameObject c){
        CardStackView csvOfColumnCardIsIn = c.GetComponent<CardModel>().zoneIn.GetComponent<CardStackView>();
        //go through fetched list to find where selected card is then do check from there
        bool foundSelectedCard = false;
        int selectedCardIndex = c.GetComponent<CardModel>().cardIndex;
        List<GameObject> temp = new List<GameObject>();
        foreach (KeyValuePair<int, CardView> entry in csvOfColumnCardIsIn.fetchedCards)
        {
            //find selected card
            if (!foundSelectedCard)
            {
                if (entry.Key == selectedCardIndex)
                {
                    foundSelectedCard = true;
                    temp.Add(entry.Value.card);
                }
            }
            else
            { // start checking for valid column to move all card down from selected card
                temp.Add(entry.Value.card);
            }


        }

        return temp;
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
