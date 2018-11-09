using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//used to group game controller common functions (and variables if needed)
//first use of this is DragAndDrop Script to give control to a game controller to implement some logic.
//second use: ...
public interface IGameController {
    // -s-- DRAG AND DROP functions
    //called by drag and drop script when objects have been placed somewhere either by dragging and letting go or clicking somewhere after selecting.
    //use this to run game logic and update objects into new positions or snap them back or something inbetween ... etc
    void onMouseUpGameLogic(List<GameObject> gcDraggedList, Vector3 mousePosition, CardStackViewNew cardStackViewOfCardBeingMoved, int sizeOfColumnBeingMoved);
    //{
    //    //EMPTY
    //}

    //called by drag and drop script when haven't got anything selected.
    //return a list of objects that are now selected based on logic. If return empty, then it will not count anything as selected and still ready to call this function again on next mouse down.
    List<GameObject> onMouseDownObjectsToMove(GameObject gcClicked, Vector3 mousePosition, CardStackViewNew cardStackViewOfCardBeingMoved, int sizeOfColumnBeingMoved);
    //{
    //    //EMPTY RETURN
    //    return null;
    //}
// --- END DRAG AND DROP functions
}
