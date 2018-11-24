using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CardModelNew))]
public class CardViewNew : MonoBehaviour
{
    private CardModelNew _CardModelNew { get; set; }
    private SpriteRenderer _SpriteRenderer {get; set;}

    public Sprite[] faces;
    public Sprite cardBack;


    public void toggleFace(bool showFace)
    {
        _CardModelNew.faceUp = showFace;
        UpdateSprite();

    }

    public void UpdateSprite(){
        if (_CardModelNew.faceUp)
        {

            _SpriteRenderer.sprite = faces[_CardModelNew.Index];
        }
        else
        {
            _SpriteRenderer.sprite = cardBack;
        }
    }

    void Awake()
    {
        _CardModelNew = GetComponent<CardModelNew>();
        _SpriteRenderer = GetComponent<SpriteRenderer>();
    }
}
