using System;
//using UnityEngine;

public class CardEventArgs : EventArgs
{
	public int cardIndex { get; private set;}

	public CardEventArgs (int i){
		cardIndex = i;
	}
}
