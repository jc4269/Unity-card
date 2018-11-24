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
    public void Undo()
    {
        GetComponent<CommandManager>().UndoCommand();
        //then update views
        //update view after putting cards in.
        for (int i = 0; i < columns.Count; i++)
        {
            columns[i].GetComponent<CardStackViewNew>().UpdateStackView();
        }
        DeckUpdateView();
        updatePileStackView();
    }

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
            Debug.Log("cardindex="+getCardRankFromIndex(gcClicked.GetComponent<CardModelNew>().Index));


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
                    //valid so update all cards in column being dragged using command action.
                    //setup command action
                    CommandMoveCardsAndFlipCard commandMoveCardsAndFlipCard = new CommandMoveCardsAndFlipCard(gcDraggedList, cardStackIn, cardStackHit);
                    //execute
                    GetComponent<CommandManager>().ExecuteCommand(commandMoveCardsAndFlipCard);
                    //valid so update all cards in column being dragged.
                    //for (int i = 0; i < gcDraggedList.Count; i++)
                    //{
                    //    cardModelNew = gcDraggedList[i].GetComponent<CardModelNew>();
                    //    //cardStackInCardStackViewNew.removeCardFromFetchedWithIndex(cm.cardIndex);
                    //    cardStackInCardStackNew.Cards.Remove(gcDraggedList[i]);
                    //    //cm.zoneIn = cardStackHit; //updating to cards new zone
                    //    cardStackHitCardStack.Push(gcDraggedList[i]);
                    //}

                    //CheckAndFlipLastCardOfColumnCardStack(cardStackInCardStackNew);

                    //update cardStack that Card is going into, the hit stack
                    if (cardStackHit.tag == "PileStackRow")
                    {
                        updatePileStackView();
                    }
                    else
                    {
                        cardStackHitCardStackView.UpdateStackView();
                    }

                    //update cardStack that Card coming from, last zone card was in.
                    if (cardStackIn.tag == "PileStackRow")
                    {
                        updatePileStackView();
                    }

                    else
                    {
                        cardStackInCardStackViewNew.UpdateStackView();
                    }



                    KingToAceCheckAndMove();
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
                        updatePileStackView();
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
                    updatePileStackView();
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
        //setup command action
        CommandDrawOneCardToXStacks commandDrawOneCardToXStacks = new CommandDrawOneCardToXStacks(Deck, columns);
        //execute
        GetComponent<CommandManager>().ExecuteCommand(commandDrawOneCardToXStacks);
        //CardStackNew cardStackNew;
        //CardStackViewNew cardStackViewNew;
        //for (int i = 0; i < columns.Count; i++){
        //    cardStackNew = columns[i].GetComponent<CardStackNew>();
        //    GameObject card = Deck.GetComponent<CardStackNew>().Pop();
        //    card.GetComponent<CardViewNew>().toggleFace(true);
        //    card.GetComponent<BoxCollider>().enabled = true;
        //    cardStackNew.GetComponent<CardStackNew>().Push(card);
        //    cardStackViewNew = columns[i].GetComponent<CardStackViewNew>();
        //    cardStackViewNew.UpdateStackView();

        //}
        for (int i = 0; i < columns.Count; i++)
        {
            columns[i].GetComponent<CardStackViewNew>().UpdateStackView();
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
            // Debug.Log("offset=" + offset);
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
            offset -= 1.5f;
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
        int selectedCardDeckID = card.GetComponent<CardModelNew>().DeckID;
        List<GameObject> temp = new List<GameObject>();

        foreach (GameObject c in columnCardStackInCardStackNew.Cards)
        {

            //find selected card
            if (!foundSelectedCard)
            {
                if (c.GetComponent<CardModelNew>().Index == selectedCardIndex && c.GetComponent<CardModelNew>().DeckID == selectedCardDeckID)
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
        int selectedCardDeckID = card.GetComponent<CardModelNew>().DeckID;
        foreach (GameObject c in columnCardStackInCardStackNew.Cards)
        {

            //find selected card
            if (!foundSelectedCard)
            {   

                if (c.GetComponent<CardModelNew>().Index == selectedCardIndex && c.GetComponent<CardModelNew>().DeckID == selectedCardDeckID)
                {
                    Debug.Log("c.GetComponent<CardModelNew>().Index="+c.GetComponent<CardModelNew>().Index + ", selectedCardIndex=" + selectedCardIndex);
                    Debug.Log("c.GetComponent<CardModelNew>().DeckID=" + c.GetComponent<CardModelNew>().DeckID + ", selectedCardDeckID=" + selectedCardDeckID);

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
                return true;
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

    void KingToAceCheckAndMove(){
        for (int i = 0; i < columns.Count; i++){
            if(hasColumnKingToAce(columns[i])){
                moveKingToAceInColumnToPileStack(columns[i]);
                //update views
                columns[i].GetComponent<CardStackViewNew>().UpdateStackView();
                updatePileStackView();
            }

        }
    }

    //since nothing can go ontop of ace, king to ace since will always be the last in column, so go from top till 13 cards taken
    private void moveKingToAceInColumnToPileStack(GameObject column)
    {
        CardStackNew columnCardStackNew = column.GetComponent<CardStackNew>();
        CardStackNew pileStackCardStackNew = PileStackRow.GetComponent<CardStackNew>();
        GameObject card = null;
        for (int i = 0; i < 13; i++){
            pileStackCardStackNew.Push(columnCardStackNew.Pop());
        }
    }

    //in therory, first first KingToAce sequence and returns true if it does.
    //in practice can't have king on ace so first full sequence found will only be one per column
    private bool hasColumnKingToAce(GameObject column)
    {
        CardStackNew cardStackNew = column.GetComponent<CardStackNew>();
        GameObject card = null;
        GameObject lastCard = null;
        int sequenceCount = 0;

        for (int i = 0; i < cardStackNew.Cards.Count; i++)
        {


            //column needs at least 13 cards left to start checking for sequence
            if (cardStackNew.Cards.Count - (i + 1) - 13 < 0)
            {
                break;
            }
            //if card not face up, then can't start checking.
            if (!card.GetComponent<CardModelNew>().faceUp){
                continue;
            }


            card = cardStackNew.Cards[i];

            //so if sequence started by being greater than one, check for next valid card in sequence, i.e same suit, rank -1
            if (sequenceCount > 1)
            {
                sequenceCount++;
                if (!isCardValidToBeOnTopOfAnotherCardInColumnToMoveAsColumn(lastCard.GetComponent<CardModelNew>().Index, card.GetComponent<CardModelNew>().Index))
                {
                    sequenceCount = 0;

                }
                
            }
            if(sequenceCount == 13){
                return true;
            }
            //if card == king
            if (getCardRankFromIndex(card.GetComponent<CardModelNew>().Index) == 12) // king
            {
                sequenceCount = 1;
            }

        }


        return false;
    }



    //custom update function for specific implementation pilestackview (a cardstack/cardstackview object)
    void updatePileStackView()
    {
        //the way cards are added to pile stack is only when have a full sequeunce and its in order. so just display columns of 13 cards with top card only visible.

        GameObject card = null;

        CardStackNew cardStackNew = PileStackRow.GetComponent<CardStackNew>();
        CardStackViewNew cardStackViewNew = PileStackRow.GetComponent<CardStackViewNew>();

        float offset = 0.0f;
        int index;
        for (int i = 0; i < cardStackNew.Cards.Count / 8; i++)
        {
            //put 10 cards in same spot 
            Debug.Log("offset=" + offset);
            for (int j = 0; j < 13; j++)
            {
                index = i * 8 + j;
                card = cardStackNew.Cards[index];

                UpdateCardWithOffset(card, cardStackNew, cardStackViewNew, offset, index);
            }

            //set interactive last, most top one.
            if (card)
            {
                if (card.GetComponent<BoxCollider>() != null)
                {
                    card.GetComponent<BoxCollider>().enabled = false;
                }
            }
            //increment to next pile of 13 cards 
            offset += 1.0f;
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
        return false;

    }

}
