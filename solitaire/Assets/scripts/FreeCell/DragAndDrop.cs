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

	CardStackView cardStackViewOfCardBeingMoved;

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
            //LayerMask layerMask;
            //layerMask = 1 << LayerMask.NameToLayer ("Card"); // only check for collisions with cards
            //layerMask = ~(1 << LayerMask.NameToLayer ("layerX")); // ignore collisions with layerX
            //layerMask = ~(1 << LayerMask.NameToLayer ("layerX") | 1 << LayerMask.NameToLayer ("layerY")); // ignore both layerX and layerY
            //RaycastHit hitTest;

            if (RaycastWithOrder(out hit, ray, Mathf.Infinity, LayerMask.GetMask("Card")))
            {
                gameobjectToDrag = hit.collider.gameObject;
                FreeCellGameController freeCellGC = GetComponent<FreeCellGameController>();

                cardStackViewOfCardBeingMoved = gameobjectToDrag.GetComponent<CardModel>().zoneIn.GetComponent<CardStackView>();
                if(freeCellGC.onMouseDownisValidObject(gameobjectToDrag, Input.mousePosition, cardStackViewOfCardBeingMoved)){
                    Debug.Log("cardStackViewOfCardBeingMoved=" + cardStackViewOfCardBeingMoved);
                    //Debug.Log("printout of column");
                    //foreach (KeyValuePair<int, CardView> entry in cardStackViewOfCardBeingMoved.fetchedCards){
                    //    FCGC.printCardSuitRankFromIndex(entry.Key);
                    //    SpriteRenderer myr = entry.Value.card.GetComponent<SpriteRenderer>();
                    //    Debug.Log("sortingOrder = " + myr.sortingOrder);
                    //}
                    GOCenter = gameobjectToDrag.transform.position;
                    touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    offset = touchPosition - GOCenter;
                    //int cardIndex = gameobjectToDrag.GetComponent<CardModel>().cardIndex;
                    //SpriteRenderer myRenderer = hit.transform.GetComponent<SpriteRenderer>();
                    //Debug.Log("CardSingleGrab");
                    //FCGC.printCardSuitRankFromIndex(cardIndex);
                    //Debug.Log("sortingOrder = " + myRenderer.sortingOrder);
                    draggingMode = true;
                    SpriteRenderer sr = gameobjectToDrag.GetComponent<SpriteRenderer>();
                    sr.sortingOrder = 100;
                }
               
            }

            //hit = raycastFirstHitWithLayerMask(ray, layerMask);
   //         if (Physics.Raycast(ray,out hit,Mathf.Infinity,LayerMask.GetMask("Card"))) {
			//	gameobjectToDrag = hit.collider.gameObject;
			//	cardStackViewOfCardBeingMoved = gameobjectToDrag.GetComponent<CardModel> ().zoneIn.GetComponent<CardStackView>();
			//	Debug.Log ("cardStackViewOfCardBeingMoved="+cardStackViewOfCardBeingMoved);
   //             //Debug.Log("printout of column");
   //             //foreach (KeyValuePair<int, CardView> entry in cardStackViewOfCardBeingMoved.fetchedCards){
   //             //    FCGC.printCardSuitRankFromIndex(entry.Key);
   //             //    SpriteRenderer myr = entry.Value.card.GetComponent<SpriteRenderer>();
   //             //    Debug.Log("sortingOrder = " + myr.sortingOrder);
   //             //}
			//	GOCenter = gameobjectToDrag.transform.position;
			//	touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//	offset = touchPosition - GOCenter;
   //             //int cardIndex = gameobjectToDrag.GetComponent<CardModel>().cardIndex;
   //             //SpriteRenderer myRenderer = hit.transform.GetComponent<SpriteRenderer>();
   //             //Debug.Log("CardSingleGrab");
   //             //FCGC.printCardSuitRankFromIndex(cardIndex);
   //             //Debug.Log("sortingOrder = " + myRenderer.sortingOrder);
   //             draggingMode = true;
			//	SpriteRenderer sr = gameobjectToDrag.GetComponent<SpriteRenderer> ();
			//	sr.sortingOrder = 100;
			//}

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
			FreeCellGameController freeCellGC = GetComponent<FreeCellGameController> ();
			freeCellGC.onMouseUpGameLogic (gameobjectToDrag, Input.mousePosition, cardStackViewOfCardBeingMoved);
			gameobjectToDrag = null;
			cardStackViewOfCardBeingMoved = null;

		}
	}

    //output to rch first object hit that matches tag t
    //return true if rch is assigned something. else return false if nothing found.
    //	bool raycastFirstHitWithLayerMask (Ray r, LayerMask lm, out RaycastHit rch){
    //		RaycastHit[] hits = Physics.RaycastAll (r,Mathf.Infinity,lm);
    //		foreach(RaycastHit hit in hits) {
    //			if (hit.collider.gameObject.tag == t) {
    //				rch = hit;
    //			}
    //		}
    //
    //		return false;
    //	}
    // The function for dealing with multiple objects being overlapped and only returning the highest sorting order as hit.
    // TODO: if using, dont use 2D?
    public bool RaycastWithOrder(out RaycastHit rch, Ray r, float distance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers)
    {
        RaycastHit[] hits = Physics.RaycastAll(r, distance, layerMask);
        //Debug.Log("hits.Length=" + hits.Length);
        //FCGC = GetComponent<FreeCellGameController>();
        if (hits.Length > 0)
        { //Only function if we actually hit something
            int closestItem = -1; //Set our top hit to a default of the first index in our "hits" array, in case there are no others
            int lowestLayerID = int.MaxValue;
            int highestSortingOrder = int.MaxValue;
            //Debug.Log("closestItem=" + closestItem + ", lowestLayerID="+ lowestLayerID+ ", highestSortingOrder="+ highestSortingOrder);
            for (int i = 0; i < hits.Length; i++)
            { //Loop for every extra item the raycast hit
                SpriteRenderer myRenderer = hits[i].transform.GetComponent<SpriteRenderer>();
                if (myRenderer == null)
                {
                    break; // if transform has no SpriteRenderer, we leave it out
                }

                int currentLayerID = myRenderer.sortingLayerID; //Store SortingLayerID of the current item in the array being accessed
                //Debug.Log("currentLayerID=" + currentLayerID + ", lowestLayerID=" + lowestLayerID +", myRenderer.sortingOrder="+ myRenderer.sortingOrder + ", highestSortingOrder=" + highestSortingOrder);
                if (currentLayerID < lowestLayerID)
                { //If the SortingLayerID of the current array item is lower than the previous lowest
                    lowestLayerID = currentLayerID; //Set the "Previous Value" to the current one since it's lower, as it will become the "Previous Lowest" on the next loop
                    closestItem = i; //Set our topHit with the Array Index value of the current closest Array item, since it currently has the highest/closest SortingLayerID
                    highestSortingOrder = myRenderer.sortingOrder; //Store SortingOrder value of the current closest object, for comparison next loop if we end up going to the "else if"
                    //Debug.Log("closestItem=" + closestItem + ", lowestLayerID=" + lowestLayerID + ", highestSortingOrder=" + highestSortingOrder);
                }
                else if (currentLayerID == lowestLayerID)
                {
                    int MRsortingOrder = myRenderer.sortingOrder;
                    if (myRenderer.sortingOrder > highestSortingOrder)
                    { //If SortingLayerID are the same, then we need to compare SortingOrder. If the SortingOrder is lower than the one stored in the previous loop, then update values
                        closestItem = i;
                        highestSortingOrder = myRenderer.sortingOrder;
                        //Debug.Log("closestItem=" + closestItem + ", lowestLayerID=" + lowestLayerID + ", highestSortingOrder=" + highestSortingOrder);
                    }
                }
            }
            rch = hits[closestItem];
            return true;
        }
        rch = default(RaycastHit);
        return false; // means nothing output/hit, so dont use out variable.
    }
    // The function for dealing with multiple objects being overlapped and only returning the highest sorting order as hit.
    // TODO: if using, dont use 2D?
    //public bool Raycast2DWithOrder (out RaycastHit2D rch, Vector2 origin, Vector2 direction, float distance = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers) {
    //	RaycastHit2D[] hits = Physics2D.RaycastAll (origin, direction, distance, layerMask);

    //	if (hits.Length > 0) { //Only function if we actually hit something
    //		int closestItem = 0; //Set our top hit to a default of the first index in our "hits" array, in case there are no others
    //		int lowestLayerID = int.MaxValue;
    //		int highestSortingOrder = int.MaxValue;

    //		for (int i = 1; i < hits.Length; i++) { //Loop for every extra item the raycast hit
    //			SpriteRenderer myRenderer = hits [i].transform.GetComponent<SpriteRenderer> ();
    //			if (myRenderer == null) {
    //				break; // if transform has no SpriteRenderer, we leave it out
    //			}

    //			int currentLayerID = myRenderer.sortingLayerID; //Store SortingLayerID of the current item in the array being accessed

    //			if (currentLayerID < lowestLayerID) { //If the SortingLayerID of the current array item is lower than the previous lowest
    //				lowestLayerID = currentLayerID; //Set the "Previous Value" to the current one since it's lower, as it will become the "Previous Lowest" on the next loop
    //				closestItem = i; //Set our topHit with the Array Index value of the current closest Array item, since it currently has the highest/closest SortingLayerID
    //				highestSortingOrder = myRenderer.sortingOrder; //Store SortingOrder value of the current closest object, for comparison next loop if we end up going to the "else if"
    //			} else if (currentLayerID == lowestLayerID) {
    //				if (myRenderer.sortingOrder > highestSortingOrder) { //If SortingLayerID are the same, then we need to compare SortingOrder. If the SortingOrder is lower than the one stored in the previous loop, then update values
    //					closestItem = i;
    //					highestSortingOrder = myRenderer.sortingOrder;
    //				}
    //			}
    //		}

    //		rch =  hits [closestItem];
    //        return true;
    //	}
    //    rch = default(RaycastHit2D);
    //    return false;
    //}
}
