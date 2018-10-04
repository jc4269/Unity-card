using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugChangeCard : MonoBehaviour 
{
	CardFlipper cardFlipper;

	CardModel cardModel;
	int cardIndex = 0;

	public GameObject card;


	void Awake () {
		cardModel = card.GetComponent<CardModel>();
		cardFlipper = card.GetComponent<CardFlipper>();
	}
	
	void OnGUI(){

		if(GUI.Button(new Rect(10,10,100,20),"Hit me!")){
			Debug.Log ("button hit");
			if (cardIndex >= cardModel.faces.Length) {
				cardIndex = 0;
				cardFlipper.flipCard (cardModel.faces[cardModel.faces.Length-1], cardModel.cardBack, -1);
			} else {
				if (cardIndex > 0) {
					cardFlipper.flipCard (cardModel.faces[cardIndex - 1], cardModel.faces[cardIndex], cardIndex);
				} else {
					Debug.Log ("cardIndex: " + cardIndex);
					cardFlipper.flipCard (cardModel.cardBack, cardModel.faces[cardIndex], cardIndex);
					Debug.Log (cardModel.cardBack);
				}
				cardIndex++;
			}



		}

	}
}
