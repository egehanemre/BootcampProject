using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletManager : MonoBehaviour
{

    // Array to hold child GameObjects
    public List<GameObject> bulletObjects = new List<GameObject>();

    public Transform[] bulletSlots;
    public bool[] availableBulletSlots;

    public GameObject bulletToAdd;

    public static int bulletIndex;

    public void addBullet()
    {
        for (int i = 0; i < availableBulletSlots.Length; i++)
        {
            if (availableBulletSlots[i] == true)
            {
                bulletToAdd = bulletObjects[bulletIndex];
                bulletToAdd.SetActive(true);
                bulletToAdd.transform.position = bulletSlots[i].position;
                availableBulletSlots[i] = false;

                return;
            }
        }
    }
}
