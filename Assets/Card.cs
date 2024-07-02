using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardIndex;

    public bool hasBeenPlayed;
    public int handIndex;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    private void OnMouseDown()
    {
        if (!hasBeenPlayed)
        {
            //destory card
            hasBeenPlayed = true;
            _gameManager.availableCardSlots[handIndex] = true;

            _gameManager.bulletIndex = cardIndex;
            _gameManager.AddBullet();
            Invoke("MoveToDiscard", 0.2f);
        }
    }

    void MoveToDiscard()
    {
        _gameManager.discardPile.Add(this);
        gameObject.SetActive(false);
    }
}
