using System;
//using UnityEngine;

public class CardRemovedEventArgs : EventArgs
{
	public int cardIndex { get; private set;}

	public CardRemovedEventArgs (int i){
		cardIndex = i;
	}
}
