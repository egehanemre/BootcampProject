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

    public GameObject bulletToAdd;
    public int bulletIndex;

    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI discardPileText;

    public Queue<Bullet> BulletQueue = new Queue<Bullet>();

    public GameObject cylinder; // Reference to the cylinder GameObject
    public float rotationSpeed = 60f; // Degrees per second

    private bool isRotating = false;
    private Quaternion targetRotation;
    private float rotationTime = 1f; // Time to rotate degrees

    public int arrayIndex = 0;
    public int shootIndex = 0;  

    public Bullet firedBullet;

    private void Start()
    {
        availableBulletSlots = new bool[bulletSlots.Length];
        for (int i = 0; i < availableBulletSlots.Length; i++)
        {
            availableBulletSlots[i] = true;
        }
    }

    public void DrawCards()
    {
        if (deck.Count >= 1)
        {
            Card randCard = deck[Random.Range(0, deck.Count)];

            for (int i = 0; i < availableCardSlots.Length; i++)
            {
                if (availableCardSlots[i] == true)
                {
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;

                    randCard.transform.position = cardSlots[i].position;
                    randCard.hasBeenPlayed = false;

                    availableCardSlots[i] = false;

                    deck.Remove(randCard);
                    return;
                }
            }
        }
    }

    public void ShuffleCards()
    {
        if (discardPile.Count >= 1)
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

    public void AddBullet()
    {
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
                break;
            }
        }

        DisplayBulletQueue();

        if (arrayIndex < availableBulletSlots.Length - 1)
        {
            arrayIndex++;
        }
        else
        {
            arrayIndex = 0;
        }
    }

    public void FireBullet()
    {
        if (BulletQueue.Count > 0)
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
            Debug.Log("No bullets in queue to fire.");
        }

        DisplayBulletQueue(); // Display queue contents after firing a bullet
    }

    private void RotateCylinder()
    {
        if (cylinder != null && !isRotating)
        {
            targetRotation = cylinder.transform.rotation * Quaternion.Euler(0, 0, 60f); // Rotate
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
    }

    public void DisplayBulletQueue()
    {
        Debug.Log("Current Bullets in Queue:");
        foreach (Bullet bullet in BulletQueue)
        {
            Debug.Log(bullet.name);
        }
    }

    void Update()
    {
        UpdateDeckCount();
    }
}
