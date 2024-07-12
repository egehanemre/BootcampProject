using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;

    public Image healthBar;

    public float maxHealth = 100;
    public float currentHealth;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        currentHealth = maxHealth;
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
}
