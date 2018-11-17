using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpiderSolitaireGameController : MonoBehaviour, IGameController {

    public GameObject Card;
    public GameObject Deck;
    //public CardStack column;
    public GameObject Column;
    public GameObject PileStackRow;
    //public GameObject DrawnPileColumn;

    public Button playAgainButton;
    public Text feedBackText;
    //public Text CardDrawnText;
    //public Button CardDrawnAmountButton;
    //public Button ResetDeckButton;

    public GameObject RowColBackground;
    public GameObject PileRowBackground;
    public GameObject DeckBackground;

    public List<GameObject> columns;
    public List<GameObject> cardsToMoveAsColumn;
    private int CardDrawAmount = 1;
// --- Button Functions

    public void backToMenu(int menuIndex)
    {
        //clean up memory
        resetBoard();

        //TODO: do i need to get rid of columns?

        //then load menu
        SceneManager.LoadScene(menuIndex);
    }

    public void playAgain()
    {
        //Debug.Log("play again");
        resetBoard();
        gameSetup();
    }

// --- END Button Functions
    void resetBoard()
    {
        PileStackRow.GetComponent<CardStackNew>().Reset();
        //DrawnPileColumn.GetComponent<CardStackNew>().Reset();
        Deck.GetComponent<CardStackNew>().Reset();
        for (int i = 0; i < columns.Count; i++)
        {
            columns[i].GetComponent<CardStackNew>().Reset();
        }
        //columns.Clear();
    }

    void gameSetup(){

        CreateTwoDecks(Deck);
        //deck cards are face down
        for (int i = 0; i < Deck.GetComponent<CardStackNew>().Cards.Count; i++)
        {
            Deck.GetComponent<CardStackNew>().Cards[i].GetComponent<CardViewNew>().toggleFace(false);
        }

        CardStackNew cardStackNew;
        CardStackViewNew cardStackViewNew;
        GameObject card = null;
        //5 cards for each column
        for (int i = 0; i <columns.Count; i++)
        {
            cardStackNew = columns[i].GetComponent<CardStackNew>();
            for (int j = 0; j < 5; j++) // use column index as a quick way to give the correct cards out to each column
            {
                card = Deck.GetComponent<CardStackNew>().Pop();
                card.GetComponent<CardViewNew>().toggleFace(false);
                cardStackNew.Push(card);
            }
        }
        //first 4 columns get 1 extra card each.
        for (int i = 0; i < 4; i++)
        {
            cardStackNew = columns[i].GetComponent<CardStackNew>();
            card = Deck.GetComponent<CardStackNew>().Pop();
            card.GetComponent<CardViewNew>().toggleFace(false);
            cardStackNew.Push(card);
        }

        //set all last cards in column to face up and update the column view
        for (int i = 0; i < columns.Count; i++)
        {
            cardStackNew = columns[i].GetComponent<CardStackNew>();
            card = cardStackNew.Cards[cardStackNew.Cards.Count-1];
            //last card needs to be visable
            card.GetComponent<CardViewNew>().toggleFace(true);
            cardStackViewNew = columns[i].GetComponent<CardStackViewNew>();

            //update view
            cardStackViewNew.UpdateStackView();
        }

        //remaining cards stay in the deck
        //move remaining cards in deck into position
        //Deck.GetComponent<CardStackViewNew>().UpdateStackView();
        DeckUpdateView();
    }

    void boardSetup(){
        PileStackRow.transform.position = PileStackRow.GetComponent<CardStackViewNew>().StartPosition;
        //DrawnPileColumn.transform.position = DrawnPileColumn.GetComponent<CardStackViewNew>().StartPosition;
        Deck.transform.position = Deck.GetComponent<CardStackViewNew>().StartPosition;
        //PileRowBackground.transform.position = PileStackRow.GetComponent<CardStackView>().startPosition + new Vector3(2f, 0f, 0f);
        //DrawnPileBackground.transform.position = DrawnPileColumn.GetComponent<CardStackView>().startPosition + new Vector3(2f, 0f, 0f);
        GameObject c;
        GameObject rowColBackgroundTemp;
        BoxCollider bc = Column.GetComponent<BoxCollider>();
        Vector3 sizeTemp = bc.size;
        sizeTemp.z = 0.2f;
        bc.size = sizeTemp;

        Vector3 rowColScale = new Vector3(1, 4, 0);

        CardStackViewNew cardStackViewNew = Column.GetComponent<CardStackViewNew>();
        CardStackNew cardStackNew = Column.GetComponent<CardStackNew>();
        Vector3 startPosition = cardStackViewNew.StartPosition;
        Column.transform.position = startPosition;
        columns.Add(Column);

        rowColBackgroundTemp = (GameObject)Instantiate(RowColBackground);
        rowColBackgroundTemp.transform.localScale = rowColScale;
        Vector3 backgroundOffset = new Vector3(0f, -2f, 0f);
        rowColBackgroundTemp.transform.position = startPosition + backgroundOffset;
        //Debug.Log ("startPosition="+startPosition);
        Vector3 temp;
        float columnOffset = 1.0f;
        float cs;
        for (int i = 0; i < 9; i++)
        {
            c = (GameObject)Instantiate(Column);
            rowColBackgroundTemp = (GameObject)Instantiate(RowColBackground);
            rowColBackgroundTemp.transform.localScale = rowColScale;
            cs = columnOffset * (i + 1);
            temp = startPosition + (new Vector3(cs, 0f, 0f));
            c.transform.position = temp;
            rowColBackgroundTemp.transform.position = temp + backgroundOffset;
            //bc = c.GetComponent<BoxCollider> ();
            //Debug.Log ("sizez="+bc.size.z);
            cardStackViewNew = c.GetComponent<CardStackViewNew>();
            cardStackNew = c.GetComponent<CardStackNew>();
            cardStackViewNew.Offset = -0.3f;

            cardStackViewNew.StartPosition = temp;
            columns.Add(c);
        }
    }
// --- Drag and Drop functions
    public List<GameObject> onMouseDownObjectsToMove(GameObject gcClicked, Vector3 mousePosition, CardStackViewNew cardStackViewOfCardBeingMoved, int sizeOfColumnBeingMoved)
    {
        List<GameObject> tempList = new List<GameObject>();
        Debug.Log("gcClicked.GetComponent<CardModelNew>().StackIn.tag="+ gcClicked.GetComponent<CardModelNew>().StackIn.tag);
        if (gcClicked.GetComponent<CardModelNew>().StackIn.tag == "Deck")
        {
            Debug.Log("clicked card in deck.");
            //draw cards from deck (up to card drawn amount option) to drawnpilerow
            DrawCardsFromDeckToColumns();

            Debug.Log("templist.count="+tempList.Count);
            return tempList; // empty, dont do anything.

        }
        if (cardStackViewOfCardBeingMoved.tag == "Column")
        {
            //if card is face down, can't be selected so return nothing
            if(gcClicked.GetComponent<CardModelNew>().faceUp == false){
                Debug.Log("card is face down, not valid");
                return tempList; //should be empty;
            }
            //Debug.Log("is a column");


            //check if card to pile end is valid (descending rank and alternating card colour)
            if (isCardToColumnEndInValidOrder(gcClicked))
            {
                cardsToMoveAsColumn = getCardsFromSelectedToEndOfColumn(gcClicked);
                Debug.Log("card till end is valid");
                return cardsToMoveAsColumn;

            }
            else
            {
                Debug.Log("NOT card till end is valid");
            }
            return tempList;
        }
        tempList.Add(gcClicked);
        return tempList;
    }

    public void onMouseUpGameLogic(List<GameObject> gcDraggedList, Vector3 mousePosition, CardStackViewNew cardStackViewOfCardBeingMoved, int sizeOfColumnBeingMoved)
    {
        Debug.Log("onMouseUp");
        if (gcDraggedList.Count > 0)
        {
            GameObject selectedCardDragged = gcDraggedList[0]; // first card in list is the base card of column being dragged.
            //hit test to see which column or row its over.
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            //LayerMask layerMask;
            //layerMask = 1 << LayerMask.NameToLayer ("CardStack"); // only check for collisions with cards
            //hit = raycastFirstHitWithLayerMask(ray, layerMask);
            //Debug.Log("CardStackLayerNumber:"+LayerMask.GetMask("CardStack"));
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("CardStack")))
            {
                Debug.Log("stackname:" + hit.collider.gameObject.name);
                Debug.Log("stacktag:" + hit.collider.gameObject.tag);
                GameObject cardStackHit = hit.collider.gameObject;

                CardStackViewNew cardStackHitCardStackView = cardStackHit.GetComponent<CardStackViewNew>();
                CardStackNew cardStackHitCardStack = cardStackHit.GetComponent<CardStackNew>();

                CardModelNew cardModelNew = selectedCardDragged.GetComponent<CardModelNew>();
                //Debug ("card suit="+getCardSuitFromIndex( cm.cardIndex) + ", card rank=" + getCardRankFromIndex( cm.cardIndex));
                GameObject cardStackIn = cardModelNew.StackIn;
                CardStackViewNew cardStackInCardStackViewNew = cardStackIn.GetComponent<CardStackViewNew>();
                CardStackNew cardStackInCardStackNew = cardStackIn.GetComponent<CardStackNew>();
                //Debug.Log("getmouseup.name="+cmzoneIn);
                //FreeCellGameController freeCellGC = GetComponent<FreeCellGameController> ();
                if (checkIfValidDrop(cardStackHit, selectedCardDragged, sizeOfColumnBeingMoved))
                {
                    Debug.Log("Valid Drop and gcdraggedlist.count=" + gcDraggedList.Count);
                    //valid so update all cards in column being dragged.
                    for (int i = 0; i < gcDraggedList.Count; i++)
                    {
                        cardModelNew = gcDraggedList[i].GetComponent<CardModelNew>();
                        //cardStackInCardStackViewNew.removeCardFromFetchedWithIndex(cm.cardIndex);
                        cardStackInCardStackNew.Cards.Remove(gcDraggedList[i]);
                        //cm.zoneIn = cardStackHit; //updating to cards new zone
                        cardStackHitCardStack.Push(gcDraggedList[i]);
                    }

                    CheckAndFlipLastCardOfColumnCardStack(cardStackInCardStackNew);

                    //update cardStack that Card is going into, the hit stack
                    if (cardStackHit.tag == "PileStackRow")
                    {
                        updatePileStackView(cardStackHit);
                    }
                    else if (cardStackHit.tag == "DrawnPileColumn")
                    {
                        //DrawnPileColumUpdateView();
                    }
                    else
                    {
                        cardStackHitCardStackView.UpdateStackView();
                    }

                    //update cardStack that Card coming from, last zone card was in.
                    if (cardStackIn.tag == "PileStackRow")
                    {
                        updatePileStackView(cardStackIn);
                    }
                    else if (cardStackIn.tag == "DrawnPileColumn")
                    {
                        //DrawnPileColumUpdateView();
                    }
                    else
                    {
                        cardStackInCardStackViewNew.UpdateStackView();
                    }

 
                    

                    //check win condition after every valid action.

                    bool didWinGame = checkWinGame();
                    Debug.Log("win=" + didWinGame);
                    if (didWinGame)
                    {
                        feedBackText.text = "YOU WON!!!   Play Again?";
                    }

                }
                else
                {
                    //snap back to card stack if not valid
                    if (cardStackIn.tag == "PileStackRow")
                    {
                        updatePileStackView(cardStackIn);
                    }
                    else if (cardStackIn.tag == "DrawnPileColumn")
                    {
                        //DrawnPileColumUpdateView();
                    }
                    else
                    {
                        cardStackInCardStackViewNew.UpdateStackView();
                    }
                }
            }
            else
            {
                //didn't drop onto stack, so snap back
                if (cardStackViewOfCardBeingMoved.gameObject.tag == "PileStackRow")
                {
                    updatePileStackView(cardStackViewOfCardBeingMoved.gameObject);
                }
                else if (cardStackViewOfCardBeingMoved.gameObject.tag == "DrawnPileColumn")
                {
                    //DrawnPileColumUpdateView();
                }
                else
                {
                    cardStackViewOfCardBeingMoved.UpdateStackView();
                }
            }
        }
    }
// --- END Drag and Drop functions

    void DrawCardsFromDeckToColumns(){
        CardStackNew cardStackNew;
        CardStackViewNew cardStackViewNew;
        for (int i = 0; i < columns.Count; i++){
            cardStackNew = columns[i].GetComponent<CardStackNew>();
            GameObject card = Deck.GetComponent<CardStackNew>().Pop();
            card.GetComponent<CardViewNew>().toggleFace(true);
            cardStackNew.GetComponent<CardStackNew>().Push(card);
            cardStackViewNew = columns[i].GetComponent<CardStackViewNew>();
            cardStackViewNew.UpdateStackView();

        }
        //Deck.GetComponent<CardStackViewNew>().UpdateStackView();
        DeckUpdateView();
    }
    // Use this for initialization
    void Start () {
        //ResetDeckButton.interactable = false;
        //CardDrawnText.text = "Cards Drawn: " + CardDrawAmount;
        boardSetup();
        gameSetup();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateDeckWithDeckID(CardStackNew cardStackNew, int deckID){
        GameObject card = null;
        for (int i = 0; i < 52; i++)
        {
            //clone card
            card = (GameObject)Instantiate(Card);

            //init card
            card.GetComponent<CardModelNew>().Index = i;
            card.GetComponent<CardModelNew>().DeckID = deckID;
            card.GetComponent<CardViewNew>().toggleFace(true);

            //add card
            cardStackNew.Push(card);
            //Debug.Log (i);
        }
    }

    void CreateTwoDecks(GameObject deckStack)
    {
        //GameObject card = null;
        CardStackNew cardStackNew = deckStack.GetComponent<CardStackNew>();
        //add unshuffled cards to list (deck)
        //for (int i = 0; i < 52; i++)
        //{
        //    //clone card
        //    card = (GameObject)Instantiate(Card);

        //    //init card
        //    card.GetComponent<CardModelNew>().Index = i;
        //    card.GetComponent<CardModelNew>().DeckID = 1;
        //    card.GetComponent<CardViewNew>().toggleFace(true);

        //    //add card
        //    cardStackNew.Push(card);
        //    //Debug.Log (i);
        //}
        CreateDeckWithDeckID(cardStackNew, 1);
        CreateDeckWithDeckID(cardStackNew, 2);

        //shuffle cards
        int n = cardStackNew.Cards.Count;        //number of cards in deck
                                                 //Debug.Log("n="+n);
        int k;                      // random value
        GameObject temp;                   // holds int for swapping
        while (n > 1)
        {
            n--;
            k = UnityEngine.Random.Range(0, n + 1);
            temp = cardStackNew.Cards[k];
            cardStackNew.Cards[k] = cardStackNew.Cards[n];
            cardStackNew.Cards[n] = temp;
        }


    }

    //for spider solitaire, will show the deck with in piles of 8 cards.
    void DeckUpdateView(){
        GameObject card = null;

        CardStackNew cardStackNew = Deck.GetComponent<CardStackNew>();
        CardStackViewNew cardStackViewNew = Deck.GetComponent<CardStackViewNew>();
        Debug.Log("DeckUpdateView cardStackNew.Cards.Count=" + cardStackNew.Cards.Count);
        float offset = 0.0f;

        for (int i = 0; i < cardStackNew.Cards.Count/10; i++){
            //put 10 cards in same spot 
            Debug.Log("offset=" + offset);
            for (int j = 0; j < 10; j++){
                card = cardStackNew.Cards[i*10+j];

                UpdateCardWithOffset(card, cardStackNew, cardStackViewNew, offset, i*10+j);
            }

            //set interactive last, most top one.
            if (card)
            {
                if (card.GetComponent<BoxCollider>() != null)
                {
                    card.GetComponent<BoxCollider>().enabled = true;
                }
            }
            //increment to next pile of 10 cards 
            offset += 1.0f;
        }

    }

    void UpdateCardWithOffset(GameObject card, CardStackNew cardStackNew, CardStackViewNew cardStackViewNew, float offset, int index){
        CardViewNew cardViewNew;
        CardModelNew cardModelNew;
        SpriteRenderer spriteRenderer;
        Vector3 temp;
        //float cardOffsetToApply;
        cardViewNew = card.GetComponent<CardViewNew>();
        cardModelNew = card.GetComponent<CardModelNew>();
        if (card.GetComponent<BoxCollider>() != null)
        {
            if (cardStackViewNew.IsLastCardClickableOnly)
            {
                card.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                card.GetComponent<BoxCollider>().enabled = true;
            }
        }
        spriteRenderer = card.GetComponent<SpriteRenderer>();
        //Vector3 temp = c.transform.position;
        //cardOffsetToApply = 0.5f * i;

        temp = cardStackViewNew.StartPosition + cardStackViewNew.OffsetPositionWithDirection(offset);

        card.transform.position = temp;

        if (cardStackViewNew.ReverseLayerOrder)
        {
            spriteRenderer.sortingOrder = cardStackNew.Cards.Count - index; //right to left put down first to last.
        }
        else
        {
            spriteRenderer.sortingOrder = index; //left to right put down first to last.
        }
        cardViewNew.UpdateSprite();
    }

    private List<GameObject> getCardsFromSelectedToEndOfColumn(GameObject card)
    {
        CardStackViewNew columnCardStackInCardStackViewNew = card.GetComponent<CardModelNew>().StackIn.GetComponent<CardStackViewNew>();
        CardStackNew columnCardStackInCardStackNew = card.GetComponent<CardModelNew>().StackIn.GetComponent<CardStackNew>();
        //go through fetched list to find where selected card is then do check from there
        bool foundSelectedCard = false;
        int selectedCardIndex = card.GetComponent<CardModelNew>().Index;
        List<GameObject> temp = new List<GameObject>();

        foreach (GameObject c in columnCardStackInCardStackNew.Cards)
        {

            //find selected card
            if (!foundSelectedCard)
            {
                if (c.GetComponent<CardModelNew>().Index == selectedCardIndex)
                {
                    foundSelectedCard = true;
                    temp.Add(c);
                }
            }
            else
            { // start checking for valid column to move all card down from selected card
                temp.Add(c);
            }


        }



        return temp;
    }

    //check if card to pile end is valid to move as a column (descending rank and alternating card colour)
    public bool isCardToColumnEndInValidOrder(GameObject card)
    {
        CardStackViewNew columnCardStackInCardStackViewNew = card.GetComponent<CardModelNew>().StackIn.GetComponent<CardStackViewNew>();
        CardStackNew columnCardStackInCardStackNew = card.GetComponent<CardModelNew>().StackIn.GetComponent<CardStackNew>();
        //go through fetched list to find where selected card is then do check from there
        bool foundSelectedCard = false;
        int previousCardIndex = -1;
        int selectedCardIndex = card.GetComponent<CardModelNew>().Index;
        foreach (GameObject c in columnCardStackInCardStackNew.Cards)
        {

            //find selected card
            if (!foundSelectedCard)
            {
                if (c.GetComponent<CardModelNew>().Index == selectedCardIndex)
                {
                    foundSelectedCard = true;
                }
            }
            else
            { // start checking for valid column to move all card down from selected card
                if (!isCardValidToBeOnTopOfAnotherCardInColumnToMoveAsColumn(previousCardIndex, c.GetComponent<CardModelNew>().Index))
                {
                    return false;
                }
            }
            previousCardIndex = c.GetComponent<CardModelNew>().Index;

        }
        return true;
    }


    bool isCardValidToBeOnTopOfAnotherCardInColumn(int previousCardIndex, int currentCardIndex)
    {
        //if current card rank is one less than previous card and suit of current card is opposite colour to previous card,
        int currentCardRank = getCardRankFromIndex(currentCardIndex);
        //int currentCardSuit = getCardSuitFromIndex(currentCardIndex);
        int previousCardRank = getCardRankFromIndex(previousCardIndex);
        //int previousCardSuit = getCardSuitFromIndex(previousCardIndex);
        if (currentCardRank == previousCardRank - 1)
        { // card needs to be one less than zone card
          //then valid, add card to column.
            return true;
        }
        return false;
    }

    bool isCardValidToBeOnTopOfAnotherCardInColumnToMoveAsColumn(int previousCardIndex, int currentCardIndex)
    {
        //if current card rank is one less than previous card and suit of current card is opposite colour to previous card,
        int currentCardRank = getCardRankFromIndex(currentCardIndex);
        int currentCardSuit = getCardSuitFromIndex(currentCardIndex);
        int previousCardRank = getCardRankFromIndex(previousCardIndex);
        int previousCardSuit = getCardSuitFromIndex(previousCardIndex);
        if (currentCardSuit == previousCardSuit)
        {
            if (currentCardRank == previousCardRank - 1)
            { // card needs to be one less than zone card
              //then valid, add card to column.
                return true;
            }
        }
        return false;
    }

    public void printCardSuitRankFromIndex(int cardIndex)
    {

        Debug.Log("Card Index=" + cardIndex + ", Card Suit=" + getCardSuitFromIndex(cardIndex) + ", Card Rank=" + getCardRankFromIndex(cardIndex));

    }
    //0 - hearts
    //1 - diamonds
    //2 - clubs
    //3 - spades
    public int getCardSuitFromIndex(int cardIndex)
    {
        return cardIndex / 13; // due to interger division, decimals will get trunicated resulting in 0-3 as return.
    }

    public int getCardRankFromIndex(int cardIndex)
    {
        return cardIndex % 13;

    }

    //zone is the column or row where card was dropped on.
    //c is card that was dropped.
    public bool checkIfValidDrop(GameObject zone, GameObject c, int sizeOfColumnBeingMoved)
    {
        //Debug.Log ("checkIfValidDrop in");
        //Debug.Log ("zone.tag ="+ zone.tag);


        //get rank and suit of card
        int cardIndex = c.GetComponent<CardModelNew>().Index;
        int cardRank = getCardRankFromIndex(cardIndex);
        int cardSuit = getCardSuitFromIndex(cardIndex);

        //get rank and suit of last card in zone.
        CardStackViewNew lastZoneCardStackView = zone.GetComponent<CardStackViewNew>();
        CardStackNew lastZoneCardStackNew = zone.GetComponent<CardStackNew>();
        int zoneLastCardIndex = -1;
        int zoneLastCardRank = -1;
        int zoneLastCardSuit = -1;
        if (lastZoneCardStackNew.Cards.Count > 0)
        {
            zoneLastCardIndex = lastZoneCardStackNew.Cards[lastZoneCardStackNew.Cards.Count - 1].GetComponent<CardModelNew>().Index;
            zoneLastCardRank = getCardRankFromIndex(zoneLastCardIndex);
            zoneLastCardSuit = getCardSuitFromIndex(zoneLastCardIndex);
        }
        //if zone is column
        if (zone.tag == "Column")
        {
            Debug.Log("dorpped on Column");
            //            Debug.Log ("cardSuit="+cardSuit);
            //            Debug.Log ("cardRank="+cardRank);
            //            Debug.Log ("zoneLastCardSuit="+zoneLastCardSuit);
            //            Debug.Log ("zoneLastCardRank="+zoneLastCardRank);


            // if no cards on column, allow any card
            if (zoneLastCardIndex == -1)
            {
                Debug.Log("is empty");

            }

            if(isCardValidToBeOnTopOfAnotherCardInColumn(zoneLastCardIndex, cardIndex)){
                return true;
            }

        }

        //if zone is pilestackrow
        if (zone.tag == "PileStackRow")
        {
            //Debug.Log ("dorpped on PileStackRow");

            int[] piles = suitPileStackVisibleView(lastZoneCardStackNew);
            //if card is 1 rank higher than card in its suit pile. (no cards means pile needs an ace)
            // ace case (lowest card): since empty is -1, +1 = 0 therefore ace handled
            // king case (highest card): since only one deck and only one copy of each card, there shouldn't be anything higher so should be handled.
            if (piles[cardSuit] + 1 == cardRank)
            {
                return true;
                //lastZoneCardStackView.addCardToFetchedWithIndexAndCard (cardIndex, c);
            }
            //update pile view
            //updatePileStackView(lastZoneCardStackView);
            //return true;
        }

        return false;
    }

    //return an array of four ints representing the the suit piles with highest rank card on the top of the pile (hearts=0, diamonds=1, clubs=2, spades=3)
    int[] suitPileStackVisibleView(CardStackNew cardStackNew)
    {
        int[] piles = new int[4];
        for (int i = 0; i < 4; i++)
        {
            piles[i] = -1; //-1 means there is nothing in the corresponding suit pile
        }
        foreach (GameObject card in cardStackNew.Cards)
        {
            int cardIndex = card.GetComponent<CardModelNew>().Index;
            int cardSuit = getCardSuitFromIndex(cardIndex);
            int cardRank = getCardRankFromIndex(cardIndex);
            //looking for highest rank card for each suit.
            if (piles[cardSuit] < cardRank)
            {
                piles[cardSuit] = cardRank;
            }
        }

        return piles;
    }


    //custom update function for specific implementation pilestackview (a cardstack/cardstackview object)
    void updatePileStackView(GameObject pileStack)
    {
        int i = 0;
        //GameObject card = null;
        SpriteRenderer spriteRenderer;
        Vector3 temp;
        float co;
        CardStackNew pileStackCardStackNew = PileStackRow.GetComponent<CardStackNew>();
        CardStackViewNew pileStackCardStackViewNew = PileStackRow.GetComponent<CardStackViewNew>();
        // stores the entries at the top of each suit pile (hearts=0, diamonds=1, clubs=2, spades=3)
        //List<KeyValuePair<int, CardView>> suitPile = new List<KeyValuePair<int, CardView>>();
        List<GameObject> suitPiles = new List<GameObject>();
        for (int j = 0; j < 4; j++)
        {
            suitPiles.Add(null);
        }
        //        suitPile.Add(null);
        //        suitPile.Add(null);
        //        suitPile.Add(null);

        int cardIndex;
        int cardSuit;
        int cardRank;
        int suitPileRank;
        foreach (GameObject card in pileStackCardStackNew.Cards)
        {

            card.GetComponent<BoxCollider>().enabled = false; // set all cards to disabled, will set top of piles back after
            cardIndex = card.GetComponent<CardModelNew>().Index;
            cardSuit = getCardSuitFromIndex(cardIndex);
            cardRank = getCardRankFromIndex(cardIndex);
            //Debug.Log("value of suitpile[cardindex]="+suitPiles [cardindex]);
            //Debug.Log("entryCardSuit="+cardSuit+", entryCardRank="+cardRank);
            if (suitPiles[cardSuit] == null)
            { // empty
                suitPiles[cardSuit] = card;
            }
            else
            {
                suitPileRank = getCardRankFromIndex(suitPiles[cardSuit].GetComponent<CardModelNew>().Index);
                //Debug.Log("suitCardRank="+suitPileRank);
                //if card of same suit is higher than previous recorded, update suitpile column with current card entry.
                if (suitPileRank < cardRank)
                {
                    suitPiles[cardSuit] = card;
                }
            }





            spriteRenderer = card.GetComponent<SpriteRenderer>();
            //Vector3 temp = c.transform.position;
            co = -1.6f * cardSuit;


            temp = pileStackCardStackViewNew.StartPosition + pileStackCardStackViewNew.OffsetPositionWithDirection(co);

            card.transform.position = temp;
            spriteRenderer.sortingOrder = cardRank;

            i++;
        }


        //set cardstack object collider size to encompass all cards.
        //TODO:see if this dynamicism is still needed?

        //set pile top cards to enabled.
        for (int j = 0; j < 4; j++)
        {

            if (suitPiles[j] != null)
            {
                Debug.Log("j=" + j + ", rank=" + getCardRankFromIndex(suitPiles[j].GetComponent<CardModelNew>().Index));
                suitPiles[j].GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                Debug.Log("j=" + j + " is empty.");
            }
        }
    }

    //if column card was moved check if last card is face down, and if so flip it face up.
    void CheckAndFlipLastCardOfColumnCardStack(CardStackNew cardStackInCardStackNew)
    {
        if (cardStackInCardStackNew.Cards.Count > 0 && cardStackInCardStackNew.Cards[cardStackInCardStackNew.Cards.Count - 1].GetComponent<CardModelNew>().faceUp == false)
        {
            cardStackInCardStackNew.Cards[cardStackInCardStackNew.Cards.Count - 1].GetComponent<CardViewNew>().toggleFace(true);
        }
    }


    public bool checkWinGame()
    {
        //check if deck is empty
        if (Deck.GetComponent<CardStackNew>().Cards.Count > 0) {
            //check if all cards are in foundation
            if(PileStackRow.GetComponent<CardStackNew>().Cards.Count == 104){
                return true;
            }
        }
        

    }

}
