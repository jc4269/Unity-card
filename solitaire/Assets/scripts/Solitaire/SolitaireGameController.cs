using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SolitaireGameController : MonoBehaviour, IGameController {

    public GameObject Card;
    public GameObject Deck;
    //public CardStack column;
    public GameObject Column;
    public GameObject PileStackRow;
    public GameObject DrawnPileColumn;

    public Button playAgainButton;
    public Text feedBackText;
    public Text CardDrawnText;
    public Button CardDrawnAmountButton;
    public Button ResetDeckButton;

    public GameObject RowColBackground;
    public GameObject PileRowBackground;
    public GameObject DrawnPileBackground;

    public List<GameObject> columns;
    public List<GameObject> cardsToMoveAsColumn;
    private int CardDrawAmount = 1;
// --- Button Functions
    public void ResetDeck(){
        //take all cards from DrawnPileColumn and put them in deck in the same order.
        Debug.Log("clicked RESET DECK");
        int n = DrawnPileColumn.GetComponent<CardStackNew>().Cards.Count;
        for (int i = 0; i < n; i++) {
            GameObject card = DrawnPileColumn.GetComponent<CardStackNew>().Pop();
            card.GetComponent<CardViewNew>().toggleFace(false);
            Deck.GetComponent<CardStackNew>().Push(card);
        }
        Deck.GetComponent<CardStackViewNew>().UpdateStackView();
        DrawnPileColumUpdateView();


    }

    public void CardDrawnAmountChanger(){
        if (CardDrawAmount == 1){
            CardDrawAmount = 3;
        }
        else{
            CardDrawAmount = 1;
        }
        CardDrawnText.text = "Cards Drawn: " + CardDrawAmount;
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
        resetBoard();
        gameSetup();
    }

// --- END Button Functions
    void resetBoard()
    {
        PileStackRow.GetComponent<CardStackNew>().Reset();
        DrawnPileColumn.GetComponent<CardStackNew>().Reset();
        Deck.GetComponent<CardStackNew>().Reset();
        for (int i = 0; i < columns.Count; i++)
        {
            columns[i].GetComponent<CardStackNew>().Reset();
        }
        //columns.Clear();
    }

    void gameSetup(){

        CreateDeck(Deck);
        for (int i = 0; i < Deck.GetComponent<CardStackNew>().Cards.Count; i++)
        {
            Deck.GetComponent<CardStackNew>().Cards[i].GetComponent<CardViewNew>().toggleFace(false);
        }

            CardStackNew cardStackNew;
        CardStackViewNew cardStackViewNew;
        //deal cards to each column in this format: 1,2,3,4,5,6,7.
        GameObject card = null;
        for (int i = 0; i < 7; i++)
        {
            cardStackNew = columns[i].GetComponent<CardStackNew>();
            for (int j = 0; j < i + 1; j++) // use column index as a quick way to give the correct cards out to each column
            {
                card = Deck.GetComponent<CardStackNew>().Pop();
                card.GetComponent<CardViewNew>().toggleFace(false);
                cardStackNew.Push(card);
            }
            //last card needs to be visable
            card.GetComponent<CardViewNew>().toggleFace(true);
            cardStackViewNew = columns[i].GetComponent<CardStackViewNew>();
            //cardStackViewNew.Toggle(cardStackNew.Cards.Count - 1, true);

            //update view
            cardStackViewNew.UpdateStackView();
        }

        //remaining cards stay in the deck
        //move remaining cards in deck into position
        Deck.GetComponent<CardStackViewNew>().UpdateStackView();
    }

    void boardSetup(){
        PileStackRow.transform.position = PileStackRow.GetComponent<CardStackViewNew>().StartPosition;
        DrawnPileColumn.transform.position = DrawnPileColumn.GetComponent<CardStackViewNew>().StartPosition;
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
        for (int i = 0; i < 6; i++)
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
            DrawCardsFromDeckToDrawnPile();
            Debug.Log("templist.count="+tempList.Count);
            return tempList; // empty, dont do anything.
        }
        tempList.Add(gcClicked);
        return tempList;
    }

    public void onMouseUpGameLogic(List<GameObject> gcDraggedList, Vector3 mousePosition, CardStackViewNew cardStackViewOfCardBeingMoved, int sizeOfColumnBeingMoved)
    {
        Debug.Log("onMouseUp");
    }
// --- END Drag and Drop functions

    void DrawCardsFromDeckToDrawnPile(){
        for (int i = 0; i < CardDrawAmount; i++){
            if (Deck.GetComponent<CardStackNew>().Cards.Count > 0)
            {
                GameObject card = Deck.GetComponent<CardStackNew>().Pop();
                card.GetComponent<CardViewNew>().toggleFace(true);
                DrawnPileColumn.GetComponent<CardStackNew>().Push(card);
            }
            else { // deck is empty no need to continue

                break;
            }
        }
        if(Deck.GetComponent<CardStackNew>().Cards.Count == 0){
            ResetDeckButton.interactable = true;
        }
        Deck.GetComponent<CardStackViewNew>().UpdateStackView();
        DrawnPileColumUpdateView();
    }
    // Use this for initialization
    void Start () {
        ResetDeckButton.interactable = false;
        CardDrawnText.text = "Cards Drawn: " + CardDrawAmount;
        boardSetup();
        gameSetup();
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    void CreateDeck(GameObject deckStack)
    {
        GameObject card = null;
        CardStackNew cardStackNew = deckStack.GetComponent<CardStackNew>();
        //add unshuffled cards to list (deck)
        for (int i = 0; i < 52; i++)
        {
            //clone card
            card = (GameObject)Instantiate(Card);

            //init card
            card.GetComponent<CardModelNew>().Index = i;
            card.GetComponent<CardViewNew>().toggleFace(true);

            //add card
            cardStackNew.Push(card);
            //Debug.Log (i);
        }


        //shuffle cards
        int n = cardStackNew.Cards.Count;        //number of cards in deck
                                                 //Debug.Log("n="+n);
        int k;                      // random value
        GameObject temp;                   // holds int for swapping
        while (n > 1)
        {
            n--;
            k = Random.Range(0, n + 1);
            temp = cardStackNew.Cards[k];
            cardStackNew.Cards[k] = cardStackNew.Cards[n];
            cardStackNew.Cards[n] = temp;
        }


    }

    void DrawnPileColumUpdateView(){
        //the last 3 cards get offset, the rest are in a pile behind the first card.
        //or if just 1 card drawn, stack cards.
        if (CardDrawAmount == 1)
        {
            //Debug.Log("CardDrawnAmount == 1");
            DrawnPileColumn.GetComponent<CardStackViewNew>().Offset = 0;
            DrawnPileColumn.GetComponent<CardStackViewNew>().UpdateStackView();
        }
        else{ //3 cards drawn
            //custom, put all but the last 2 cards at start position then start using offset for the last 2 cards.

            GameObject lastCard = null;
            GameObject card;

            CardStackNew cardStackNew = DrawnPileColumn.GetComponent<CardStackNew>();
            CardStackViewNew cardStackViewNew = DrawnPileColumn.GetComponent<CardStackViewNew>();
            for (int i = 0; i < cardStackNew.Cards.Count - 2; i++)
            {
                card = cardStackNew.Cards[i];
                UpdateCardWithOffset(card, cardStackNew, cardStackViewNew, 0, i);

                //lastCard = card;
            }

            //now apply offsets to last 2 cards, if 
            int start = -1;
            if (cardStackNew.Cards.Count >= 2)
            {
                start = cardStackNew.Cards.Count - 2;
            }
            if(cardStackNew.Cards.Count == 1){
                start = 0;
            }
            if (start >= 0)
            {
                float offset = 0.0f;
                for (int i = start; i < cardStackNew.Cards.Count; i++)
                {
                    card = cardStackNew.Cards[i];
                    offset += -0.5f;
                    UpdateCardWithOffset(card, cardStackNew, cardStackViewNew, offset, i);

                    lastCard = card;
                }
            }
            //set cardstack collider size to encompass all cards.
            //TODO:see if this dynamicism is still needed?

            //last object processed now gets box colider enabled. Saves using if statement in loop above for now.
            if (lastCard)
            {
                if (lastCard.GetComponent<BoxCollider>() != null)
                {
                    lastCard.GetComponent<BoxCollider>().enabled = true;
                }
            }
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
}
