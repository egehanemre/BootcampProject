using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardSelection : MonoBehaviour
{
    public GameObject rewardPanel;
    public List<Card> rewardCards; // Your potential reward cards
    public List<Image> cardImages; // Your UI Images for the cards
    public GameObject deckGameObject; // Your Deck GameObject
    public GameManager gameManager; // Reference to GameManager

    public void ShowRewardSelection()
    {
        rewardPanel.SetActive(true);
        // Assuming you have 3 cardImages and 3 rewardCards
        for (int i = 0; i < rewardCards.Count; i++)
        {
            cardImages[i].sprite = rewardCards[i].spriteRenderer.sprite; // Assuming Card has a Sprite property for its image
                                                              // You might also want to set up any other visual elements here
        }
    }
    public void OnCardSelected(int index)
    {
        Card selectedCard = rewardCards[index];
        GameObject cardInstance = Instantiate(selectedCard.gameObject, deckGameObject.transform);
        gameManager.deck.Add(cardInstance.GetComponent<Card>());

        // Optionally, add some gold to the player's total here

        rewardPanel.SetActive(false); // Hide the reward panel after selection
    }
}
