﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardStackNew : MonoBehaviour
{
    public List<GameObject> Cards;

    public void Push(GameObject c){
        c.GetComponent<CardModelNew>().StackIn = gameObject;
        Cards.Add(c);
    }

    public GameObject Pop(){
        GameObject temp = null;
        if (Cards.Count > 0)
        {
            temp = Cards[Cards.Count-1]; 
            Cards.RemoveAt(Cards.Count - 1);
        }
        return temp;
    }

    //public GameObject RemoveCardWithCardIndex(int index){

    //}

    public void Reset()
    {
        foreach (GameObject c in Cards){
            Destroy(c);
        }
        Cards.Clear();

    }

    void Awake()
    {
        Cards = new List<GameObject>();
    }

}
