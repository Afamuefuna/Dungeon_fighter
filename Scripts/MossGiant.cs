using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MossGiant : Enemy
{
    [SerializeField]
    float numberOfAttacksPerformed, resetNumberOfAttacksToBePerformed,
        setNextAttack;

    [SerializeField]
    bool attacking, turn;

    [SerializeField]
    bool reachedDestination, move;

    [SerializeField]
    private float firstCount;
    [SerializeField]
    private float firstWaitTime;

    [SerializeField]
    private GameObject blockingWall;

    [SerializeField]
    private bool keepMoving = true, activateMossAttack;

    [SerializeField]
    private AudioClip walk, hit, attack, death, idleMood;

    [SerializeField]
    private bool canPlaySound;
    private float distanceFromPlayer;

    public override void idleMode()
    {
        idle();
    }

    public void turnOff()
    {
        turn = false;
    }

    public void turnOn()
    {
        turn = true;
    }

    public override void Start()
    {
        base.Start();
        count = 0;
        nextAttack = setNextAttack;
        numberOfAttacksPerformed = resetNumberOfAttacksToBePerformed;
        Collider.enabled = false;

        Physics2D.IgnoreCollision(HitBox.GetComponent<Collider2D>(), playerScript.frontDetector.GetComponent<Collider2D>(), true);
        Physics2D.IgnoreCollision(HitBox.GetComponent<Collider2D>(), playerScript.backDetector.GetComponent<Collider2D>(), true);
    }

    private void Awake()
    {
        if (GameManager.Instance.hasKilledMoss)
        {
            Boss.unlockBossPoints += 1;

            Destroy(gameObject);
        }
    }

    public override void Damage()
    {
        base.Damage();
        if(life <= 0)
        {
            blockingWall.GetComponent<Collider2D>().enabled = false;
            blockingWall.GetComponent<Tilemap>().color = Color.white;
        }        
    }

    public override void FixedUpdate()
    {
        if(playerScript != null)
        {
            determineMode();
            if (Mode == EnemyMode.Idle)
            {
                idleMode();
            }
            if (Mode == EnemyMode.Attack)
            {
                attackMode();
            }
            if (Mode == EnemyMode.Die)
            {
                Death();
            }

            if (playerScript.transform.position.x > 14f && playerScript.transform.position.x < 48f
                && playerScript.transform.position.y < -7f)
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

    public override void determineMode()
    {
        if (life <= 0)
        {
            Mode = EnemyMode.Die;
        }
        else
        {
            if (playerScript.transform.position.y < -15)
            {
                if (playerScript.transform.position.x > 14f && playerScript.transform.position.x < 32f)
                {
                    if (Vector2.Distance(player.position, EnemyRigidBody.position) <= 30)
                    {
                        activateMossAttack = true;
                    }
                }
            }
            if (activateMossAttack)
            {
                Mode = EnemyMode.Attack;
            }
            else if(!activateMossAttack)
            {
                Mode = EnemyMode.Idle;
            }
        }
    }

    public override void LookAtPlayer()
    {
        if (playerScript.transform.position.x < endMarker.transform.position.x - 1f)
        {
            blockingWall.GetComponent<Collider2D>().enabled = true;
            blockingWall.GetComponent<Tilemap>().color = Color.green;
        }
        if (!attacking)
        {
            if (activateMossAttack)
            {
                if (turn)
                {
                    base.LookAtPlayer();
                }
            }
        }
    }
    public override void moveTowardsPlayer()
    {
        if (Vector2.Distance(player.position, EnemyRigidBody.position) <= attackRange || attacking)
        {
            shouldCount = true;

            if (shouldCount)
            {
                count += Time.deltaTime;

                if (count > nextAttack - 1.5f)
                {
                    Collider.enabled = false;
                    EnemyRenderer.color = Color.green;
                }
                else
                {
                    Collider.enabled = true;
                }

                if (count > nextAttack)
                {
                    if (numberOfAttacksPerformed < 4)
                    {
                        attacking = true;
                        EnemyAnimation.SetBool("attack", true);
                    }
                    if (numberOfAttacksPerformed >= 4)
                    {
                        EnemyRenderer.color = Color.white;
                        count = 0;
                        nextAttack = setNextAttack;
                        numberOfAttacksPerformed = resetNumberOfAttacksToBePerformed;

                        if (EnemyAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && !EnemyAnimation.IsInTransition(0))
                        {
                            attacking = false;
                        }

                        EnemyAnimation.SetBool("attack", false);
                    }
                }
                else
                {
                    EnemyAnimation.SetBool("walk", false);
                }
            }
        }


        if (!attacking && Vector2.Distance(player.position, EnemyRigidBody.position) > attackRange)
        {
            base.LookAtPlayer();

            Collider.enabled = false;
            shouldCount = false;
            EnemyAnimation.SetBool("attack", false);
            EnemyAnimation.SetBool("walk", true);
            if (this.EnemyAnimation.GetCurrentAnimatorStateInfo(0).IsName("walk"))
            {
                Vector2 target = new Vector2(player.position.x, EnemyRigidBody.position.y);
                Vector2 newPos = Vector2.MoveTowards(EnemyRigidBody.position, target, speed * Time.fixedDeltaTime);
                EnemyRigidBody.MovePosition(newPos);
            }
        }

    }

    void idle()
    {
        lifeText.gameObject.SetActive(false);

        if (FaceRight)
        {
            if (keepMoving)
            {
                EnemyAnimation.SetBool("walk", true);
                firstWaitTime = 4.5f;
                firstCount = 0;
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                transform.localScale = new Vector2(theScale, transform.localScale.y);
            }
        }
        else
        {
            if (keepMoving)
            {
                EnemyAnimation.SetBool("walk", true);
                firstWaitTime = 4.5f;
                firstCount = 0;
                transform.Translate(-Vector2.right * speed * Time.deltaTime);
                transform.localScale = new Vector2(-theScale, transform.localScale.y);
            }
        }

        if (transform.position.x <= startMarker.position.x + 1)
        {
            keepMoving = false;

            EnemyAnimation.SetBool("walk", false);

            if(firstCount <= firstWaitTime)
            {
                firstCount += Time.deltaTime;
            }

            if (firstCount >= firstWaitTime)
            {
                keepMoving = true;
                FaceRight = true;
                isFlipped = false;
            }
        }

        if (transform.position.x >= endMarker.position.x - 1)
        {
            keepMoving = false;

            EnemyAnimation.SetBool("walk", false);

            if (firstCount <= firstWaitTime)
            {
                firstCount += Time.deltaTime;
            }

            if (firstCount >= firstWaitTime)
            {
                keepMoving = true;
                FaceRight = false;
                isFlipped = true;
            }
        }      
    }

    public override void Death()
    {
        base.Death();
        GameManager.Instance.hasKilledMoss = true;
    }

    public void countNumberOfAttacksPerformed()
    {
        numberOfAttacksPerformed += 1;
    }

    public void playWalkSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(walk);
        }
    }

    public void playHitSound()
    {
        if (canPlaySound)
        {
            if(life > 0)
            {
                Audio.Instance.enemySound(hit);
            }
        }
    }

    public void playAttackSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(attack);
        }
    }

    public void playDeathSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(death);
        }
    }

    public void playIdleSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(idleMood);
        }
    }
}
