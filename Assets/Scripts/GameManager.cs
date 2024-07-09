using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();
    public List<GameObject> bulletObjects = new List<GameObject>();

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

    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI discardPileText;

    public GameObject bulletToAdd;
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

    private void Start()
    {
        currentHealth = maxHealth;
        EnemySelection(selectedEnemy);

        SetStartSlots();
    }

    void Update()
    {
        UpdateDeckCount();
        DisplayEnemyQ();
    }

    public void SetStartSlots()
    {
        availableBulletSlots = new bool[bulletSlots.Length];
        for (int i = 0; i < availableBulletSlots.Length; i++)
        {
            availableBulletSlots[i] = true;
        }

        availableCardSlots = new bool[cardSlots.Length];
        for (int i = 0; i < availableCardSlots.Length; i++)
        {
            availableCardSlots[i] = true;
        }

    }

    #region Card Methods

    public void DrawCards()
    {
        if (deck.Count > 0)
        {
            Card randCard = deck[Random.Range(0, deck.Count)];

            bool cardDrawn = false;
            for (int i = 0; i < availableCardSlots.Length; i++)
            {
                if (availableCardSlots[i] == true) 
                {
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;

                    randCard.baseSortingOrder = i;
                    randCard.transform.position = cardSlots[i].position;
                    randCard.transform.rotation = cardSlots[i].rotation;
                    randCard.hasBeenPlayed = false;

                    availableCardSlots[i] = false;

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
        for (int i = arrayIndex; i < availableBulletSlots.Length; i++)
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

    public void DisplayBulletQueue()
    {
        Debug.Log("Current Bullets in Queue:");
        foreach (Bullet bullet in BulletQueue)
        {
            Debug.Log(bullet.name);
        }
    }
    public void DisplayEnemyQ()
    {
        if (TargetEnemyQueue == null)
        {
            Debug.LogError("TargetEnemyQueue is null!");
            return;
        }

        if (TargetEnemyQueue.Count == 0)
        {
            Debug.Log("TargetEnemyQueue is empty.");
            return;
        }

        Debug.Log("Current enemy in Queue:");
        foreach (Enemy enemy in TargetEnemyQueue)
        {
            if (enemy == null)
            {
                Debug.LogWarning("An enemy in the queue is null!");
                continue;
            }

            if (enemy.gameObject == null)
            {
                Debug.LogWarning("An enemy's gameObject is null!");
                continue;
            }

            Debug.Log(enemy.gameObject);
        }
    }


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

    public void TurnShot()
    {
        switch (firedIndex)
        {
            case 0:
                selectedEnemy.EnemyTakeDamage(20);
                break;
            case 1:
                selectedEnemy.EnemyTakeDamage(20);

                break;
            case 2:
                selectedEnemy.EnemyTakeDamage(20);
                break;
            case 3:
                selectedEnemy.EnemyTakeDamage(20);

                break;
            case 4:
                selectedEnemy.EnemyTakeDamage(20);

                break;
            case 5:
                selectedEnemy.EnemyTakeDamage(20);

                break;
            case 6:
                selectedEnemy.EnemyTakeDamage(20);

                break;
            case 7:
                selectedEnemy.EnemyTakeDamage(20);

                break;
            case 8:
                selectedEnemy.EnemyTakeDamage(20);

                break;
        }
    }
    public void EnemySelection(Enemy enemy)
    {
        selectedEnemy = enemy;
        selectedEnemyContainerImage.transform.position = enemy.transform.position;
        selectedEnemyContainerImage.SetActive(true);
    }

}