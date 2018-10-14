using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour {
	public GameObject gameobjectToDrag;
	//public GameObject maincamera;
	//Camera mcCamera;
	public Vector3 GOCenter;
	public Vector3 touchPosition;
	public Vector3 offset;

	Vector3 newGOCenter;

	RaycastHit hit;

	public bool draggingMode = false;

	// Use this for initialization
	void Start () {
		//mcCamera = maincamera.GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			//Debug.Log ("Input.mousePosition="+Input.mousePosition);
			//RaycastHit[] rch = Physics.RaycastAll (ray);
			//Debug.Log("Length="+rch.Length);
			if (Physics.Raycast (ray, out hit)) {
				gameobjectToDrag = hit.collider.gameObject;
				GOCenter = gameobjectToDrag.transform.position;
				touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				offset = touchPosition - GOCenter;
				draggingMode = true;
				SpriteRenderer sr = gameobjectToDrag.GetComponent<SpriteRenderer> ();
				sr.sortingOrder = 100;
				//TODO: card tells column it's being removed and is in freespace. Keep column its from to snap back if no valid drop is made
			}

		}
		if (Input.GetMouseButton (0)) {
			if (draggingMode) {
				touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				newGOCenter = touchPosition - offset;
				gameobjectToDrag.transform.position = new Vector3 (newGOCenter.x, newGOCenter.y, GOCenter.z);

			}
		}
		if (Input.GetMouseButtonUp (0)) {
			draggingMode = false;
			if(gameobjectToDrag){
				//snap back to column
				CardModel cm = gameobjectToDrag.GetComponent<CardModel>();
	 			GameObject cmzoneIn = cm.zoneIn;
				//Debug.Log("getmouseup.name="+cmzoneIn);
				cmzoneIn.GetComponent<CardStackView> ().updateCardViewStackFetchedCardsVisually ();
			}

			//TODO: hit test to see which column or row its over.
			//		if column card is from, just snap it back
			//		Then pass to function to see if valid placement.
			//		If valid, add card into column/row
			//		If false, add back to column card is from.
		}
	}
}
