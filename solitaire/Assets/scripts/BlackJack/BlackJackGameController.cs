using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackJackGameController : MonoBehaviour 
{
	int dealersFirstCard = -1;
    int betAmountForCurrentHand;

	public GameObject Player;
	public GameObject Dealer;
    public GameObject Deck;
    public GameObject Card;
	public Button hitButton;
	public Button standButton;
	public Button playAgainButton;

	public Text feedBackText;
    public Text moneyText;
    public Text betText;
    public int money;
    public int betAmount;


    /***
	 * Deal 2 cards to each player and dealer.
	 * 	Dealers cards have first face down and rest face up.
	 * 	Players cards are all face up.
	 * Players turn is first, choosing to "HIT" and get another card or "STAND" and pass on turn with current hand value.
	 * Dealers turn must HIT if <17 in hand value.
	 * 
	 */

    #region Public methods

    public void backToMenu(int menuIndex)
    {
        //clean up memory
        resetBoard();

        //then load menu
        SceneManager.LoadScene(menuIndex);
    }

    public void hit(){
        HitPlayer();
        //Debug.Log ("hand value = "+player.handValue ());
        updateStacks();

        if (HandValue(Player.GetComponent<CardStackNew>()) > 21) {
			hitButton.interactable = false;
			standButton.interactable = false;
			StartCoroutine(dealersTurn());
		}
	}
	public void stand(){
		Debug.Log ("stand hit");
		//player turn ends, disable buttons
		hitButton.interactable = false;
		standButton.interactable = false;
		//start dealers turn
		StartCoroutine(dealersTurn());
	}
    
	public void playAgain(){
		//disable play again button
		playAgainButton.interactable = false;
        //reset hands and deck
        resetBoard();

        CreateDeck (Deck);
		//deck.GetComponent<CardStackView> ().showCards ();
		dealersFirstCard = -1;

		//enable gameplay buttons
		hitButton.interactable = true;
		standButton.interactable = true;


		//reset feedback text
		feedBackText.text = "Playing again!";

		startGame ();
	}

	#endregion

    void resetBoard(){
        Player.GetComponent<CardStackNew>().Reset();
        Dealer.GetComponent<CardStackNew>().Reset();
        Deck.GetComponent<CardStackNew>().Reset();
    }

    void updateStacks()
    {
        Player.GetComponent<CardStackViewNew>().UpdateStackView();
        Dealer.GetComponent<CardStackViewNew>().UpdateStackView();
        Deck.GetComponent<CardStackViewNew>().UpdateStackView();
    }

    #region Unity messages

    void Start(){
        //money will be persistant through games. TODO: Save value between games.
        money = 500;
        betAmount = 10;

        updateBetText();
        updateMoneyText();
        startGame ();
	}

    #endregion

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
            card.GetComponent<CardViewNew>().toggleFace(false);

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

    void startGame(){
        //take bet amount
        betAmountForCurrentHand = betAmount;
        money -= betAmountForCurrentHand;

        updateMoneyText();

        CreateDeck(Deck);

        //deal cards to player and dealer
        for (int i = 0; i<2; i++){
            HitPlayer();
            HitDealer (); 
		}
        updateStacks();

    }
    void HitPlayer(){
        GameObject card = Deck.GetComponent<CardStackNew>().Pop();
        card.GetComponent<CardViewNew>().toggleFace(true);
        Player.GetComponent<CardStackNew>().Push(card);
    }
	void HitDealer (){
        GameObject card = Deck.GetComponent<CardStackNew>().Pop();

        Dealer.GetComponent<CardStackNew>().Push(Deck.GetComponent<CardStackNew>().Pop());

        if (dealersFirstCard < 0)
        {
            dealersFirstCard = card.GetComponent<CardModelNew>().Index;
            Dealer.GetComponent<CardStackNew>().Cards[0].GetComponent<CardViewNew>().toggleFace(false);
        }
        int dealersHandSize = Dealer.GetComponent<CardStackNew>().Cards.Count;
        if (dealersHandSize >= 2) {
            Dealer.GetComponent<CardStackNew>().Cards[dealersHandSize-1].GetComponent<CardViewNew>().toggleFace(true);
		}

        updateStacks();
    }

	IEnumerator dealersTurn(){
		Debug.Log ("dealers turn");
        CardStackViewNew dealerCardStackViewNew = Dealer.GetComponent<CardStackViewNew>();
        CardStackNew dealerCardStackNew = Dealer.GetComponent<CardStackNew>();
        dealerCardStackNew.Cards[0].GetComponent<CardViewNew>().toggleFace(true);//flipp first card over
        //view.showCards ();
        yield return new WaitForSeconds (1f);
		while (HandValue(dealerCardStackNew) < 17) {
			HitDealer ();
			yield return new WaitForSeconds (1f);
		}

		string winStr = "You win! Play again?";
		string loseStr = "You lost. Play again?";
		string drawStr = "Draw. Play again?";
        int playerHandValue = HandValue (Player.GetComponent<CardStackNew>());
		int dealerHandValue = HandValue (dealerCardStackNew);

		Debug.Log ("playerHandValue = "+ playerHandValue + ", dealerHandValue = "+ dealerHandValue);
		//determine who won and give feedback
		if (playerHandValue > 21) { //player bust
            feedBackText.text = loseStr;
            //lose: money already bet. no need to do anything

        } 
		else if (dealerHandValue > 21) { //dealer bust
			feedBackText.text = winStr;
            money += 2* betAmountForCurrentHand;
        }
		else if (dealerHandValue < playerHandValue) {
			feedBackText.text = winStr;
            money += 2* betAmountForCurrentHand;
        }
		else if (dealerHandValue == playerHandValue) {
			feedBackText.text = drawStr;
            money += betAmountForCurrentHand; // draw, get back bet amount. no winnings
        }
		else if (dealerHandValue > playerHandValue) {
			feedBackText.text = loseStr;
            //lose: money already bet. no need to do anything
        }
        updateMoneyText();
        //end of game, enable play again button
        playAgainButton.interactable = true;
	}

    public void updateMoneyText(){
        moneyText.text = "Money: " + money;
    }
    public void updateBetText()
    {
        betText.text = "Bet: " + betAmount;
    }
    public void updateBetAmount(int updateAmount){
        betAmount += updateAmount;
        updateBetText();
    }

    public void resetBetAmount(){
        betAmount = 0;
        updateBetText();
    }



    //calculate hand value
    public int HandValue(CardStackNew cardStackNew)
    {
        int total = 0;
        int cardRank;
        int cardIndex;
        int aces = 0; // count the number of aces in hand, used later to do 1 or 11 logic
        foreach (GameObject card in cardStackNew.Cards)
        {
            cardIndex = card.GetComponent<CardModelNew>().Index;
            cardRank = cardIndex % 13;
            //Debug.Log ("Cardrank="+cardRank);
            if (cardRank >= 10 && cardRank <= 12)
            { // J, Q, K
                cardRank = 10; // J,Q,K worth 10 points

            }
            else if (cardRank == 0)
            { // ace
                aces++;
                continue; // dont want to add cardrank to total.
            }
            else
            { // pip value 2-10
                cardRank += 1; // needs one added to get correct value.
            }

            total += cardRank; // add hand value up.
        }

        for (int i = 0; i < aces; i++)
        {
            if (total + 11 <= 21)
            {
                total += 11;
            }
            else
            {
                total += 1;
            }
        }
        return total;
    }
}
