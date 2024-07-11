using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardIndex;
    public int handIndex;

    private GameManager _gameManager;
    private SpriteRenderer spriteRenderer;

    // Sorting order variables
    public int baseSortingOrder = 0; // Default sorting order
    public int hoverSortingOrder = 100; // Sorting order when hovered

    private Vector2 initialScale;
    private Vector2 originalPosition;
    private Quaternion originalRotation;

    public bool played = false;
    public bool playedThisTurn = false;

    public Enemy targetEnemy;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();


        // Randomize hover amplitude
        //hoverAmplitude = Random.Range(minHoverAmplitude, maxHoverAmplitude);

        // Store initial scale and position
        initialScale = transform.localScale;
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Initialize with base sorting order
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
        played = true;
        playedThisTurn = true;

        _gameManager.hand.Remove(this);

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
        _gameManager.discardPile.Add(this);
        transform.position = _gameManager.discardPileTransform.position;
        gameObject.SetActive(false);
        ResetCardState();
    }

    public void ResetCardState()
    {
        // Reset all relevant properties and transforms for the card
        played = false;
        handIndex = -1; // Reset hand index to an invalid state
        baseSortingOrder = 0;
    }
}
