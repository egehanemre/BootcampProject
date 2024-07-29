using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
    public int shopCost;
    public GameObject bulletPrefab;
    private GameManager _gameManager;
    public SpriteRenderer spriteRenderer;
    private Canvas childCanvas;
    private int originalSortingOrder;

    public int cardIndex;
    public int handIndex;

    public bool isAnimating = false;

    [SerializeField] private string cardName;
    [SerializeField] private CardType cardType;
    [SerializeField] private Element element;
    [SerializeField] private Rarity rarity;
    public bool isRewardSceneCard = false;
    public bool isShopCard = false;

    public int baseSortingOrder = 0;

    public Enemy targetEnemy;

    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private float hoverScaleFactor = 1.2f;
    [SerializeField] private float hoverYOffset = 0.5f;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public enum CardType
    {
        Bullet,
        Incantation,
    }
    public enum Element
    {
        EmptyElement,
        Hellfire,
        Soulfire,
        Thunder,
        Silver,
        Blood,
        Holy,
        Dark,
        Inferno,
        Explosion,
        HolyFlame,
        BloodFlame,
        BlackFlame,
        Plasma,
        SpiritStorm,
        QuickSilver,
        RedLightning,
        PureSilver,
        Sacrifice,
        Unholy,
        Curse,
    }
    public enum Rarity
    {
        Common,
        Rare,
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        childCanvas = GetComponentInChildren<Canvas>();
        originalScale = transform.localScale;
        originalPosition = transform.position; // Ensure original position is set in Awake
    }

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        initialScale = transform.localScale;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = baseSortingOrder;
        }

        if (childCanvas != null)
        {
            childCanvas.sortingOrder = baseSortingOrder;
        }
    }

    private void OnMouseDown()
    {
        if (isAnimating) return; // Prevent clicking if the card is animating

        // Reset position and scale before playing the card
        ResetPositionAndScale();

        if (!isRewardSceneCard)
        {
            if (cardType == CardType.Bullet)
            {
                PlayCard();
            }
            if (cardType == CardType.Incantation)
            {
                PlaySpell();
            }
        }
        //check if this card is shown for reward
        else if (isRewardSceneCard && !isShopCard)
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
            Cursor.visible = false; // Hide the cursor
            isRewardSceneCard = false;
            transform.SetParent(_gameManager.cardsContainer.transform);
            _gameManager.deck.Add(this);
            StartCoroutine(ShowPurchaseFeedback());

            Invoke("destroyOtherRewards", 0.45f);
            
        }
        else if (isRewardSceneCard && isShopCard)
        {
            if (_gameManager.coin >= shopCost)
            {
                _gameManager.coin -= shopCost;
                _gameManager.UpdateDeckCount();
                StartCoroutine(ShowPurchaseFeedback());
            }
            else
            {
                StartCoroutine(ShowInsufficientCoinsFeedback());
            }
        }
    }

    private void destroyOtherRewards()
    {
        foreach (Transform child in _gameManager.rewardsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        _gameManager.rewardCards.Clear();
        gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    private void OnMouseEnter()
    {
        if (isShopCard || isRewardSceneCard || isAnimating) return; // Skip hover effects if the card is in the shop, reward scene, or animating

        originalPosition = transform.position; // Store the original position
        originalSortingOrder = baseSortingOrder;
        baseSortingOrder = 100; // Set the sorting order to be on top

        transform.localScale = originalScale * hoverScaleFactor;
        transform.position = originalPosition + new Vector3(0, hoverYOffset, 0);
    }

    private void OnMouseExit()
    {
        if (isShopCard || isRewardSceneCard || isAnimating) return; // Skip hover effects if the card is in the shop, reward scene, or animating

        ResetPositionAndScale();
        baseSortingOrder = originalSortingOrder;
    }


    private void ResetPositionAndScale()
    {
        if (isAnimating) return;

        transform.position = originalPosition;
        transform.localScale = originalScale;
    }

    void BuyCard()
    {
        transform.SetParent(_gameManager.cardsContainer.transform);
        _gameManager.deck.Add(this);
        _gameManager.availableCardSlots[handIndex] = true;
        isShopCard = false;
        isRewardSceneCard = false;
        gameObject.SetActive(false);
    }

    void PlaySpell()
    {
        _gameManager.spellName = bulletPrefab.name;
        _gameManager.UseSpellEffect();
        _gameManager.availableCardSlots[handIndex] = true;
        if(_gameManager.spellName == "OpportunitySpell")
        {
            _gameManager.DrawSingleCard();
        }
        MoveToDiscard();
    }

    void PlayCard()
    {
        if (isAnimating) return;

        _gameManager.bulletIndex = cardIndex;

        bool bulletAdded = _gameManager.AddBullet();

        if (bulletAdded)
        {
            _gameManager.availableCardSlots[handIndex] = true;
            MoveToDiscard();
        }
        else
        {
            Debug.Log("Failed to add bullet. Card will not be played.");
        }
    }

    public void MoveToDiscard()
    {
        if (isAnimating) return; // Prevent moving to discard if the card is animating

        _gameManager.hand.Remove(this);
        _gameManager.discardPile.Add(this);
        baseSortingOrder = 200; // Set the sorting order to be on top 

        // Start the discard animation coroutine
        StartCoroutine(_gameManager.AnimateCardToDiscard(transform, _gameManager.discardPileTransform, this, 0.5f));

        // Set the card's position to the discard pile after the animation
        StartCoroutine(SetCardInactiveAfterAnimation());
    }

    private IEnumerator SetCardInactiveAfterAnimation()
    {
        yield return new WaitForSeconds(0.5f); // Wait for the animation to complete
        gameObject.SetActive(false);
        ResetCardState();
        transform.localScale = initialScale; // Reset the scale to initial value
        transform.position = initialPosition; // Reset the position to initial value
        transform.rotation = initialRotation; // Reset the rotation to initial value
    }
    public void ResetCardState()
    {
        handIndex = -1; // Reset hand index to an invalid state
        baseSortingOrder = 0;
    }

    private IEnumerator ShowInsufficientCoinsFeedback()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        Vector3 originalPosition = transform.position;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
        spriteRenderer.color = originalColor;
    }

    private IEnumerator ShowPurchaseFeedback()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.green;

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float animationDuration = 0.4f;
        float elapsed = 0.0f;

        while (elapsed < animationDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        spriteRenderer.color = originalColor;

        if(isShopCard)
        {
            BuyCard();
        }
    }
}
