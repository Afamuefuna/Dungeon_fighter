using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Boss : MonoBehaviour
{
    public Transform player, point_A, point_B;
    public Rigidbody2D mRigidBody;
    public Animator mAnimator;

    [SerializeField]
    private float speed, attackRange, walkRange;
    public static bool isFlipped = false;

    private bool activateBoss;

    public GameObject HitBox, blockings;
    public Player playerScript;

    float theScale;

    [SerializeField]
    float resetNumberOfAttacks, setNextAttack;

    public bool shouldCount = false, attacking, reachedDestination, startRunAttack, startArrowAttack
        , charge, canGetHurt, hasPlayedChargingSound, hasLaughed;
    public float count, laughToPlay;

    [SerializeField]
    protected int life, numberOfRuns, numberOfShoots;

    public AudioClip footstep, swordSwing, arrow_shoot, charging, death, laugh1, laugh2, laugh3, laugh4, hurt;

    private float nextAttack;
    float numberOfAttacks;

    public GameObject arrow;

    public bool isEnemyAtMyBack;

    [SerializeField]
    public bool FaceRight = true;

    public string currentState;

    public enum EnemyMode { MOVE_TO_PLAYER_ATTACK, ARROW_ATTACK, RUN_ATTACK, DEATH }

    public EnemyMode Mode;

    [SerializeField]
    private bool runCounted = false;

    [SerializeField] CapsuleCollider2D bossCollider;

    [SerializeField]private float firstCount, firstWaitTime, secondCount, secondWaitTime
        , startAnotherAttackTime, ChangeAttackCountDown;

    [SerializeField]
    private bool canPlaySound;

    [SerializeField]
    public static int unlockBossPoints;

    const string BOSS_IDLE = "Idle";
    const string BOSS_RUN = "Run";
    const string BOSS_ATTACK = "Attack";
    const string BOSS_WALK = "walk";
    const string BOSS_HURT = "Hurt";
    const string BOSS_CHARGE = "Charge";
    const string BOSS_SHOOT = "Shoot_Bow";
    const string BOSS_DEATH = "Death";

    [SerializeField]
    private float fullHealth;

    [SerializeField]
    public TMP_Text lifeText;

    [SerializeField]
    HitBox mHitBox;

    [SerializeField]
    private float attacksToMake;

    private void Awake()
    {
        lifeText.gameObject.SetActive(false);
    }

    private void Start()
    {
        UImanager.Instance.enemyHealthUpdate(life, fullHealth, lifeText);

        laughToPlay = Random.Range(1, 4);

        bossCollider.enabled = false;
        theScale = transform.localScale.x;
        count = 0;
        nextAttack = setNextAttack;
        numberOfAttacks = resetNumberOfAttacks;

        playerScript.frontDetector.SetActive(false);
        playerScript.backDetector.SetActive(false);
    }

    void changeAnimationState(string newState)
    {
        if (currentState == newState) return;

        if (mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !mAnimator.IsInTransition(0))
        {
            mAnimator.Play(newState);
            currentState = newState;
        }
    }

    public void enableCanGetHurt()
    {
        playerScript.canGetHurt = true;
    }

    void detectWhenBossCharges()
    {
        if (currentState == BOSS_CHARGE)
        {
            if(hasPlayedChargingSound == false)
            {
                playChargingSound();
                hasPlayedChargingSound = true;
            }
        }
    }

    public void countNumberOfAttacks()
    {
        numberOfAttacks += 1;
    }

    private void FixedUpdate()
    {
        if (playerScript != null)
        {
            detectWhenBossCharges();
            if (playerScript.transform.position.x > point_A.transform.position.x + 13f)
            {
                UImanager.Instance.startBossWord();
                if (UImanager.Instance.endOfBossWord)
                {
                    activateBoss = true;
                }
                canPlaySound = true;
                blockings.GetComponent<Tilemap>().color = Color.red;
                blockings.GetComponent<Collider2D>().enabled = true;
            }

            if (!activateBoss)
            {
                if (unlockBossPoints <= 2)
                {
                    blockings.GetComponent<Tilemap>().color = Color.red;
                    blockings.GetComponent<Collider2D>().enabled = true;
                }
                else
                {
                    if (unlockBossPoints > 2)
                    {
                        blockings.GetComponent<Tilemap>().color = Color.white;
                        blockings.GetComponent<Collider2D>().enabled = false;
                    }
                }
            }
            if (activateBoss)
            {
                determineState();
            }
        }
    }

    void determineState()
    {
        if(life == 20)
        {
            Mode = EnemyMode.MOVE_TO_PLAYER_ATTACK;
        }
        if(life <= 15 && life > 7)
        {
            if(numberOfRuns <= 3)
            {
                Mode = EnemyMode.RUN_ATTACK;
            }
            if(numberOfRuns > 3)
            {
                Mode = EnemyMode.MOVE_TO_PLAYER_ATTACK;
                ChangeAttackCountDown = Time.deltaTime + ChangeAttackCountDown;
                if (ChangeAttackCountDown > startAnotherAttackTime)
                {
                    numberOfRuns = 0;
                    ChangeAttackCountDown = 0;
                }
            }
        }

        if (life <= 7 && life > 0)
        {
            if(numberOfShoots <= 7)
            {
                Mode = EnemyMode.ARROW_ATTACK;
            }
            if (numberOfShoots > 7)
            {
                Mode = EnemyMode.MOVE_TO_PLAYER_ATTACK;
                ChangeAttackCountDown = Time.deltaTime + ChangeAttackCountDown;
                if (ChangeAttackCountDown > startAnotherAttackTime)
                {
                    numberOfShoots = 0;
                    ChangeAttackCountDown = 0;
                }
            }
        }

        if (life <= 0)
        {
            Mode = EnemyMode.DEATH;
        }

        if(Mode == EnemyMode.DEATH)
        {
            Death();
            lifeText.gameObject.SetActive(false);
        }

        if (Mode == EnemyMode.MOVE_TO_PLAYER_ATTACK)
        {
            canGetHurt = true;
            moveToPlayerAttack();
            LookAtPlayer();
            lifeText.gameObject.SetActive(true);
        }
        if (Mode == EnemyMode.ARROW_ATTACK)
        {
            canGetHurt = false;
            shootArrow();
            lifeText.gameObject.SetActive(true);
        }
        if (Mode == EnemyMode.RUN_ATTACK)
        {
            canGetHurt = false;
            runAttack();
            lifeText.gameObject.SetActive(true);
        }
    }

    public void LookAtPlayer()
    {
        if (transform.position.x < player.position.x)
        {
            isFlipped = false;
            transform.localScale = new Vector2(theScale, transform.localScale.y);
        }
        else if (transform.position.x > player.position.x)
        {
            isFlipped = true;
            transform.localScale = new Vector2(-theScale, transform.localScale.y);
        }
    }

    public void moveToPlayerAttack()
    {
        hasPlayedChargingSound = false;
        startRunAttack = false;
        startArrowAttack = false;

        playerScript.frontDetector.SetActive(true);
        playerScript.backDetector.SetActive(true);

        checkPlayerShield();

        if (Vector2.Distance(player.position, mRigidBody.position) <= attackRange)
        {
            shouldCount = true;

            if (shouldCount)
            {
                if (!attacking)
                {
                    changeAnimationState(BOSS_IDLE);
                    bossCollider.enabled = true;
                }
            }
            if (shouldCount)
            {
                count += Time.deltaTime;

                if (count > nextAttack)
                {
                    if (numberOfAttacks < attacksToMake)
                    {
                        attacking = true;
                        bossCollider.enabled = true;
                        changeAnimationState(BOSS_ATTACK);
                    }
                    if (numberOfAttacks >= attacksToMake)
                    {
                        count = 0;
                        nextAttack = setNextAttack;
                        numberOfAttacks = resetNumberOfAttacks;
                        attacking = false;
                    }
                }
            }
        }
        else
        {
            if (attacking)
            {
                count += Time.deltaTime;

                if (count > nextAttack)
                {
                    if (numberOfAttacks < attacksToMake)
                    {
                        attacking = true;
                        bossCollider.enabled = true;
                        changeAnimationState(BOSS_ATTACK);
                    }

                    if (numberOfAttacks >= attacksToMake)
                    {
                        count = 0;
                        nextAttack = setNextAttack;
                        numberOfAttacks = resetNumberOfAttacks;
                        bossCollider.enabled = true;

                        attacking = false;
                    }
                }                
            }
            if (!attacking)
            {
                if (Vector2.Distance(player.position, mRigidBody.position) <= walkRange)
                {
                    bossCollider.enabled = false;
                    changeAnimationState(BOSS_WALK);
                    speed = 3;
                }
                else
                {
                    bossCollider.enabled = false;
                    changeAnimationState(BOSS_RUN);
                    speed = 4;
                }
                Vector2 target = new Vector2(player.position.x, mRigidBody.position.y);
                Vector2 newPos = Vector2.MoveTowards(mRigidBody.position, target, speed * Time.fixedDeltaTime);
                mRigidBody.MovePosition(newPos);
            }
        }
    }

    void shootArrow()
    {
        hasPlayedChargingSound = false;
        startRunAttack = false;
        bossCollider.enabled = false;

        playerScript.frontDetector.SetActive(false);
        playerScript.backDetector.SetActive(false);

        if (!startArrowAttack)
        {
            transform.localScale = new Vector2(theScale, transform.localScale.y);

            if (Vector2.Distance(point_B.position, mRigidBody.position) <= walkRange)
            {
                changeAnimationState(BOSS_WALK);
                speed = 3;
            }
            else
            {
                changeAnimationState(BOSS_RUN);
                speed = 4;
            }

            Vector2 target = new Vector2(point_B.position.x, mRigidBody.position.y);
            Vector2 newPos = Vector2.MoveTowards(mRigidBody.position, target, speed * Time.fixedDeltaTime);
            mRigidBody.MovePosition(newPos);

            if (transform.position.x >= point_B.position.x - 1.5f)
            {
                startArrowAttack = true;
            }
        }

        if (startArrowAttack)
        {
            LookAtPlayer();
            changeAnimationState(BOSS_SHOOT);
        }
    }

    public void winEvents()
    {
        UImanager.Instance.startWinWord();
        Audio.Instance.BGsound(Audio.Instance.win);
    }

    void runAttack()
    {
        playerScript.frontDetector.SetActive(false);
        playerScript.backDetector.SetActive(false);

        startArrowAttack = false;

        if (!startRunAttack)
        {
            mHitBox.player.canGetHurt = true;
            if (transform.position.x >= point_B.position.x - 1.5f)
            {
                startRunAttack = true;
                changeAnimationState(BOSS_IDLE);
            }
            else
            {
                startRunAttack = false;
                if (Vector2.Distance(point_B.position, mRigidBody.position) <= walkRange)
                {
                    changeAnimationState(BOSS_WALK);
                    speed = 2;
                }
                else
                {
                    changeAnimationState(BOSS_RUN);
                    speed = 4;
                }

                transform.localScale = new Vector2(theScale, transform.localScale.y);
                Vector2 target = new Vector2(point_B.position.x, mRigidBody.position.y);
                Vector2 newPos = Vector2.MoveTowards(mRigidBody.position, target, speed * Time.fixedDeltaTime);
                mRigidBody.MovePosition(newPos);
            }
        }

        if (startRunAttack)
        {
            runAttacking();
        }
    }

    void runAttacking()
    {
        bossCollider.enabled = true;

        count = 0;
        nextAttack = setNextAttack;
        numberOfAttacks = resetNumberOfAttacks;
        attacking = false;
        shouldCount = false;
        speed = 8;

        if (FaceRight)
        {
            if (!reachedDestination)
            {
                secondCount = 0;
                secondWaitTime = 2;
                firstCount = 0;
                firstWaitTime = 3;

                changeAnimationState(BOSS_CHARGE);
                {
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                    transform.localScale = new Vector2(theScale, transform.localScale.y);
                }
            }
        }
        else
        {
            if (!reachedDestination)
            {
                changeAnimationState(BOSS_CHARGE);
                {
                    transform.Translate(-Vector2.right * speed * Time.deltaTime);
                    transform.localScale = new Vector2(-theScale, transform.localScale.y);
                }
            }
        }

        if (transform.position.x <= point_A.position.x + 1.5f)
        {
            mHitBox.player.canGetHurt = true;

            reachedDestination = true;
            transform.localScale = new Vector2(theScale, transform.localScale.y);
            if (firstCount < firstWaitTime)
            {
                firstCount += Time.deltaTime;

            }

            if (firstCount >= firstWaitTime)
            {
                changeAnimationState(BOSS_CHARGE);

                secondCount += Time.deltaTime;
            }

            if (secondCount >= secondWaitTime)
            {
                FaceRight = true;
                isFlipped = false;
                reachedDestination = false;
            }

        }

        if (transform.position.x >= point_B.position.x - 1.5f)
        {
            mHitBox.player.canGetHurt = true;

            reachedDestination = true;
            
            transform.localScale = new Vector2(-theScale, transform.localScale.y);

            if (firstCount < firstWaitTime)
            {
                firstCount += Time.deltaTime;
                hasPlayedChargingSound = false;
                playLaughSound();
                changeAnimationState(BOSS_IDLE);
            }

            if (firstCount >= firstWaitTime)
            {
                changeAnimationState(BOSS_CHARGE);
                hasLaughed = false;
                secondCount += Time.deltaTime;
            }

            if (secondCount >= secondWaitTime)
            {
                FaceRight = false;
                isFlipped = true;
                reachedDestination = false;
            }
        }
    }

    public virtual void Damage()
    {
        if (canGetHurt)
        {
            UImanager.Instance.enemyHealthUpdate(life, fullHealth, lifeText);

            changeAnimationState(BOSS_HURT);
            life = life - 1;
        }
    }

    public void Death()
    {
        bossCollider.enabled = false;
        changeAnimationState(BOSS_DEATH);
        blockings.GetComponent<Collider2D>().enabled = false;
        blockings.GetComponent<Tilemap>().color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerHitBox")
        {
            Damage();
        }
        if(collision.tag == "Boss_B")
        {
            if (!runCounted)
            {
                numberOfRuns += 1;
                runCounted = true;
            }
        }

        if (collision.tag == "PlayersFront")
        {
            isEnemyAtMyBack = false;
        }
        if (collision.tag == "PlayersBack")
        {
            isEnemyAtMyBack = true;
        }
        if (collision.tag == "Boss_A")
        {
            runCounted = false;
        }
    }

    public void shoot()
    {
        numberOfShoots += 1;

        if (isFlipped)
        {
            Instantiate(arrow, gameObject.transform.position +
            new Vector3(-1f, 0, 0), Quaternion.identity);
        }
        if (!isFlipped)
        {
            Instantiate(arrow, gameObject.transform.position +
            new Vector3(+1f, 0, 0), Quaternion.identity);
        }
        
    }

    public void checkPlayerShield()
    {
        if (HitBox != null)
        {
            if (playerScript._isShieldActive == false)
            {
                HitBox.gameObject.SetActive(true);
            }

            if (playerScript._isShieldActive == true)
            {
                if (playerScript._isFlipped == true)
                {
                    if (isEnemyAtMyBack == false)
                    {
                        HitBox.gameObject.SetActive(false);
                    }
                    if (isEnemyAtMyBack == true)
                    {
                        HitBox.gameObject.SetActive(true);
                    }
                }
                if (playerScript._isFlipped == false)
                {
                    if (isEnemyAtMyBack == false)
                    {
                        HitBox.gameObject.SetActive(false);
                    }

                    if (isEnemyAtMyBack == true)
                    {
                        HitBox.gameObject.SetActive(true);
                    }
                }

            }
        }
    }

    public void playWalkSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(footstep);
        }
    }

    public void playDeathSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(death);
        }
    }

    public void playArrowShootSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(arrow_shoot);
        }
    }

    public void playChargingSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(charging);
        }
    }

    public void playSwordSwingSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(swordSwing);
        }
    }

    public void playHurtSound()
    {
        if (canPlaySound)
        {
            Audio.Instance.enemySound(hurt);
        }
    }

    public void playLaughSound()
    {
        if (canPlaySound)
        {
            if (!hasLaughed)
            {
                if(laughToPlay == 1)
                {
                    Audio.Instance.enemySound(laugh1);
                    hasLaughed = true;
                }
                if(laughToPlay == 2)
                {
                    Audio.Instance.enemySound(laugh2);
                    hasLaughed = true;
                }
                if (laughToPlay == 3)
                {
                    Audio.Instance.enemySound(laugh3);
                    hasLaughed = true;
                }
                if (laughToPlay == 4)
                {
                    Audio.Instance.enemySound(laugh4);
                    hasLaughed = true;
                }
            }
            if (hasLaughed)
            {
                laughToPlay = Random.Range(1, 5);
            }
        }
    }
}