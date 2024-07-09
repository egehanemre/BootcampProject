using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardIndex;
    public bool hasBeenPlayed;
    public int handIndex;

    private GameManager _gameManager;
    private SpriteRenderer spriteRenderer;

    // Sorting order variables
    public int baseSortingOrder = 0; // Default sorting order
    public int hoverSortingOrder = 100; // Sorting order when hovered

    // Hover effect variables
    public float minHoverAmplitude = 0.09f; // Minimum hover amplitude
    public float maxHoverAmplitude = 0.13f; // Maximum hover amplitude
    public float hoverSpeed = 1f; // Speed of hover animation
    public float hoverScaleFactor = 1.2f; // Scale factor when hovered
    public float hoverScaleSpeed = 2f; // Speed of scale change
    public float hoverForwardOffset = 0.8f; // Forward offset when hovered

    private Vector3 initialScale;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isHovering = false;
    private float hoverAmplitude;

    public Enemy targetEnemy;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Randomize hover amplitude
        hoverAmplitude = Random.Range(minHoverAmplitude, maxHoverAmplitude);

        // Store initial scale and position
        initialScale = transform.localScale;
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Initialize with base sorting order
        spriteRenderer.sortingOrder = baseSortingOrder;
    }

    private void Update()
    {
        // Hover effect logic
        float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        Vector3 targetPosition = originalPosition + Vector3.up * hoverOffset;

        if (isHovering)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition + Vector3.up * hoverForwardOffset, Time.deltaTime * hoverSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale * hoverScaleFactor, Time.deltaTime * hoverScaleSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * hoverSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, Time.deltaTime * hoverScaleSpeed);
        }
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

    private void OnMouseEnter()
    {
        // Bring the card to the front when hovered
        spriteRenderer.sortingOrder = hoverSortingOrder;
        isHovering = true;
    }

    private void OnMouseExit()
    {
        // Return the card to its original order and reset hover state
        spriteRenderer.sortingOrder = baseSortingOrder;
        isHovering = false;
        ResetTransform();
    }

    public void SelectCard()
    {
        // Start hovering and bring the card to the front when selected
        isHovering = true;
        spriteRenderer.sortingOrder = hoverSortingOrder;
    }

    public void DeselectCard()
    {
        // Stop hovering and reset transform
        isHovering = false;
        spriteRenderer.sortingOrder = baseSortingOrder;
        ResetTransform();
    }

    private void ResetTransform()
    {
        // Reset position, scale, and rotation to original state
        transform.position = originalPosition;
        transform.localScale = initialScale;
        transform.rotation = originalRotation;
    }

    private void MoveToDiscard()
    {
        _gameManager.discardPile.Add(this);
        gameObject.SetActive(false);
    }
}
