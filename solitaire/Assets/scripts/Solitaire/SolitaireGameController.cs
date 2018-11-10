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

    public GameObject RowColBackground;
    public GameObject PileRowBackground;
    public GameObject DrawnPileBackground;

    public List<GameObject> columns;
    public List<GameObject> cardsToMoveAsColumn;

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

    public List<GameObject> onMouseDownObjectsToMove(GameObject gcClicked, Vector3 mousePosition, CardStackViewNew cardStackViewOfCardBeingMoved, int sizeOfColumnBeingMoved)
    {
        List<GameObject> tempList = new List<GameObject>();
        tempList.Add(gcClicked);
        return tempList;
    }

    public void onMouseUpGameLogic(List<GameObject> gcDraggedList, Vector3 mousePosition, CardStackViewNew cardStackViewOfCardBeingMoved, int sizeOfColumnBeingMoved)
    {

    }

    // Use this for initialization
    void Start () {

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

}
