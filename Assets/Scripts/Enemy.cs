using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
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

    public TextMeshProUGUI healthText;

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
        healthText.text = currentHealth + " / " + maxHealth;

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
        UpdateDebuffDisplays();

        foreach (Effect effect in activeEffects)
        {
            if (effect.IsActive())
            {
                effect.ApplyEffect(this);
            }
        }

        UpdateDebuffDisplays();
    }
    public void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
        healthText.text = currentHealth + " / " + maxHealth;
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

    #region Effects
    public void AddEffect(Effect newEffect)
    {
        // Check if the effect of the same type already exists
        Effect existingEffect = activeEffects.FirstOrDefault(effect => effect.effectType == newEffect.effectType);

        if (existingEffect != null)
        {
            // If the effect exists, update its stack count
            existingEffect.stackCount += newEffect.stackCount;
        }
        else
        {
            // If the effect does not exist, add it to the list 
            activeEffects.Add(newEffect);
        }

        UpdateDebuffDisplays();
    }
    public void RemoveEffect(Effect effect)
    {
        activeEffects.Remove(effect);
        UpdateDebuffDisplays();
    }

    public void AddHellfire(int stackCount)
    {
        for (int i = 0; i < stackCount; i++)
        {
            // Create a new instance each time
            AddEffect(new Effect(EffectType.Hellfire, 1, 1)); // Use the same parameters as the original Hellfire instance
        }
        UpdateDebuffDisplays();
    }
    public void AddThunder(int stackCount)
    {
        for (int i = 0; i < stackCount; i++)
        {
            // Create a new instance each time
            AddEffect(new Effect(EffectType.Thunder, 1, 1)); // Use the same parameters as the original Thunder instance
        }
        UpdateDebuffDisplays();
    }
    public void CreateExplosion()
    {
        Effect hellfireEffect = activeEffects.FirstOrDefault(e => e.effectType == EffectType.Hellfire);
        Effect thunderEffect = activeEffects.FirstOrDefault(e => e.effectType == EffectType.Thunder);

        if (hellfireEffect != null && thunderEffect != null)
        {
            int totalExplosionStacks = hellfireEffect.stackCount * thunderEffect.stackCount;

            // Assuming you want to add a single Explosion effect with the total stack count
            Effect explosionEffect = new Effect(EffectType.Explosion, totalExplosionStacks, 5); // Adjust the damagePerTurn as needed
            AddEffect(explosionEffect);

            hellfireEffect.stackCount = 0;
            thunderEffect.stackCount = 0;
            // Optionally, remove the Hellfire and Thunder effects after creating the Explosion
            activeEffects.RemoveAll(e => e.effectType == EffectType.Hellfire || e.effectType == EffectType.Thunder); 
        }

        UpdateDebuffDisplays();
    }

    public void UpdateDebuffDisplays()
    {
        // Clear existing debuff displays
        foreach (Transform child in debuffDisplays.transform)
        {
            Destroy(child.gameObject);
        }

        // Create new debuff displays only for active effects with stackCount > 0
        foreach (Effect effect in activeEffects.Where(e => e.stackCount > 0))
        {
            GameObject newElement = Instantiate(gridElementPrefab, debuffDisplays.transform);
            Image elementImage = newElement.GetComponent<Image>();

            // Update the sprite for the effect
            if (elementImage != null && effectSprites.ContainsKey(effect.effectType))
            {
                elementImage.sprite = effectSprites[effect.effectType];
            }

            // Access the TextMeshProUGUI component and update it with the stackCount
            TextMeshProUGUI stackText = newElement.GetComponentInChildren<TextMeshProUGUI>();
            if (stackText != null)
            {
                stackText.text = effect.stackCount.ToString();
            }
        }
    }


    #endregion
}
