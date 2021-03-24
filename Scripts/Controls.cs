using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controls : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    Player player;

    Animator playerAnim;

    bool canAttack;

    private void Start()
    {
        playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(gameObject.name == "Shield")
        {
            shieldUp();
        }
        if(gameObject.name == "jump")
        {
            JumpUp();
        }
        if(gameObject.name == "attack")
        {
            if (canAttack)
            {
                attackUp();
                canAttack = false;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (gameObject.name == "Shield")
        {
            shieldDown();
        }
        if (gameObject.name == "attack")
        {
            if (!canAttack)
            {
                canAttack = true;
            }
        }
    }

    public void shieldUp()
    {
        player.shield();
    }

    public void shieldDown()
    {
        player.speed = 3;
        player._isShieldActive = false;
        player.animation.playerShield(false);
    }

    public void JumpUp()
    {
        player.jump(playerAnim);
    }

    public void attackUp()
    {
        player.attack();
    }
}
