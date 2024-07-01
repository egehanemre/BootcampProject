using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
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
            //change the bullet color on the mag
            hasBeenPlayed = true;
            _gameManager.availableCardSlots[handIndex] = true;
            Invoke("MoveToDiscard", 0.2f);
        }
    }

    void MoveToDiscard()
    {
        _gameManager.discardPile.Add(this);
        gameObject.SetActive(false);
    }
}
