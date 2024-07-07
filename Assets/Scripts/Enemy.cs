using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;

    public int maxHealth = 5;
    public int currentHealth;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        currentHealth = maxHealth;
    }

    public void OnMouseDown()
    {
        gameManager.selectedEnemy = gameObject;
        gameManager.selectedEnemyContainerImage.transform.position = transform.position;
        gameManager.selectedEnemyContainerImage.SetActive(true);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
