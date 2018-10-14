using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModel : MonoBehaviour 
{
	SpriteRenderer spriteRenderer;

	public Sprite[] faces;
	public Sprite cardBack;
	public int cardIndex; // e.g. faces[cardIndex]

	public GameObject zoneIn { get; set; } // which column/row cardstack is it apart of. field or deck etc.

	public void toggleFace(bool showFace){
		if (showFace) {
			spriteRenderer.sprite = faces[cardIndex];
		} else {
			spriteRenderer.sprite = cardBack;
		}
	}

	void Awake(){
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
}
