using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas startCanvas;
    public Canvas mapCanvas;
    public Canvas transparentPanel; 

    public bool isAllEnemiesDefeated = false;

    [SerializeField] private Generator _generator;
    [SerializeField] private Showcaser _showcaser;
    public static GameManager Instance { get; private set; }

    public GameObject battleDisplay;

    public GameObject enemiesContainer; // Assign in the inspector
    public NodeData currentNodeData;

    public bool isCardsFirstTime = true;

    public List<Card> deck = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<Card> discardPile = new List<Card>();
    public List<Card> rewardCards = new List<Card>();

    public List<GameObject> bulletObjects = new List<GameObject>();

    public List<Transform> enemiesSpawnSlots = new List<Transform>();
    public List<Enemy> enemies = new List<Enemy>();

    public Transform[] cardSlots;
    public Transform[] rewardSlots;
    public Image[] bulletSlots;

    public bool[] availableCardSlots;
    public bool[] availableRewardSlots;
    public bool[] availableBulletSlots;

    public Queue<Bullet> BulletQueue = new Queue<Bullet>();
    public Queue<Enemy> TargetEnemyQueue = new Queue<Enemy>();

    public GameObject cylinder;
    public Button fireButton;

    public Image healthBar;
    public float healthAmount = 100f;

    public Transform discardPileTransform;

    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI discardPileText;
    public TextMeshProUGUI displayTurn;

    public int maxHealth = 100;
    public int currentHealth;
    public TextMeshProUGUI healthText;

    public GameObject bulletToAdd;
    public GameObject closedBulletSlot;

    public int bulletIndex;

    public Bullet firedBullet;
    public Bullet bullet;

    public float rotationSpeed = 120f;
    public bool isRotating = false;
    public Quaternion targetRotation;
    public float rotationTime = 0.5f;

    public int arrayIndex = 0;
    public int shootIndex = 0;
    public string firedName;
    public GameObject firedBulletObject;

    public Enemy selectedEnemy;
    public GameObject selectedEnemyContainerImage;
    public GameObject rewardsContainer;
    public GameObject rewardsMenu;
    public GameObject cardsContainer;

    public static bool startComplete = false;

    public int maxMana = 6;
    public int currentMana = 3;

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
    }

    public void SetVisualsToMapSelect()
    {
        rewardsMenu.SetActive(false);
        mainCanvas.enabled = false;
        battleDisplay.SetActive(false);
        startCanvas.enabled = true;
        mapCanvas.enabled = true;
        transparentPanel.enabled = false;
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
            mainCanvas.enabled = false;
            battleDisplay.SetActive(false);
            startCanvas.enabled = false;
            mapCanvas.enabled = false;
            rewardsMenu.SetActive(true);
    }

    public void Continue()
    {
        SetVisualsToMapSelect();
        _showcaser.ToggleMapForGameScene();
    }

    public void SetCurrentNodeData(NodeData nodeData)
    {
        currentNodeData = nodeData;
        SpawnEnemiesFromNodeData();
    }


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

    private void Start()
    {
        _generator.ShowMapOnStart();
    }

    public void StartGameAfterMapClick()
    {
        StartCoroutine(StartGameAfterDelay(1.5f));
    }

    public void setupStartMap()
    {
        mainCanvas.enabled = false;
        battleDisplay.SetActive(false);
        startCanvas.enabled = true;
        mapCanvas.enabled = true;
        _generator.ShowMapOnStart();
    }

    void Update()
    {
        UpdateDeckCount();

        if (startComplete)
        {
            StartGameAfterMapClick();
            startComplete = false;
        }

        if(isAllEnemiesDefeated == true)
        {
            ToggleRewardSelection();
            isAllEnemiesDefeated = false;
        }

    }
    #region Initialization
    private void InitializeGame()
    {
        currentHealth = maxHealth;
        healthText.text = $"{healthAmount} / {maxHealth}";

        SetVisualsToBattleScene();

        InitializeRewards();
        InitializeDeck();
        SetStartSlots();
        SummonEnemies();
        EnemySelection(selectedEnemy);
        DrawHand();
        displayTurn.text = "Player's Turn";
    }

    private void InitializeRewards()
    {
        Card[] cardPrefabs = Resources.LoadAll<Card>("Cards");
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
    public void SetStartSlots()
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
        DrawHand();
        displayTurn.text = "Player's Turn";
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
            displayTurn.text = "Enemy's Turn";

            if (selectedEnemy != null)
            {
                selectedEnemy.CreateExplosion();
                selectedEnemy.UpdateDebuffDisplays();
                Invoke("StartEnemyTurn", 2f);
                Invoke("StartTurn", 4f);
            }
            else
            {
                Debug.Log("AllEnemiesAreDead");
                isAllEnemiesDefeated = true;
                ToggleRewardSelection();
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
        EnemyAction();
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
            foreach (Enemy enemy in enemies)
            {
                if (enemies != null)
                {
                enemy.UpdateEffects();
                enemy.DoAction();
                }
            }
        
    }
   
    public void UpdateDeckCount()
    {
        deckSizeText.text = "" + deck.Count;
        discardPileText.text = "" + discardPile.Count;
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

                TargetEnemyQueue.Enqueue(selectedEnemy);

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
        if (BulletQueue.Count > 0 && selectedEnemy != null)
        {
            firedBullet = BulletQueue.Dequeue();
            selectedEnemy = TargetEnemyQueue.Peek();

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

                    TargetEnemyQueue.Dequeue();

                    break;
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
            case "Pink":
                selectedEnemy.EnemyTakeDamage(10);
                break;
            case "Red":
                selectedEnemy.EnemyTakeDamage(25);

                break;
            case "Yellow":
                selectedEnemy.EnemyTakeDamage(10);
                break;
            case "Green":
                selectedEnemy.EnemyTakeDamage(5);

                break;
            case "Blue":
                selectedEnemy.EnemyTakeDamage(5);

                break;
            case "Gray":
                selectedEnemy.EnemyTakeDamage(1);

                break;
            case "Black":
                selectedEnemy.AddThunder(2);
                selectedEnemy.UpdateDebuffDisplays();


                break;
            case "VioletteDark":
                selectedEnemy.AddHellfire(2);
                selectedEnemy.UpdateDebuffDisplays();


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

            EnemySelection(enemy);
        }
    }



    public void EnemySelection(Enemy enemy)
    {
        selectedEnemy = enemy;
        if(selectedEnemy != null)
        {
            selectedEnemyContainerImage.transform.position = enemy.transform.position;
            selectedEnemyContainerImage.SetActive(true);
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
        // Optionally disable user interactions here (e.g., disable buttons, ignore clicks)
        transparentPanel.enabled = true;
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Re-enable user interactions if they were disabled
        transparentPanel.enabled = false;

        // Continue with showing the map change animation and initializing the game
        _showcaser.ToggleMapForGameScene();
        InitializeGame();
    }
}