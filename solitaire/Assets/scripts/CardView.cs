using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView {
	public GameObject card { get; private set;}
	public bool faceUp { get; set;}

	public CardView (GameObject c){
		card = c;
		faceUp = false;
	}
}
