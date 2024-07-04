using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Card management
    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    // Bullet management
    public List<GameObject> bulletObjects = new List<GameObject>();
    public Image[] bulletSlots;
    public bool[] availableBulletSlots;
    public Queue<Bullet> BulletQueue = new Queue<Bullet>();

    // Cylinder and rotation
    public GameObject cylinder;
    public Button fireButton;
    private bool isRotating = false;
    private Quaternion targetRotation;
    public float rotationSpeed = 60f;
    private float rotationTime = 1f;

    // Health management
    public int maxHealth = 10;
    public int currentHealth;
    public List<Image> heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // UI Text elements
    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI discardPileText;

    // Index management
    private int arrayIndex = 0;
    private int shootIndex = 0;

    private void Start()
    {
        currentHealth = maxHealth; // Set player health to max health

        InitializeCardSlots();
        InitializeBulletSlots();
        UpdateHealthUI();
    }

    private void InitializeCardSlots()
    {
        availableCardSlots = new bool[cardSlots.Length];
        for (int i = 0; i < availableCardSlots.Length; i++)
        {
            availableCardSlots[i] = true;
        }
    }

    private void InitializeBulletSlots()
    {
        availableBulletSlots = new bool[bulletSlots.Length];
        for (int i = 0; i < availableBulletSlots.Length; i++)
        {
            availableBulletSlots[i] = true;
        }
    }

    // Card methods
    public void DrawCards()
    {
        if (deck.Count > 0)
        {
            Card randCard = deck[Random.Range(0, deck.Count)];

            bool cardDrawn = false;
            for (int i = 0; i < availableCardSlots.Length; i++)
            {
                if (availableCardSlots[i])
                {
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;
                    randCard.transform.position = cardSlots[i].position;
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

    // Bullet methods
    public bool AddBullet()
    {
        bool bulletAdded = false;
        for (int i = arrayIndex; i < availableBulletSlots.Length; i++)
        {
            if (availableBulletSlots[i])
            {
                GameObject bulletToAdd = bulletObjects[arrayIndex];
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

        DisplayBulletQueue();
        UpdateBulletIndex();

        return bulletAdded;
    }

    private void UpdateBulletIndex()
    {
        arrayIndex = (arrayIndex < availableBulletSlots.Length - 1) ? arrayIndex + 1 : 0;
    }

    public void FireBullet()
    {
        if (!isRotating && BulletQueue.Count > 0)
        {
            Bullet firedBullet = BulletQueue.Dequeue();

            for (int i = shootIndex; i < bulletSlots.Length; i++)
            {
                if (bulletSlots[i].sprite == firedBullet.GetComponent<Image>().sprite)
                {
                    bulletSlots[i].enabled = false;
                    bulletSlots[i].sprite = null;
                    bulletSlots[i].color = Color.clear;

                    availableBulletSlots[i] = true;
                    break;
                }
            }

            RotateCylinder();
            UpdateShootIndex();
        }
        else
        {
            Debug.Log("No bullets in queue to fire or cylinder is rotating.");
        }

        DisplayBulletQueue();
    }

    private void UpdateShootIndex()
    {
        shootIndex = (shootIndex < availableBulletSlots.Length - 1) ? shootIndex + 1 : 0;
    }

    private void RotateCylinder()
    {
        if (cylinder != null && !isRotating)
        {
            if (fireButton != null)
            {
                fireButton.interactable = false;
            }

            targetRotation = cylinder.transform.rotation * Quaternion.Euler(0, 0, 60f);
            StartCoroutine(RotateCoroutine());
        }
        else
        {
            Debug.LogWarning("Cylinder GameObject is not assigned or already rotating.");
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

        if (fireButton != null)
        {
            fireButton.interactable = true;
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

    // Health methods
    public void UpdateHealthUI()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            heartImages[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead");
            // Add logic for game over or reset
        }
    }
}
