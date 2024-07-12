using UnityEngine;

public class Card : MonoBehaviour
{
    private GameManager _gameManager;
    private SpriteRenderer spriteRenderer;

    public int cardIndex;
    public int handIndex;

    // Sorting order variables
    public int baseSortingOrder = 0; // Default sorting order

    public Enemy targetEnemy;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = baseSortingOrder;
    }

    private void OnMouseDown()
    {
        PlayCard();
    }
    void PlayCard()
    {
        _gameManager.bulletIndex = cardIndex;

        bool bulletAdded = _gameManager.AddBullet();

        if (bulletAdded)
        {
            _gameManager.availableCardSlots[handIndex] = true;

            Invoke("MoveToDiscard", 0.2f);
        }
        else
        {
            Debug.Log("Failed to add bullet. Card will not be played.");
        }
    }

    public void MoveToDiscard()
    {
        _gameManager.hand.Remove(this);
        _gameManager.discardPile.Add(this);
        transform.position = _gameManager.discardPileTransform.position;
        gameObject.SetActive(false);
        ResetCardState();
    }

    public void ResetCardState()
    {
        handIndex = -1; // Reset hand index to an invalid state
        baseSortingOrder = 0;
    }
}
