using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCellGameController : MonoBehaviour {

	public GameObject card;
	public CardStack deck;
	//public CardStack column;
	public GameObject column;
	public GameObject pileStackRow;
	public GameObject freeCardRow;
	public List<GameObject> columns;

	// Use this for initialization
	void Start () {
		boardSetup ();

		CardStack tempCS;
		for (int i = 0; i < 8; i++) {
			tempCS = columns [i].GetComponent<CardStack> ();
			for (int j = 0; j < 2; j++) {
				tempCS.push (deck.pop ());
			}
		}
		tempCS = freeCardRow.GetComponent<CardStack> ();
		for (int j = 0; j < 2; j++) {
			tempCS.push (deck.pop ());
		}
		tempCS = pileStackRow.GetComponent<CardStack> ();
		for (int j = 0; j < 2; j++) {
			tempCS.push (deck.pop ());
		}

	}

	void boardSetup(){
		deck.CreateDeck ();

		GameObject c;
		columns.Add (column);
		CardStackView columnCVS = column.GetComponent<CardStackView> ();
		Vector3 startPosition = columnCVS.startPosition;
		Debug.Log ("startPosition="+startPosition);
		Vector3 temp;
		float columnOffset = 1.0f;
		float cs;
		for(int i = 0; i < 7; i++){
			c = (GameObject)Instantiate (column);
			cs = columnOffset * (i+1);
			temp = startPosition + (new Vector3 (cs, 0f, 0f));
			c.transform.position = temp;
			Debug.Log ("temp="+temp);
			CardStackView csv = c.GetComponent<CardStackView> ();
			csv.cardOffset = 0.3f;
			csv.cardPrefab = card;
			csv.faceUp = true;
			csv.offsetHorizontal = false;
			csv.startPosition = temp;
			csv.reverseLayerOrder = true;
			columns.Add (c);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
