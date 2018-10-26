using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackJackGameController : MonoBehaviour 
{
	int dealersFirstCard = -1;
    int betAmountForCurrentHand;

	public CardStack player;
	public CardStack dealer;
	public CardStack deck;

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
		player.push(deck.pop());
		//Debug.Log ("hand value = "+player.handValue ());
		if (player.handValue () > 21) {
			//TODO: player bust
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
		//TODO: ends players turn with current hand value and dealer turn to reveal cards and keep hitting till handvalue is 17 or more
	}
    
	public void playAgain(){
		//disable play again button
		playAgainButton.interactable = false;
        //reset hands and deck
        resetBoard();

        deck.CreateDeck ();
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
        player.GetComponent<CardStackView>().clear();
        dealer.GetComponent<CardStackView>().clear();
        deck.GetComponent<CardStackView>().clear();
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

	void startGame(){
        //take bet amount
        betAmountForCurrentHand = betAmount;
        money -= betAmountForCurrentHand;

        updateMoneyText();
        //deal cards to player and dealer
        for (int i = 0; i<2; i++){
			player.push(deck.pop());
			HitDealer (); 
		}
	}

	void HitDealer (){
		int card = deck.pop ();
		dealer.push(card);

		if (dealersFirstCard < 0)
			dealersFirstCard = card;
		
		if (dealer.cardsCount () >= 2) {
			CardStackView view = dealer.GetComponent<CardStackView> ();
			view.Toggle (card, true);
		}
	}

	IEnumerator dealersTurn(){
		Debug.Log ("dealers turn");
		CardStackView view = dealer.GetComponent<CardStackView> ();
		view.Toggle (dealersFirstCard, true);
		view.showCards ();
		yield return new WaitForSeconds (1f);
		while (dealer.handValue () < 17) {
			HitDealer ();
			yield return new WaitForSeconds (1f);
		}

		string winStr = "You win! Play again?";
		string loseStr = "You lost. Play again?";
		string drawStr = "Draw. Play again?";
		int playerHandValue = player.handValue ();
		int dealerHandValue = dealer.handValue ();

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
}
