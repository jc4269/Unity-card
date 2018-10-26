using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//this is for keyboard or gamepad to be used on panel parent of some buttons
//buttons in unity editor have a navagation setting in a default compontent and that has them connected already based on hierarchy i guess
//	does require a already selected object
public class SelectOnInput : MonoBehaviour {

	public EventSystem eventSystem;
	public GameObject selectedObject; // public to set default selected in editor

	private bool buttonSelected;
	// Use this for initialization
	void Start () {
		buttonSelected = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxisRaw ("Vertical") != 0 && buttonSelected == false) {//any movement up/down	
			eventSystem.SetSelectedGameObject(selectedObject) ; // i
			buttonSelected = true;
		}
	}

	private void OnDisable(){
		buttonSelected = false;
	}
}
