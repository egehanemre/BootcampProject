using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();

    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI discardPileText;

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

    public void Shuffle()
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

    private void Update()
    {
        deckSizeText.text = "Deck: " + deck.Count;
        discardPileText.text = "Discard: " + discardPile.Count;
    }
}
