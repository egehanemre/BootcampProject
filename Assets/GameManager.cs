using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();
    public List<GameObject> bulletObjects = new List<GameObject>();

    public Transform[] cardSlots;
    public Transform[] bulletSlots;

    public bool[] availableCardSlots;
    public bool[] availableBulletSlots;

    public GameObject bulletToAdd;
    public int bulletIndex;

    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI discardPileText;

    public Queue<GameObject> bulletQueue = new Queue<GameObject>();


    public void DrawCards()
    {
        if (deck.Count >= 1)
        {
            Card randCard = deck[Random.Range(0, deck.Count)];

            for (int i = 0; i < availableCardSlots.Length; i++)
            {
                if (availableCardSlots[i] == true)
                {
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;

                    randCard.transform.position = cardSlots[i].position;
                    randCard.hasBeenPlayed = false;

                    availableCardSlots[i] = false;  
               
                    deck.Remove(randCard);
                    return;
                }
            }
        }
    }

    public void ShuffleCards()
    {
        if (discardPile.Count >= 1)
        {
            foreach (Card card in discardPile)
            {
                deck.Add(card);
            }
            discardPile.Clear();
        }
    }

    public void UpdateDeckCount()
    {
        deckSizeText.text = "Deck: " + deck.Count;
        discardPileText.text = "Discard: " + discardPile.Count;
    }

    public void AddBullet()
    {
        for (int i = 0; i < availableBulletSlots.Length; i++)
        {
            if (availableBulletSlots[i] == true)
            {
                bulletToAdd = bulletObjects[bulletIndex];
                bulletToAdd.SetActive(true);
                bulletToAdd.transform.position = bulletSlots[i].position;
                availableBulletSlots[i] = false;

                bulletQueue.Enqueue(bulletToAdd);

                return;
            }
        }
    }

    public void FireBullet()
    {
        if (availableBulletSlots[0] != true)
        {
            bulletQueue.Dequeue().SetActive(false);
        }
        
    }
    void Update()
    {
        UpdateDeckCount();
    }
}
