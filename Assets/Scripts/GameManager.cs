using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<Card> discardPile = new List<Card>();

    public List<GameObject> bulletObjects = new List<GameObject>();

    public List<Transform> enemiesSpawnSlots = new List<Transform>();
    public List<Enemy> enemies = new List<Enemy>();

    public Transform[] cardSlots;
    public Image[] bulletSlots;

    public bool[] availableCardSlots;
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


    public GameObject bulletToAdd;
    public GameObject closedBulletSlot;

    public int bulletIndex;

    public Bullet firedBullet;
    public Bullet bullet;

    public int maxHealth = 100;
    public int currentHealth;

    public float rotationSpeed = 120f;
    public bool isRotating = false;
    public Quaternion targetRotation;
    public float rotationTime = 0.5f;

    public int arrayIndex = 0;
    public int shootIndex = 0;
    public int firedIndex = -1;

    public Enemy selectedEnemy;
    public GameObject selectedEnemyContainerImage;

    public int maxMana = 6;
    public int currentMana = 3;

    private void Start()
    {
        currentHealth = maxHealth;

        SetStartSlots();
        SummonEnemies();
        EnemySelection(selectedEnemy);

        DrawHand();
        displayTurn.text = "Player's Turn";
    }

    void Update()
    {
        UpdateDeckCount();
    }

    #region Card Methods

    public void DrawHand()
    {
        DrawCards();
        DrawCards();
        DrawCards();
        DrawCards();
        DrawCards();
    }
    public void DrawCards()
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
    public void StartTurn()
    {
        EnableAllSlots();
        DrawHand();

        displayTurn.text = "Player's Turn";
    }
    public void StartEnemyTurn()
    {
        EnemyAttack();
    }

    public void EndTurn()
    {
        DiscardHand();

        if (deck.Count < 5)
        {
            ShuffleCards();
        }
        shootIndex = 0;

        ShootTheMagazine();

        displayTurn.text = "Enemy's Turn";
        Invoke("StartEnemyTurn", 2f);

        Invoke("StartTurn", 4f);
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
    public void EnemyAttack()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.DoAction();
        }
    }
    public void DiscardHand()
    {
        foreach (Card card in hand.ToArray())
        {
            card.MoveToDiscard();
        }
        hand.Clear(); // Clear the hand list after moving all cards to discard pile
    }
    public void ShuffleCards()
    {
        if (discardPile.Count > 0)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();
        }
    }
    public void UpdateDeckCount()
    {
        deckSizeText.text = "" + deck.Count;
        discardPileText.text = "" + discardPile.Count;
    }

    #endregion

    #region bullet Methods
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

                    firedIndex = firedBullet.bulletIndex;

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
    #endregion
    public void UseBulletEffect()
    {
        switch (firedIndex)
        {
            case 0:
                selectedEnemy.EnemyTakeDamage(10);
                break;
            case 1:
                selectedEnemy.EnemyTakeDamage(25);

                break;
            case 2:
                selectedEnemy.EnemyTakeDamage(10);
                break;
            case 3:
                selectedEnemy.EnemyTakeDamage(5);

                break;
            case 4:
                selectedEnemy.EnemyTakeDamage(5);

                break;
            case 5:
                selectedEnemy.EnemyTakeDamage(1);

                break;
            case 6:
                selectedEnemy.EnemyTakeDamage(12);

                break;
            case 7:
                selectedEnemy.EnemyTakeDamage(24);

                break;
            case 8:
                selectedEnemy.EnemyTakeDamage(20);

                break;
        }
    }

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

    public void ResetCylinder()
    {
        arrayIndex = 0;
        shootIndex = 0;
        firedIndex = -1;
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

    public void PlayerTakeDamage(int damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
    }

}