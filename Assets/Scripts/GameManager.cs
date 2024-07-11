using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.Mathematics;
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

    public GameObject bulletToAdd;
    public GameObject closedBulletSlot;

    public int bulletIndex;

    public Bullet firedBullet;
    public Bullet bullet;

    public int maxHealth = 5;
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

    public bool isPlayerTurn = true;

    public void BattleManager()
    {
        if (!isPlayerTurn) 
        {

            ClearHand();

            foreach (Enemy enemy in enemies)
            {
                enemy.DoAction();
            }
            isPlayerTurn = true;

            if (deck.Count < 6)
            {
                ShuffleCards();
                ResetActiveSlots();
                DrawCards(5);
            }
            else
            {
                ResetActiveSlots();
                DrawCards(5);
            }
        }     
    }

    public void ResetActiveSlots()
    {
        for (int i = 0; i < availableCardSlots.Length; i++)
        {
            availableCardSlots[i] = true;
        }
    }

    public void PlayerTakeDamage(int damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        EnemySelection(selectedEnemy);

        SetStartSlots();
        SummonEnemies();
        DrawCards(5);
    }

    void Update()
    {
        if (!isRotating)
        {
            BattleManager();
        }
        UpdateDeckCount();
        //DisplayEnemyQ();
    }

    #region Card Methods

    public void DrawCards(int drawAmount)
    {
        for (int x = 0; x < drawAmount; x++)
        {
            if (deck.Count > 0)
            {

                Card randCard = deck[UnityEngine.Random.Range(0, deck.Count)];

                bool cardDrawn = false;

                for (int i = 0; i < availableCardSlots.Length; i++)
                {
                    if (availableCardSlots[i] == true)
                    { 
                        hand.Add(randCard);                        

                        randCard.gameObject.SetActive(true);
                        randCard.handIndex = i;

                        randCard.baseSortingOrder = i;

                        randCard.transform.position = cardSlots[i].position;
                        randCard.transform.rotation = cardSlots[i].rotation;

                        availableCardSlots[i] = false;

                        Debug.Log("Card drawn index " + i);

                        deck.Remove(randCard);

                        cardDrawn = true;
                        break;
                    }
                }

                if (!cardDrawn)
                {
                    Debug.Log("No available card slots to draw a card.");
                }
            }
            else
            {
                Debug.Log("No cards left in the deck to draw.");
            }
        }
    }

    public void ShuffleCards()
    {
        if (discardPile.Count > 0)
        {
            foreach (Card card in discardPile)
            {
                deck.Add(card);
            }
            discardPile.Clear();
        }
    }

    public void UpdateDeckCount()
    {
        deckSizeText.text = "Deck: " + deck.Count;
        discardPileText.text = "Discard: " + discardPile.Count;
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

        if (arrayIndex < availableBulletSlots.Length - 1)
        {
            arrayIndex++;
        }
        else
        {
            arrayIndex = 0;
        }

        return bulletAdded;
    }

    public void Fire()
    {
        int bulletsToFire = BulletQueue.Count;
        StartCoroutine(FireMultipleBullets(bulletsToFire));
        isPlayerTurn = false;
    }

    public void ClearHand()
    {
        foreach(Card card in hand)
        {
            card.MoveToDiscard();
        }
    }

    private IEnumerator FireMultipleBullets(int bulletCount)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            FireBullet();

            // Wait until the rotation is complete before proceeding
            while (isRotating)
            {
                yield return null;
            }
        }
    }


    public void FireBullet()
    {
        if (!isRotating && BulletQueue.Count > 0 && selectedEnemy != null)
        {
            firedBullet = BulletQueue.Dequeue();
            selectedEnemy = TargetEnemyQueue.Peek();

            for (int i = 0 + shootIndex; i < bulletSlots.Length; i++)
            {
                if (bulletSlots[i].sprite == firedBullet.GetComponent<Image>().sprite)
                {
                    bulletSlots[i].enabled = false;
                    bulletSlots[i].sprite = null;
                    bulletSlots[i].color = Color.clear;

                    availableBulletSlots[i] = true;

                    firedIndex = firedBullet.bulletIndex;

                    TurnShot();

                    TargetEnemyQueue.Dequeue();

                    break;
                }
            }

            // Rotate the cylinder
            RotateCylinder();

            if (shootIndex < availableBulletSlots.Length - 1)
            {
                shootIndex++;
            }
            else
            {
                shootIndex = 0;
            }
        }
        else
        {
            Debug.Log("No bullets in queue to fire or cylinder is rotating.");
        }
    }

    #endregion

    #region cylinder animations
    private void RotateCylinder()
    {
        if (cylinder != null && !isRotating)
        {
            // Disable the fire button
            if (fireButton != null)
            {
                fireButton.interactable = false;
            }

            targetRotation = cylinder.transform.rotation * Quaternion.Euler(0, 0, 60f); // Rotate
            StartCoroutine(RotateCoroutine());
        }
        else
        {
            Debug.LogWarning("Cylinder GameObject is not assigned or already rotating.");
        }
    }

    //public void DisplayBulletQueue()
    //{
    //    Debug.Log("Current Bullets in Queue:");
    //    foreach (Bullet bullet in BulletQueue)
    //    {
    //        Debug.Log(bullet.name);
    //    }
    //}
    //public void DisplayEnemyQ()
    //{
    //    if (TargetEnemyQueue == null)
    //    {
    //        Debug.LogError("TargetEnemyQueue is null!");
    //        return;
    //    }

    //    if (TargetEnemyQueue.Count == 0)
    //    {
    //        Debug.Log("TargetEnemyQueue is empty.");
    //        return;
    //    }

    //    Debug.Log("Current enemy in Queue:");
    //    foreach (Enemy enemy in TargetEnemyQueue)
    //    {
    //        if (enemy == null)
    //        {
    //            Debug.LogWarning("An enemy in the queue is null!");
    //            continue;
    //        }

    //        if (enemy.gameObject == null)
    //        {
    //            Debug.LogWarning("An enemy's gameObject is null!");
    //            continue;
    //        }

    //        //Debug.Log(enemy.gameObject);
    //    }
    //}


    private IEnumerator RotateCoroutine()
    {
        isRotating = true;
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            cylinder.transform.rotation = Quaternion.RotateTowards(cylinder.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cylinder.transform.rotation = targetRotation;
        isRotating = false;

        // Enable the fire button after rotation is complete
        if (fireButton != null)
        {
            fireButton.interactable = true;
        }
    }

    #endregion

    #region health methods

    #endregion

    public void ManageMana()
    {

    }

    public void TurnShot()
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
            int randomIndex = UnityEngine.Random.Range(i, shuffledSpawnSlots.Count);
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
        selectedEnemyContainerImage.transform.position = enemy.transform.position;
        selectedEnemyContainerImage.SetActive(true);
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

}