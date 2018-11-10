//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class SolitaireGameController : MonoBehaviour, IGameController {

//    public GameObject Card;
//    public CardStack deck;
//    public GameObject Deck;
//    //public CardStack column;
//    public GameObject column;
//    public GameObject pileStackRow;
//    public GameObject freeCardRow;

//    public Button playAgainButton;
//    public Text feedBackText;

//    public GameObject rowColBackground;
//    public GameObject pileRowBackground;
//    public GameObject freeCardBackground;

//    public List<GameObject> columns;
//    public List<GameObject> cardsToMoveAsColumn;

//    public void playAgain()
//    {
//        resetBoard();
//        gameSetup();
//    }


//    void resetBoard()
//    {

//    }

//    void gameSetup(){


//    }

//    void boardSetup(){
//        boardSetup();
//    }

//    public List<GameObject> onMouseDownObjectsToMove(GameObject gcClicked, Vector3 mousePosition, CardStackViewNew cardStackViewOfCardBeingMoved, int sizeOfColumnBeingMoved)
//    {
//        throw new System.NotImplementedException();
//    }

//    public void onMouseUpGameLogic(List<GameObject> gcDraggedList, Vector3 mousePosition, CardStackViewNew cardStackViewOfCardBeingMoved, int sizeOfColumnBeingMoved)
//    {
//        throw new System.NotImplementedException();
//    }

//    // Use this for initialization
//    void Start () {

//        boardSetup();
//        gameSetup();
//    }
	
//	// Update is called once per frame
//	void Update () {
		
//	}


//    void CreateDeck(GameObject deckStack)
//    {
//        GameObject card = null;
//        CardStackNew cardStackNew = deckStack.GetComponent<CardStackNew>();
//        //add unshuffled cards to list (deck)
//        for (int i = 0; i < 52; i++)
//        {
//            //clone card
//            card = (GameObject)Instantiate(Card);

//            //init card
//            card.GetComponent<CardModelNew>().Index = i;
//            card.GetComponent<CardViewNew>().toggleFace(true);

//            //add card
//            cardStackNew.Push(card);
//            //Debug.Log (i);
//        }


//        //shuffle cards
//        int n = cardStackNew.Cards.Count;        //number of cards in deck
//                                                 //Debug.Log("n="+n);
//        int k;                      // random value
//        GameObject temp;                   // holds int for swapping
//        while (n > 1)
//        {
//            n--;
//            k = Random.Range(0, n + 1);
//            temp = cardStackNew.Cards[k];
//            cardStackNew.Cards[k] = cardStackNew.Cards[n];
//            cardStackNew.Cards[n] = temp;
//        }


//    }

//}
