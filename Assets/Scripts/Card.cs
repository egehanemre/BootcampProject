using UnityEngine;

public class Card : MonoBehaviour
{
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

    public int baseSortingOrder = 0;

    public Enemy targetEnemy;
    public enum CardType 
    {
        Bullet,
        Spell,
        Augment 
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
        if (!isRewardSceneCard)
        {
            PlayCard();
        }
        //check if this card is shown for reward
        else if (isRewardSceneCard)
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
}
