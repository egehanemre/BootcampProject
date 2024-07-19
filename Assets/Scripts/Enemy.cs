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
    public Sprite soulfireSprite;
    public Sprite thunderSprite;
    public Sprite silverSprite;
    public Sprite bloodSprite;
    public Sprite holySprite;
    public Sprite darkSprite;

    public Sprite infernoSprite;
    public Sprite explosionSprite;
    public Sprite holyFlameSprite;
    public Sprite bloodFlameSprite;
    public Sprite blackFlameSprite;
    public Sprite plasmaSprite;
    public Sprite spiritStormSprite;
    public Sprite quickSilverSprite;
    public Sprite redLightningSprite;
    public Sprite pureSilverSprite;
    public Sprite sacrificeSprite;
    public Sprite unholySprite;
    public Sprite curseSprite;


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
        // Remove this enemy from the list
        gameManager.enemies.Remove(this);

        if (gameManager.enemies.Count > 0)
        {
            // If there are still enemies left, select another one as the target
            gameManager.selectedEnemy = gameManager.enemies[0]; // Example: select the first one in the list
            gameManager.EnemySelection(gameManager.selectedEnemy);
        }
        else
        {
            // If this was the last enemy, log the message and perform necessary actions
            gameManager.isAllEnemiesDefeated = true;
            gameManager.selectedEnemy = null; // Clear the selected enemy
                                              // Optionally, hide the enemy selection indicator
            if (gameManager.selectedEnemyContainerImage != null)
            {
                gameManager.selectedEnemyContainerImage.SetActive(false);
            }
        }

        // Destroy the enemy GameObject
        Destroy(gameObject);
    }

    #region Manage Effects
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

    #region Add Effects
    public void AddHellfire(int stackCount)
    {
        AddEffect(new Effect(EffectType.Hellfire, stackCount, 1)); // Adjust damagePerTurn as needed
        UpdateDebuffDisplays();
    }

    public void AddSoulfire(int stackCount)
    {
        AddEffect(new Effect(EffectType.Soulfire, stackCount, 1)); // Adjust damagePerTurn as needed
        UpdateDebuffDisplays();
    }

    public void AddThunder(int stackCount)
    {
        AddEffect(new Effect(EffectType.Thunder, stackCount, 1)); // Adjust damagePerTurn as needed
        UpdateDebuffDisplays();
    }

    public void AddSilver(int stackCount)
    {
        AddEffect(new Effect(EffectType.Silver, stackCount, 1)); // Adjust damagePerTurn as needed
        UpdateDebuffDisplays();
    }

    public void AddBlood(int stackCount)
    {
        AddEffect(new Effect(EffectType.Blood, stackCount, 1)); // Adjust damagePerTurn as needed
        UpdateDebuffDisplays();
    }

    public void AddHoly(int stackCount)
    {
        AddEffect(new Effect(EffectType.Holy, stackCount, 1)); // Adjust damagePerTurn as needed
        UpdateDebuffDisplays();
    }

    public void AddDark(int stackCount)
    {
        AddEffect(new Effect(EffectType.Dark, stackCount, 1)); // Adjust damagePerTurn as needed
        UpdateDebuffDisplays();
    }
    #endregion

    #region Combined Create Effects
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

    #endregion
}
