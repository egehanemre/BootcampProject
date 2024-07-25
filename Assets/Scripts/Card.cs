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

    public int cardIndex;
    public int handIndex;

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
    }
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
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
        if (cardName == "HardShot")
        {
            _gameManager.DrawCard();
        }

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
            isRewardSceneCard = false;
            transform.SetParent(_gameManager.cardsContainer.transform);
            _gameManager.deck.Add(this);

            foreach (Transform child in _gameManager.rewardsContainer.transform)
            {
                Destroy(child.gameObject);
            }
            _gameManager.rewardCards.Clear();
            gameObject.SetActive(false);
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

    void BuyCard()
    {
        transform.SetParent(_gameManager.cardsContainer.transform);
        _gameManager.deck.Add(this);
        _gameManager.availableCardSlots[handIndex] = true;
        isShopCard = false;
        gameObject.SetActive(false);
    }

    void PlaySpell()
    {
        _gameManager.spellName = bulletPrefab.name;
        _gameManager.UseSpellEffect();
        _gameManager.availableCardSlots[handIndex] = true;
        MoveToDiscard();
    }

    void PlayCard()
    {
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

        BuyCard();
    }
}
