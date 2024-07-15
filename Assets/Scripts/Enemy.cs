using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;
    public Image healthBar;

    public Sprite hellfireSprite;
    public Sprite explosionSprite;
    public Sprite thunderSprite;


    public GameObject debuffDisplays;
    public GameObject gridElementPrefab;
    private Dictionary<EffectType, Sprite> effectSprites; // Map effect types to sprites

    public GameObject[] effectImageSlots;

    public float maxHealth = 100;
    public float currentHealth;

    public List<Effect> activeEffects = new List<Effect>();

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        currentHealth = maxHealth;

        effectSprites = new Dictionary<EffectType, Sprite>()
{
    { EffectType.Hellfire, hellfireSprite },
    { EffectType.Explosion, explosionSprite },
    { EffectType.Thunder, thunderSprite },
    // Add other effects as needed
};

    }
    public void UpdateEffects()
    {
        CreateExplosion();

       foreach (Effect effect in activeEffects)
        {
            if (effect.IsActive())
            {
                effect.ApplyEffect(this);
            }
        }
    }

    private void Update()
    {
        
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    public void DoAction()
    {
        gameManager.PlayerTakeDamage(10);
    }

    public void OnMouseDown()
    {
        gameManager.EnemySelection(this);  // Pass the current enemy instance
    }

    public void EnemyTakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        gameObject.SetActive(false);
    }

    public Effect Hellfire = new Effect(EffectType.Hellfire, 3, 10);
    public Effect Explosion = new Effect(EffectType.Explosion, 3, 10);
    public Effect Thunder = new Effect(EffectType.Thunder, 5, 20);

    public void AddEffect(Effect effect)
    {
        activeEffects.Add(effect);
    }
    public void RemoveEffect(Effect effect)
    {
        activeEffects.Remove(effect);
    }

    public void AddHellfire()
    {
        AddEffect(Hellfire);
        UpdateDebuffDisplays();

    }

    public void AddThunder()
    {
        AddEffect(Thunder);
        UpdateDebuffDisplays();

    }

    public void CreateExplosion()
    {
        bool hasHellfire = activeEffects.Any(effect => effect.effectType == EffectType.Hellfire);
        bool hasThunder = activeEffects.Any(effect => effect.effectType == EffectType.Thunder);

        if (hasHellfire && hasThunder)
        {
            // Assuming you have a way to calculate duration and power for the explosion
            Effect explosion = new Effect(EffectType.Explosion, 1, 50); // Example values for duration and power
            AddEffect(explosion);

            // Remove all Hellfire and Thunder effects
            activeEffects.RemoveAll(effect => effect.effectType == EffectType.Hellfire || effect.effectType == EffectType.Thunder);

            UpdateDebuffDisplays();
        }
    }

    private void UpdateDebuffDisplays()
    {
        // Clear existing debuff displays
        foreach (Transform child in debuffDisplays.transform)
        {
            Destroy(child.gameObject);
        }

        // Create new debuff displays for active effects
        foreach (Effect effect in activeEffects)
        {
            GameObject newElement = Instantiate(gridElementPrefab, debuffDisplays.transform);
            Image elementImage = newElement.GetComponent<Image>();

            if (elementImage != null && effectSprites.ContainsKey(effect.effectType))
            {
                elementImage.sprite = effectSprites[effect.effectType];
            }
        }
    }
}
