using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float attackRange;
    public bool shouldCount = false;
    public float count;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected int life;
    public float nextAttack;
    public float attackRate;

    public Rigidbody2D EnemyRigidBody;
    protected SpriteRenderer EnemyRenderer;

    public Transform player;
    // Transforms to act as start and end markers for the journey.
    public Transform startMarker;
    public Transform endMarker;

    public Animator EnemyAnimation;

    [SerializeField]
    protected GameObject HitBox;

    public CapsuleCollider2D Collider;
    public Player playerScript;

    public bool isFlipped = false;
    public bool isEnemyAtMyBack;

    public enum EnemyMode { Idle, Attack, Die}

    public EnemyMode Mode;

    [SerializeField]
    public bool FaceRight = true;

    protected float theScale;

    [SerializeField]
    private GameObject spawnObject;

    private bool isDead;

    [SerializeField]
    protected float fullHealth;

    [SerializeField]
    public TMP_Text lifeText;

    private void Awake()
    {
        lifeText.gameObject.SetActive(true);
    }

    public virtual void Start()
    {
        UImanager.Instance.enemyHealthUpdate(life, fullHealth, lifeText);

        fullHealth = life;

        theScale = transform.localScale.x;

        Mode = EnemyMode.Idle;

        EnemyRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyRigidBody = this.GetComponent<Rigidbody2D>();
        EnemyAnimation = this.gameObject.GetComponent<Animator>();

        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Collider = gameObject.GetComponent<CapsuleCollider2D>();
    }


    public virtual void FixedUpdate()
    {
        determineMode();
        if (Mode == EnemyMode.Idle)
        {
            idleMode();
        }
        else if(Mode == EnemyMode.Attack)
        {
            attackMode();
        }
        else if (Mode == EnemyMode.Die)
        {
            Death();
        }
    }

    public void enableCanGetHurt()
    {
        playerScript.canGetHurt = true;
    }

    public virtual void Damage()
    {
        life = life - 1;
        EnemyAnimation.SetTrigger("hit");

        UImanager.Instance.enemyHealthUpdate(life, fullHealth, lifeText);
    }

    public virtual void Death()
    {
        lifeText.gameObject.SetActive(false);

        Collider.enabled = false;
        EnemyAnimation.SetTrigger("death");
        if (!isDead)
        {
            Boss.unlockBossPoints += 1;
            isDead = true;
        }
    }

    public virtual void idleMode()
    {
        lifeText.gameObject.SetActive(false);

        if (FaceRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            transform.localScale = new Vector2(theScale, transform.localScale.y);
        }
        else
        {
            transform.Translate(-Vector2.right * speed * Time.deltaTime);
            transform.localScale = new Vector2(-theScale, transform.localScale.y);
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

    public void attackMode()
    {
        lifeText.gameObject.SetActive(true);
        LookAtPlayer();
        moveTowardsPlayer();
    }

    public virtual void determineMode()
    {        
        if (life <= 0)
        {
            Mode = EnemyMode.Die;
        }
        else
        {
            if (player.position.x >= startMarker.position.x && player.position.x <= endMarker.position.x)
            {
                if (Vector2.Distance(player.position, EnemyRigidBody.position) <= 100)
                {
                    Mode = EnemyMode.Attack;
                }
                else
                {
                    Mode = EnemyMode.Idle;
                }
            }
            else
            {
                Mode = EnemyMode.Idle;
            }
        }

    }

    public virtual void LookAtPlayer()
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

    public void spawnCollectible()
    {
        Instantiate(spawnObject, new Vector3(transform.position.x, transform.position.y + 0.5f, 0), Quaternion.identity);
    }

    public virtual void moveTowardsPlayer()
    {
        if (Vector2.Distance(player.position, EnemyRigidBody.position) <= attackRange)
        {

            checkPlayerShield();

            EnemyAnimation.SetBool("walk", false);

            shouldCount = true;

            if (shouldCount)
            {
                count += Time.deltaTime;

                if(count > nextAttack - 1f)
                {
                    Collider.enabled = false;
                } 

                if (count > nextAttack)
                {
                    EnemyAnimation.SetTrigger("attack");

                    if(count > nextAttack + 2.5f)
                    {
                        Collider.enabled = true;
                        count = 0;
                        nextAttack = 5;
                    }
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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayersFront")
        {
            isEnemyAtMyBack = false;
        }
        if (collision.tag == "PlayersBack")
        {
            isEnemyAtMyBack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "PlayerHitBox")
        {
            Damage();
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
}
