using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject bulletPrefab;
    private GameManager _gameManager;
    public SpriteRenderer spriteRenderer;

    public int cardIndex;
    public int handIndex;
    public string cardName;
    public CardType cardType;
    public Element element;
    public Rarity rarity;
    public bool isRewardSceneCard = false;
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

    public int baseSortingOrder = 0;

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
        if (!isRewardSceneCard)
        {
            PlayCard();
        }
        else if (isRewardSceneCard)
        {
            isRewardSceneCard = false;
            transform.SetParent(_gameManager.cardsContainer.transform);
            _gameManager.deck.Add(this);

            foreach (Transform child in _gameManager.rewardsContainer.transform)
            {
                    Destroy(child.gameObject);
            }

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
