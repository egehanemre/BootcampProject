using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static MapMovement;

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }
    public static bool isPlayerDoneSelectingThePointToMove = false;

    public MapMovement mapMovement;
    //UI Elements
    public Canvas mainCanvas;
    public Canvas startCanvas;
    public Canvas mapCanvas;
    public Canvas transparentPanel;
    public Canvas topBar;

    public GameObject selectedEnemyContainerImage;
    public GameObject rewardsContainer;
    public GameObject shopContainer;
    public GameObject rewardsMenu;
    public GameObject cardsContainer;

    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI discardPileText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI displayTurn;
    public TextMeshProUGUI healthText;

    //Game State
    public bool isAllEnemiesDefeated = false;
    public bool isCardsFirstTime = true;

    //GameData
    public Queue<Bullet> BulletQueue = new Queue<Bullet>();
    public Queue<Enemy> TargetEnemyQueue = new Queue<Enemy>();

    public List<Card> deck = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<Card> discardPile = new List<Card>();
    public List<Card> rewardCards = new List<Card>();
    public List<Card> ShopCards = new List<Card>();
    public List<Transform> enemiesSpawnSlots = new List<Transform>();
    public List<Enemy> enemies = new List<Enemy>();
    public List<GameObject> bulletObjects = new List<GameObject>();
    public NodeData currentNodeData;
    public Enemy TargetEnemy;
    public Enemy selectedEnemy;

    public Bullet firedBullet;
    public Bullet bullet;

    //Gaem Object and Transforms
    public GameObject bulletToAdd;
    public GameObject closedBulletSlot;
    public GameObject enemiesContainer; // Assign in the inspector
    public GameObject battleDisplay;
    public GameObject firedBulletObject;
    public Transform[] cardSlots;
    public Transform[] shopSlots;
    public Transform[] rewardSlots;
    public Transform discardPileTransform;

    //Gameplay Variables
    public int arrayIndex = 0;
    public int shootIndex = 0;

    public int turnCount = 0;

    public int coin = 0;
    public int maxMana = 6;
    public int currentMana = 3;
    public int maxHealth = 100;
    public int currentHealth;
    public int bulletIndex;

    [SerializeField] private Generator _generator;
    [SerializeField] private Showcaser _showcaser;

    //Others

    public Image[] bulletSlots;
    public Image[] buffSlots;
    public bool[] availableCardSlots;
    public bool[] availableRewardSlots;
    public bool[] availableShopSlots;
    public bool[] availableBulletSlots;

    public GameObject cylinder;
    public Button fireButton;
    public Image healthBar;
    public float healthAmount = 100f;

    public string firedName;
    public string spellName;

    public GameObject shop;


    #region Unity Default Methods
    private void Start()
    {
        // Create and shot the map on start
        _generator.ShowMapOnStart();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        transparentPanel.enabled = false;


        _generator = FindObjectOfType<Generator>();
        SetVisualsToMapSelect();

        mapMovement = FindAnyObjectByType<MapMovement>();
    }

    void Update()
    {
        UpdateDeckCount();

        if (isPlayerDoneSelectingThePointToMove)
        {
            StartGameAfterMapClick();
            isPlayerDoneSelectingThePointToMove = false;
        }

        if (isAllEnemiesDefeated == true)
        {
            ToggleRewardSelection();
            isAllEnemiesDefeated = false;
        }
    }

    #endregion

    #region Visual Modifiers
    public void SetVisualsToMapSelect()
    {
        rewardsMenu.SetActive(false);
        mainCanvas.enabled = false;
        battleDisplay.SetActive(false);
        startCanvas.enabled = true;
        mapCanvas.enabled = true;
        transparentPanel.enabled = false;
        shop.SetActive(false);
    }

    public void SetVisualsToBattleScene()
    {
        rewardsMenu.SetActive(false);
        mainCanvas.enabled = true;
        battleDisplay.SetActive(true);
        startCanvas.enabled = false;
        mapCanvas.enabled = false;
    }

    public void ToggleRewardSelection()
    {
        startCanvas.enabled = false;
        mainCanvas.enabled = false;
        battleDisplay.SetActive(false);
        mapCanvas.enabled = false;
        rewardsMenu.SetActive(true);
    }

    public void ToggleShop()
    {
        mainCanvas.enabled = false;
        battleDisplay.SetActive(false);
        startCanvas.enabled = false;
        mapCanvas.enabled = false;
        rewardsMenu.SetActive(false);
        shop.SetActive(true);
    }

    public void Continue()
    {
        isAllEnemiesDefeated = false;
        SetVisualsToMapSelect();
        _showcaser.ToggleMapForGameScene();
        InitializeRewards();
        InitializeShop();
        SetStartSlotsToEmpty();
        ResetRewardSlots();
        ResetShopSlots();

        enemies.Clear();
        foreach (Transform child in enemiesContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetCurrentNodeData(NodeData nodeData)
    {
        currentNodeData = nodeData;
        SpawnEnemiesFromNodeData();
    }
    #endregion
    private void SpawnEnemiesFromNodeData()
    {
        Debug.Log("Spawning enemies from node data.");

        if (enemiesContainer == null)
        {
            Debug.LogError("Enemies container is not assigned.");
            return;
        }

        if (currentNodeData == null)
        {
            Debug.LogError("Current node data or enemy prefabs are null.");
            return;
        }
        if (currentNodeData.enemyPrefabs == null)
        {
            Debug.LogError("enemy prefabs are null.");
            return;
        }

        foreach (GameObject enemyPrefab in currentNodeData.enemyPrefabs)
        {
            GameObject enemy = Instantiate(enemyPrefab, enemiesContainer.transform);
            enemies.Add(enemy.GetComponent<Enemy>());
            Debug.Log($"Spawned {enemy.name} as child of {enemiesContainer.name}");
        }
    }
    public void StartGameAfterMapClick()
    {
        // initializes game
        StartCoroutine(StartGameAfterDelay(0.5f));
    }
    public void SetupStartMap()
    {
        mainCanvas.enabled = false;
        battleDisplay.SetActive(false);
        startCanvas.enabled = true;
        mapCanvas.enabled = true;
        _generator.ShowMapOnStart();
    }

    #region Initialization
    private void InitializeGame()
    {
        currentHealth = maxHealth;
        healthText.text = $"{healthAmount} / {maxHealth}";

        //show the battle scene
        SetVisualsToBattleScene();
        InitializeRewards();
        InitializeDeck();
        InitializeShop();
        SetStartSlotsToEmpty();
        SummonEnemies();
        EnemySelection();
        DrawHand();
        displayTurn.text = "Player's Turn:  " + turnCount + ".";
        turnCount++;
    }

    private void InitializeRewards()
    {
        //we'll load the cards from resources folder
        Card[] cardPrefabs = Resources.LoadAll<Card>("RewardCards");
        List<Card> selectedRewards = new List<Card>();

        while (selectedRewards.Count < 3)
        {
            Card potentialReward = cardPrefabs[Random.Range(0, cardPrefabs.Length)];
            if (!selectedRewards.Contains(potentialReward))
            {
                selectedRewards.Add(potentialReward);
            }
        }
        for (int i = 0; i < selectedRewards.Count; i++)
        {
            if (availableRewardSlots[i])
            {
                // Instantiate the reward card prefab and set its parent to the rewardsContainer
                GameObject rewardInstance = Instantiate(selectedRewards[i].gameObject, rewardSlots[i].position, Quaternion.identity, rewardsContainer.transform);
                rewardInstance.transform.rotation = rewardSlots[i].rotation;
                availableRewardSlots[i] = false;
                selectedRewards[i].isRewardSceneCard = true;
            }
        }
        rewardCards = selectedRewards;
    }

    private void InitializeShop()
    {
        //we'll load the cards from resources folder
        Card[] cardPrefabs = Resources.LoadAll<Card>("ShopCards");
        List<Card> selectedShops = new List<Card>();

        while (selectedShops.Count < 5)
        {
            Card potentialShop = cardPrefabs[Random.Range(0, cardPrefabs.Length)];
            if (!selectedShops.Contains(potentialShop))
            {
                selectedShops.Add(potentialShop);
            }
        }
        for (int i = 0; i < selectedShops.Count; i++)
        {
            if (availableShopSlots[i])
            {
                // Instantiate the reward card prefab and set its parent to the rewardsContainer
                GameObject ShopInstance = Instantiate(selectedShops[i].gameObject, shopSlots[i].position, Quaternion.identity, shopContainer.transform);
                ShopInstance.transform.rotation = shopSlots[i].rotation;
                availableShopSlots[i] = false;
                selectedShops[i].isRewardSceneCard = true;
            }
        }
        ShopCards = selectedShops;
    }

    private void ResetRewardSlots()
    {
       for (int i = 0; i < availableRewardSlots.Length; i++)
        {
            availableRewardSlots[i] = true;
        }
    }
    private void ResetShopSlots()
    {
        for (int i = 0; i < availableShopSlots.Length; i++)
        {
            availableShopSlots[i] = true;
        }
    }

    private void InitializeDeck()
    {
        if (isCardsFirstTime)
        {
            isCardsFirstTime = false;
            Card[] cardsInHierarchy = GameObject.Find("Cards").GetComponentsInChildren<Card>(true);
            deck.AddRange(cardsInHierarchy);
            bulletObjects.Clear();
            foreach (Card card in deck)
            {
                if (card.bulletPrefab != null && !bulletObjects.Contains(card.bulletPrefab))
                {
                    bulletObjects.Add(card.bulletPrefab);
                }
            }
        }
    }

    

    public void ResetCylinder()
    {
        arrayIndex = 0;
        shootIndex = 0;
        firedName = null;
        bulletToAdd = null;
        cylinder.transform.rotation = Quaternion.Euler(0, 0, 0);
        //make animations for this later
    }
    public void SetStartSlotsToEmpty()
    {
        availableBulletSlots = new bool[bulletSlots.Length];
        for (int i = 0; i < currentMana; i++)
        {
            availableBulletSlots[i] = true;
        }

        for (int i = currentMana; i < availableBulletSlots.Length; i++)
        {
            closedBulletSlot.SetActive(true);
            bulletSlots[i].enabled = true;
            bulletSlots[i].sprite = closedBulletSlot.GetComponent<Image>().sprite;
            bulletSlots[i].color = closedBulletSlot.GetComponent<Image>().color;
        }

        availableCardSlots = new bool[cardSlots.Length];
        for (int i = 0; i < availableCardSlots.Length; i++)
        {
            availableCardSlots[i] = true;
        }

    }
    public void IncreaseManaSlots()
    {
        currentMana++;
        availableBulletSlots[currentMana - 1] = true;

        bulletSlots[currentMana - 1].enabled = false;
        bulletSlots[currentMana - 1].sprite = null;
        bulletSlots[currentMana - 1].color = Color.clear;
    }

    #endregion
    #region Card Management

    public void DrawHand()
    {
        if ( isAllEnemiesDefeated == false)
        {
            for (int i = 0; i < 5; i++)
            {
                DrawCard();
            }
        }
        
    }
    public void DrawCard()
    {
        if (deck.Count > 0)
        {
            Card randCard = deck[Random.Range(0, deck.Count)];
            for (int i = 0; i < availableCardSlots.Length; i++)
            {
                if (availableCardSlots[i])
                {
                    hand.Add(randCard);
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;
                    randCard.baseSortingOrder = i;
                    randCard.transform.position = cardSlots[i].position;
                    randCard.transform.rotation = cardSlots[i].rotation;
                    availableCardSlots[i] = false;
                    deck.Remove(randCard);
                    break;
                }
            }
        }
    }
    public void StartTurn()
    {
        EnableAllSlots();
        if(rewardsMenu.activeSelf == false)
        {
            DrawHand();
        }
        displayTurn.text = "Player's Turn:  " + turnCount + ".";
        turnCount++;
    }

    public void EndTurn()
    { 
        DiscardHand();
        {
            if (deck.Count < 5)
            {
                ShuffleCards();
            }
            shootIndex = 0;
            ShootTheMagazine();
            displayTurn.text = "Enemy's Turn:  " + turnCount + ".";
            turnCount++;

            if (TargetEnemy != null)
            {
                TargetEnemy.CreateExplosion();
                TargetEnemy.UpdateDebuffDisplays();
                Invoke("StartEnemyTurn", 2f);
                Invoke("StartTurn", 4f);
            }
            else
            {
                Debug.Log("AllEnemiesAreDead");
                isAllEnemiesDefeated = true;
                ToggleRewardSelection();
                DiscardHand();
                //SetVisualsToMapSelect();
                //_showcaser.ToggleMapForGameScene();
            }
        }
    }
    public void DiscardHand()
    {
        foreach (Card card in hand.ToArray())
        {
            card.MoveToDiscard();
        }
        hand.Clear();
    }
    public void ShuffleCards()
    {
        if (discardPile.Count > 0)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();
        }
    }
    public void StartEnemyTurn()
    {
        if (TargetEnemy != null)
        {
            EnemyAction();
        }
    }


    public void ShootTheMagazine()
    {
        int shootAmount = BulletQueue.Count;

        for (int x = 0; x < shootAmount; x++)
        {
            FireBullet();
        }
    }
    public void EnableAllSlots()
    {
        for (int i = 0; i < availableCardSlots.Length; i++)
        {
            availableCardSlots[i] = true;
        }
    }
    public void EnemyAction()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].UpdateEffects();
        }
        for(int j = 0; j < enemies.Count; j++)
        {
            enemies[j].DoAction();
        }
    }

    public void UpdateDeckCount()
    {
        deckSizeText.text = "" + deck.Count;
        discardPileText.text = "" + discardPile.Count;
        coinText.text = "Coin: " + coin;
    }

    #endregion
    #region Bullet Managements
    public bool AddBullet()
    {
        bool bulletAdded = false;

        for (int i = arrayIndex; i < currentMana; i++)
        {
            if (availableBulletSlots[i])
            {
                bulletToAdd = bulletObjects[bulletIndex];
                bulletToAdd.SetActive(true);

                bulletSlots[i].enabled = true;
                bulletSlots[i].sprite = bulletToAdd.GetComponent<Image>().sprite;
                bulletSlots[i].color = bulletToAdd.GetComponent<Image>().color;

                BulletQueue.Enqueue(bulletToAdd.GetComponent<Bullet>());

                TargetEnemyQueue.Enqueue(TargetEnemy);

                availableBulletSlots[i] = false;
                bulletAdded = true;
                break;
            }
        }

        if (!bulletAdded)
        {
            Debug.Log("No available slots to add bullet.");
            return bulletAdded;
        }
        return bulletAdded;
    }

    public void Fire()
    {
        int bulletsToFire = BulletQueue.Count;
    }
    public void FireBullet()
    {
        if (BulletQueue.Count > 0)
        {
            foreach (Bullet bullet in BulletQueue)
            {
                Debug.Log("Firing bullet: " + bullet.name);
            }

            firedBullet = BulletQueue.Dequeue();
            TargetEnemy = TargetEnemyQueue.Dequeue();

            for (int i = shootIndex ; i < bulletSlots.Length; i++)
            {
                if (bulletSlots[i].sprite == firedBullet.GetComponent<Image>().sprite)
                {
                    bulletSlots[i].enabled = false;
                    bulletSlots[i].sprite = null;
                    bulletSlots[i].color = Color.clear;

                    availableBulletSlots[i] = true;

                    firedName = firedBullet.name;

                    UseBulletEffect();

                    break;
                }
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] != null)
                {
                    enemies[i].CheckIsDead();
                }
            }
        }
        else
        {
            Debug.Log("No bullets in queue to fire or cylinder is rotating.");
        }
    }
    public void UseBulletEffect()
    {
        switch (firedName)
        {
            case "Bullet":
                TargetEnemy.EnemyTakeDamage(5);
                TargetEnemy.UpdateDebuffDisplays();
                break;
            case "HellfireBullet":
                TargetEnemy.AddHellfire(2);
                TargetEnemy.UpdateDebuffDisplays();
                break;
            case "SpikedBullet":
                TargetEnemy.EnemyTakeDamage(4);
                TargetEnemy.AddBlood(2);
                TargetEnemy.UpdateDebuffDisplays();
                break;
            case "HardShot":
                TargetEnemy.EnemyTakeDamage(11);
                TargetEnemy.AddBlood(2);
                TargetEnemy.UpdateDebuffDisplays();
                break;
        }
    }

    public void UseSpellEffect()
    {
        switch(spellName)
        {
            case "BloodPactSpell":
                TargetEnemy.AddBlood(5);
                TargetEnemy.UpdateDebuffDisplays();
                break;
            case "HopeSpell":
                TargetEnemy.AddHoly(3);
                TargetEnemy.UpdateDebuffDisplays();
                break;
            case "OpportunitySpell":
                healthAmount += 5;
                healthBar.fillAmount = healthAmount / 100f;
                healthText.text = healthAmount + " / " + maxHealth;
                DrawCard();
                break;
        }
    }

    #endregion
    public void SummonEnemies()
    {
     
        // Shuffle the spawn slots list
        List<Transform> shuffledSpawnSlots = new List<Transform>(enemiesSpawnSlots);

        for (int i = 0; i < shuffledSpawnSlots.Count; i++)
        {
            Transform temp = shuffledSpawnSlots[i];
            int randomIndex = Random.Range(i, shuffledSpawnSlots.Count);
            shuffledSpawnSlots[i] = shuffledSpawnSlots[randomIndex];
            shuffledSpawnSlots[randomIndex] = temp;
        }

        // Assign each enemy to a unique slot
        for (int i = 0; i < enemies.Count && i < shuffledSpawnSlots.Count; i++)
        {
            Enemy enemy = enemies[i];
            Transform spawnSlot = shuffledSpawnSlots[i];
            enemy.transform.position = spawnSlot.position;
            enemy.gameObject.SetActive(true);

            EnemySelection();
        }
    }

    public void EnemySelection()
    {
        if (enemies.Count > 0)
        {
            TargetEnemy = enemies[0];
            selectedEnemyContainerImage.transform.position = TargetEnemy.transform.position;
            selectedEnemyContainerImage.SetActive(true);
        }
        else
        {
            Debug.Log("No enemies available to select.");
            TargetEnemy = null;
            selectedEnemyContainerImage.SetActive(false);
        }
    }
    public void PlayerTakeDamage(int damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
        healthText.text = healthAmount + " / " + maxHealth;
    }

    private IEnumerator StartGameAfterDelay(float delay)
    {
        transparentPanel.enabled = true;
        yield return new WaitForSeconds(delay);
        transparentPanel.enabled = false;

        if (mapMovement.currentNode.nodeData.isShop == true)
        {
            EnemySelection();
            Debug.Log("Shop Node");
            _showcaser.ToggleMapForGameScene();
            ToggleShop();
        }
        else
        {
            _showcaser.ToggleMapForGameScene();
            //Do the everything related to game initialization here

            InitializeGame();
        }
    }
}