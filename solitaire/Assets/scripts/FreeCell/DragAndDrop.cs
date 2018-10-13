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

			if (Physics.Raycast (ray, out hit)) {
				gameobjectToDrag = hit.collider.gameObject;
				GOCenter = gameobjectToDrag.transform.position;
				touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				offset = touchPosition - GOCenter;
				draggingMode = true;
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
		}
	}
}
