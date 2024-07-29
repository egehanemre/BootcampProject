using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Animator animator;
    public bool x = false;

    public void TogglePlayerShootAnimation()
    {
        animator.SetTrigger("ShootTrigger");
    }
}