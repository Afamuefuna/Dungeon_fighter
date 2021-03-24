using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Enemy
{
    [SerializeField]
    private GameObject laser;
    public bool Attacking, canPlaySound;
    private float distanceFromPlayer;
    public AudioClip walk, hurt, death, laserShoot;

    private void Awake()
    {
        if (GameManager.Instance.hasKilledMonster)
        {
            Boss.unlockBossPoints += 1;

            Destroy(gameObject);
        }
    }

    public override void Damage()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(hurt);
        }
        base.Damage();
    }

    public void playWalkSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(walk);
        }
    }

    public void playDeathSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(death);
        }
    }

    public void playLaserShootSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(laserShoot);
        }
    }

    public override void FixedUpdate()
    {
        if (playerScript != null)
        {
            base.FixedUpdate();

            if (playerScript.transform.position.x > -10.0f && playerScript.transform.position.x < 30f &&
                playerScript.transform.position.y > -6)
            {
                canPlaySound = true;

                distanceFromPlayer = Vector2.Distance(player.position, EnemyRigidBody.position);

                distanceFromPlayer = 1.0f / distanceFromPlayer;

                if (distanceFromPlayer > 0.7f)
                {
                    distanceFromPlayer = 0.7f;
                }

                Audio.Instance.audioSources[1].volume = distanceFromPlayer;
            }
            else
            {
                canPlaySound = false;
            }
        }
    }

    public override void idleMode()
    {
        lifeText.gameObject.SetActive(false);

        if (FaceRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            transform.localScale = new Vector2(-theScale, transform.localScale.y);
            //EnemyRenderer.flipX = false;
        }
        else
        {
            transform.Translate(-Vector2.right * speed * Time.deltaTime);
            transform.localScale = new Vector2(theScale, transform.localScale.y);
            //EnemyRenderer.flipX = true;
        }

        if (transform.position.x <= startMarker.position.x + 2)
        {
            FaceRight = true;
            isFlipped = false;
        }

        if (transform.position.x >= endMarker.position.x - 2)
        {
            FaceRight = false;
            isFlipped = true;
        }

        EnemyAnimation.SetBool("walk", true);
    }

    public override void LookAtPlayer()
    {
        if (!Attacking)
        {
            if (transform.position.x < player.position.x)
            {
                isFlipped = false;
                FaceRight = true;
                transform.localScale = new Vector2(-theScale, transform.localScale.y);
            }
            else if (transform.position.x > player.position.x)
            {
                FaceRight = false;
                isFlipped = true;
                transform.localScale = new Vector2(theScale, transform.localScale.y);
            }
        }
    }

    public override void Death()
    {
        base.Death();
        GameManager.Instance.hasKilledMonster = true ;
    }

    public void shoot()
    {
        playLaserShootSound();

        if (FaceRight)
        {
            Instantiate(laser, gameObject.transform.position +
                new Vector3(0.5f, +0.058f, 0), Quaternion.identity);
        }
        else
        {
            Instantiate(laser, gameObject.transform.position +
                new Vector3(-0.5f, -0.058f, 0), Quaternion.identity);
        }
    }

    public void AttackingOn()
    {
        Attacking = true;
    }

    public void AttackingOff()
    {
        Attacking = false;
    }
}
