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
    public int enemyHealth;
    public int currentHealth; 

    public float rotationSpeed = 120f;
    public bool isRotating = false;
    public Quaternion targetRotation;
    public float rotationTime = 0.5f;

    public int arrayIndex = 0;
    public int shootIndex = 0;
    public int firedIndex = -1;

    public GameObject selectedEnemy;
    public GameObject selectedEnemyContainerImage;

    private void Start()
    {
        currentHealth = maxHealth;

        TakeDamage(45);
        SetStartSlots();
        UpdateHealthUI();
    }

    void Update()
    {
        UpdateDeckCount();
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

    public void FireBullet()
    {
        if (!isRotating && BulletQueue.Count > 0)
        {
            firedBullet = BulletQueue.Dequeue();

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
    public void UpdateHealthUI()
    {
        TakeDamage(20);
    }

    public void TakeDamage(int dmgAmount)
    {
        healthAmount -= dmgAmount;
        healthBar.fillAmount = healthAmount / 100f;
    }

    #endregion

    public void TurnShot()
    {
        // Add logic for each bullet type
        switch (firedIndex)
        {
            case 0:
                enemyHealth -= 1;
                break;
            case 1:
                enemyHealth -= 1;
                break;
            case 2:
                enemyHealth -= 1;
                break;
            case 3:
                enemyHealth -= 1;

                break;
            case 4:
                enemyHealth -= 1;

                break;
            case 5:
                enemyHealth -= 1;

                break;
            case 6:
                enemyHealth -= 1;

                break;
            case 7:
                enemyHealth -= 1;

                break;
            case 8:
                enemyHealth -= 1;

                break;
        }
    }

    public void EnemySelection()
    {
        selectedEnemyContainerImage.SetActive(true);
        
    }
}