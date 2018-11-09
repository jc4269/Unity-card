using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour {
    public GameObject GameControllerObject; // this script will have access to common functions for game controller component. must set.

    private IGameController GameControllerComponent;

	public GameObject gameobjectToDrag;
    public List<GameObject> gameObjectsToDrag;
	//public GameObject maincamera;
	//Camera mcCamera;
    
	public Vector3 GOCenter;
	public Vector3 touchPosition;
	public Vector3 offset;

    Vector3 onMouseDownMousePos;
    Vector3 currentMousePos;

    Vector3 newGOCenter;

	RaycastHit hit;

	public bool draggingMode = false;
    bool draggingModeStarted = false; // this is so don't run init dragging code again because without this the init check would be mouse pos not equaling each other, so this bool will handle that rare but possible case/bug.
    bool cardSelectedByClick = false;

    public Color cardSelectedColor = new Color(125/255.0f,125 / 255.0f, 125 / 255.0f, 1/1.0f);
    public Color cardUnselectedColor = new Color(255 / 255.0f, 255 / 255.0f, 255 / 255.0f, 1 / 1.0f);

    CardStackViewNew cardStackViewOfCardBeingMoved;

	// Use this for initialization
	void Start () {
        GameControllerComponent = GameControllerObject.GetComponent<IGameController>();

        //mcCamera = maincamera.GetComponent<Camera> ();
    }


	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButtonDown (0)) {
            if (cardSelectedByClick)
            {
                //ignore getting new cards as next mouse up is our trigger for "drop"
            }
            else
            {
                //Debug.Log("cardNotSelected");
                //nothing selected, get card
                onMouseDownMousePos = Input.mousePosition;
                cardSelectedByClick = false;
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                if (RaycastWithOrder(out hit, ray, Mathf.Infinity, LayerMask.GetMask("Card")))
                {
                    //Debug.Log("at least 1 object");
                    gameobjectToDrag = hit.collider.gameObject;
                    //FreeCellGameController freeCellGC = GetComponent<FreeCellGameController>();
                    //Debug.Log("Card clicked");
                    cardStackViewOfCardBeingMoved = gameobjectToDrag.GetComponent<CardModelNew>().StackIn.GetComponent<CardStackViewNew>();
                    gameObjectsToDrag = GameControllerComponent.onMouseDownObjectsToMove(gameobjectToDrag, Input.mousePosition, cardStackViewOfCardBeingMoved, gameObjectsToDrag.Count);
                    if (gameObjectsToDrag.Count > 0){
                        //Debug.Log("cardStackViewOfCardBeingMoved=" + cardStackViewOfCardBeingMoved);
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

                    }

                }
            }
            //Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            ////Debug.Log ("Input.mousePosition="+Input.mousePosition);
            ////RaycastHit[] rch = Physics.RaycastAll (ray);
            ////Debug.Log("Length="+rch.Length);
            ////LayerMask layerMask;
            ////layerMask = 1 << LayerMask.NameToLayer ("Card"); // only check for collisions with cards
            ////layerMask = ~(1 << LayerMask.NameToLayer ("layerX")); // ignore collisions with layerX
            ////layerMask = ~(1 << LayerMask.NameToLayer ("layerX") | 1 << LayerMask.NameToLayer ("layerY")); // ignore both layerX and layerY
            ////RaycastHit hitTest;

            //if (RaycastWithOrder(out hit, ray, Mathf.Infinity, LayerMask.GetMask("Card")))
            //{
            //    gameobjectToDrag = hit.collider.gameObject;
            //    FreeCellGameController freeCellGC = GetComponent<FreeCellGameController>();
            //    //Debug.Log("Card clicked");
            //    cardStackViewOfCardBeingMoved = gameobjectToDrag.GetComponent<CardModel>().zoneIn.GetComponent<CardStackView>();
            //    gameObjectsToDrag = freeCellGC.onMouseDownObjectsToMove(gameobjectToDrag, Input.mousePosition, cardStackViewOfCardBeingMoved);
            //    if (gameObjectsToDrag.Count > 0){
            //        Debug.Log("cardStackViewOfCardBeingMoved=" + cardStackViewOfCardBeingMoved);
            //        //Debug.Log("printout of column");
            //        //foreach (KeyValuePair<int, CardView> entry in cardStackViewOfCardBeingMoved.fetchedCards){
            //        //    FCGC.printCardSuitRankFromIndex(entry.Key);
            //        //    SpriteRenderer myr = entry.Value.card.GetComponent<SpriteRenderer>();
            //        //    Debug.Log("sortingOrder = " + myr.sortingOrder);
            //        //}
            //        GOCenter = gameobjectToDrag.transform.position;
            //        touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //        offset = touchPosition - GOCenter;
            //        //int cardIndex = gameobjectToDrag.GetComponent<CardModel>().cardIndex;
            //        //SpriteRenderer myRenderer = hit.transform.GetComponent<SpriteRenderer>();
            //        //Debug.Log("CardSingleGrab");
            //        //FCGC.printCardSuitRankFromIndex(cardIndex);
            //        //Debug.Log("sortingOrder = " + myRenderer.sortingOrder);
            //        draggingMode = true;
            //        gameobjectToDrag = gameObjectsToDrag[0];
            //        for (int i = 0; i < gameObjectsToDrag.Count; i++)
            //        {
            //            SpriteRenderer sr = gameObjectsToDrag[i].GetComponent<SpriteRenderer>();
            //            sr.sortingOrder = 100 + i;
            //        }
            //    }

            //}


        }
		if (Input.GetMouseButton (0)) {


            if (!draggingModeStarted && !cardSelectedByClick && gameObjectsToDrag.Count > 0)
            {

                currentMousePos = Input.mousePosition;
                if (onMouseDownMousePos != currentMousePos)
                {
                    //init dragging stuff
                    draggingMode = true;
                    draggingModeStarted = true;
                    gameobjectToDrag = gameObjectsToDrag[0];
                    for (int i = 0; i < gameObjectsToDrag.Count; i++)
                    {
                        SpriteRenderer sr = gameObjectsToDrag[i].GetComponent<SpriteRenderer>();
                        sr.sortingOrder = 100 + i;
                    }
                }
            }
            if(draggingMode){
                touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newGOCenter = touchPosition - offset;

                for (int i = 0; i < gameObjectsToDrag.Count; i++)
                {
                    gameObjectsToDrag[i].transform.position = new Vector3(newGOCenter.x, newGOCenter.y - 0.3f*i, GOCenter.z);
                }
            }
            //if (draggingMode) {

            //touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //newGOCenter = touchPosition - offset;

            //    for (int i = 0; i < gameObjectsToDrag.Count; i++)
            //    {
            //        gameObjectsToDrag[i].transform.position = new Vector3(newGOCenter.x, newGOCenter.y - 0.3f*i, GOCenter.z);
            //    }

            //}
        }
		if (Input.GetMouseButtonUp (0)) {
            currentMousePos = Input.mousePosition;
            //Debug.Log("mouse up");
            if (draggingMode || cardSelectedByClick){
                //Debug.Log("was dragging or selected, doing drop code");
                // drag and click selected drop code is same on mouse up.
                draggingMode = false;
                draggingModeStarted = false;
                cardSelectedByClick = false;
                //FreeCellGameController freeCellGC = GetComponent<FreeCellGameController> ();
                GameControllerComponent.onMouseUpGameLogic (gameObjectsToDrag, Input.mousePosition, cardStackViewOfCardBeingMoved, gameObjectsToDrag.Count);
                changeCardsSpriteColour(gameObjectsToDrag, cardUnselectedColor);

                //clear variables
                gameobjectToDrag = null;
                cardStackViewOfCardBeingMoved = null;
                gameObjectsToDrag.Clear();

            }
            if (!draggingMode && !cardSelectedByClick && gameObjectsToDrag.Count > 0) // nothing confirmed selected or dragged and also something to potentially select
            {
                //Debug.Log("not dragging not selected and at least one object clicked");
                //check if selecting card by click (mouse pos not moved)
                if (onMouseDownMousePos == currentMousePos)
                {
                    Debug.Log("mouse is same");
                    //card is selected
                    cardSelectedByClick = true;
                    draggingMode = false;
                    draggingModeStarted = false;

                    changeCardsSpriteColour(gameObjectsToDrag, cardSelectedColor);
                }
            }
            //draggingMode = false;
            //FreeCellGameController freeCellGC = GetComponent<FreeCellGameController> ();
            //         freeCellGC.onMouseUpGameLogic (gameObjectsToDrag, Input.mousePosition, cardStackViewOfCardBeingMoved);
            //gameobjectToDrag = null;
            //cardStackViewOfCardBeingMoved = null;
            //gameObjectsToDrag.Clear();

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
        Debug.Log("hits.Length=" + hits.Length);
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
                    //Debug.Log("no renderer");
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

    private void changeCardsSpriteColour(List<GameObject> gameObjectList, Color changeToColor){
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            gameObjectsToDrag[i].GetComponent<SpriteRenderer>().color = changeToColor;
        }
    }
}
