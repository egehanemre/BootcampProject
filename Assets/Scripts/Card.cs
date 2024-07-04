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
            _gameManager.bulletIndex = cardIndex;
            bool bulletAdded = _gameManager.AddBullet();

            if (bulletAdded)
            {
                hasBeenPlayed = true;
                _gameManager.availableCardSlots[handIndex] = true;
                Invoke("MoveToDiscard", 0.2f);
            }
            else
            {
                Debug.Log("Failed to add bullet. Card will not be played.");
            }
        }
    }

    void MoveToDiscard()
    {
        _gameManager.discardPile.Add(this);
        gameObject.SetActive(false);
    }
}
