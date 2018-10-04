using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlipper : MonoBehaviour 
{
	SpriteRenderer spriteRenderer;
	CardModel cardModel;

	public AnimationCurve scaleCurve;
	public float duration = 0.5f;

	void Awake(){
		spriteRenderer = GetComponent<SpriteRenderer>();
		cardModel = GetComponent<CardModel>();
	}

	public void flipCard(Sprite startImage, Sprite endImage, int cardIndex){
		StopCoroutine (flip (startImage, endImage, cardIndex));
		StartCoroutine (flip (startImage, endImage, cardIndex));
	}

	IEnumerator flip(Sprite startImage, Sprite endImage, int cardIndex){
		spriteRenderer.sprite = startImage;
		float time = 0f;
		while (time <= 1f) {
			//get scale for current time
			float scale = scaleCurve.Evaluate (time);

			//update time based on how much time has passed since last execution and duration of animation.
			time = time + Time.deltaTime / duration;

			//update card scale along the x axis
			Vector3 localScale = transform.localScale;
			localScale.x = scale;
			transform.localScale = localScale;

			//update displayed card image half way through as card is now "flipped"
			if (time >= 0.5f) {
				spriteRenderer.sprite = endImage;
			}

			yield return new WaitForFixedUpdate ();
		}

		//once animation is done, need to update the card model
		if (cardIndex == -1) { // no face cards, just back
			//cardModel.cardIndex = 0;
			cardModel.toggleFace (false);
		} else {
			cardModel.cardIndex = cardIndex;
			cardModel.toggleFace (true);
		}
	}
}
