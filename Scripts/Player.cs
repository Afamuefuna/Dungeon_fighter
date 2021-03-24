using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public SpriteRenderer playerSpriteRenderer;
    Rigidbody2D _rigidBody;
    public new Animation animation;

    public bool _canMove;
    public bool _jump = false;
    public bool _isShieldActive = false;
    public bool _isFlipped;
    [SerializeField]
    public bool isGrounded, canGetHurt;

    [SerializeField]
    public float speed;
    float theScale;
    public int Playerslive = 4;
    public GameObject[] livesImage;
 
    public float nextJump;

    public GameObject frontDetector, backDetector;

    public Vector2 jumpHeight;

    [SerializeField]
    private float timeToPlayJump;
    public float count;

    [SerializeField]
    private GameObject ground;

    [SerializeField]
    private bool canSave;

    float move;

    [SerializeField]
    private Collider2D playerCollider;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.bodyType = RigidbodyType2D.Dynamic;
        _canMove = true;
        theScale = transform.localScale.x;

        Physics2D.IgnoreCollision(ground.GetComponent<Collider2D>(), frontDetector.GetComponent<Collider2D>(), true);
        Physics2D.IgnoreCollision(ground.GetComponent<Collider2D>(), backDetector.GetComponent<Collider2D>(), true);
    }

    private void Update()
    {
        if (!UImanager.Instance.isOnDialogue)
        {
            jump(GetComponent<Animator>());
            jumpOnFall();
            attack();
            shield();
        }
    }

    public void jump(Animator animator)
    {
        //add upArrow when I switch to PC build
        //remove Jump comment when I switch to PC

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded || animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                playJumpSound();
                _rigidBody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                animation.playerJump(true);
            }
        }
    }

    public void jumpOnFall()
    {
        if (!isGrounded)
        {
            count += Time.deltaTime;

            if (count > timeToPlayJump)
            {
                animation.playerJump(true);
            }
        }
    }

    public void Dead()
    {
        Boss.unlockBossPoints = 0;
        GameManager.Instance.loadScene("Death");
    }

    void FixedUpdate()
    {
        if (!UImanager.Instance.isOnDialogue)
        {
            movement();
            _rigidBody.bodyType = RigidbodyType2D.Dynamic;
        }
        else if(UImanager.Instance.isOnDialogue && isGrounded)
        {
            _rigidBody.bodyType = RigidbodyType2D.Static;
            move = 0f;
            animation.move(move);
        }
    }

    public void movement()
    {
        if (_canMove == true)
        {
            //change from CrossPlatormInput... to Input when building for PC
            move = Input.GetAxisRaw("Horizontal");

            if (isGrounded)
            {
                animation.move(move);
            }
            _rigidBody.velocity = new Vector2(move * speed, _rigidBody.velocity.y);

            if (move > 0)
            {
                _isFlipped = false;
                transform.localScale = new Vector2(theScale, transform.localScale.y);
            }
            else if (move < 0)
            {
                _isFlipped = true;
                transform.localScale = new Vector2(-theScale, transform.localScale.y);
            }
        }
    }

    public void attack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animation.playerAttack();

            if (animation._PlayerAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                speed = 3;
            }
        }
    }

    public void damage()
    {
        Playerslive = Playerslive - 1;

        for (int i = 0; i <= Playerslive; i++)
        {
            livesImage[Playerslive].GetComponent<Image>().enabled = false;
        }

        if (Playerslive <= 0)
        {
            animation.death();
        }

        else
        {
            animation.PlayerHit();
        }
    }

    public void increaseHealth()
    {
        for (int i = 0; i <= Playerslive; i++)
        {
            livesImage[Playerslive].GetComponent<Image>().enabled = true;
        }
        Playerslive = Playerslive + 1;
    }

    public void shield()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _isShieldActive = true;
            animation.playerShield(true);
            speed = 0;
        }
        else if(Input.GetKeyUp(KeyCode.DownArrow))
        {
            speed = 3;
            _isShieldActive = false;
            animation.playerShield(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyHitBox")
        {
            if (canGetHurt)
            {
                damage();
            }
        }
        if (collision.tag == "save")
        {
            canSave = true;
            if (canSave)
            {
                UImanager.Instance.startSaveWord();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            speed = 3;
            animation.playerJump(false);
            if (count > timeToPlayJump)
            {
                playLaunchSound();
            }
            count = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Spikes")
        {
            if(Playerslive > 0)
            {
                damage();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "save")
        {
            canSave = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    public void playRunSound()
    {
        Audio.Instance.playerSound(Audio.Instance.Run);
    }

    public void playJumpSound()
    {
        Audio.Instance.playerSound(Audio.Instance.Jump);
    }

    public void playLaunchSound()
    {
        Audio.Instance.playerSound(Audio.Instance.launch);
    }

    public void playAttackSound()
    {
        int randomPitch = Random.Range(1, 4);
        Audio.Instance.audioSources[0].pitch = randomPitch;
        Audio.Instance.playerSound(Audio.Instance.Attack);
    }

    public void resetPitch()
    {
        Audio.Instance.audioSources[0].pitch = 1;
    }

    public void playHurtSound()
    {
        Audio.Instance.playerSound(Audio.Instance.Hit);
    }
}
