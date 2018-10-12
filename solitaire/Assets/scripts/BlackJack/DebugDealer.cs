using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDealer : MonoBehaviour 
{
	public CardStack dealer;
	public CardStack player;

	void OnGUI(){
		if (GUI.Button (new Rect (10,10,256,58), "Hit me!")) {
			player.push (dealer.pop ());
		}
	}
}
