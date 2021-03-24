using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skeleton : Enemy
{
    public AudioClip walk, attack, hit, shield, death;
    public float distanceFromPlayer;
    public bool canPlaySound;

    public override void Start()
    {
        base.Start();

        HitBox = this.gameObject.transform.GetChild(0).gameObject;
    }

    private void Awake()
    {
        if (GameManager.Instance.hasKilledSkeleton)
        {
            Boss.unlockBossPoints += 1;

            Destroy(gameObject);
        }
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

    public void playHitSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(hit);
        }
    }

    public void playAttackSound()
    {
        if (canPlaySound)
        {
            if (HitBox.activeInHierarchy)
            {
                Audio.Instance.enemySound(attack);
            }
            else
            {
                Audio.Instance.enemySound(shield);
            }
        }
    }

    public override void moveTowardsPlayer()
    {
        if (Vector2.Distance(player.position, EnemyRigidBody.position) <= attackRange)
        {

            checkPlayerShield();

            EnemyAnimation.SetBool("walk", false);

            shouldCount = true;

            if (shouldCount)
            {
                count += Time.deltaTime;

                if (count > nextAttack - 1f)
                {
                    Collider.enabled = false;
                }

                if (count > nextAttack)
                {
                    EnemyAnimation.SetTrigger("attack");

                    Collider.enabled = true;
                    count = 0;
                    nextAttack = 5;
                }
            }
        }
        else
        {
            Collider.enabled = true;
            shouldCount = false;
            EnemyAnimation.SetBool("walk", true);
            Vector2 target = new Vector2(player.position.x, EnemyRigidBody.position.y);
            Vector2 newPos = Vector2.MoveTowards(EnemyRigidBody.position, target, speed * Time.fixedDeltaTime);
            EnemyRigidBody.MovePosition(newPos);
        }
    }

    public override void Death()
    {
        base.Death();
        GameManager.Instance.hasKilledSkeleton = true;
    }

    public override void Damage()
    {
        base.Damage();
        playHitSound();
    }

    public override void FixedUpdate()
    {
        if (player != null)
        {
            base.FixedUpdate();

            if (playerScript.transform.position.x < -10.0f)
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
}