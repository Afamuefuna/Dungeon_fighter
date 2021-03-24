using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Player player;

    [SerializeField]
    bool released, goingLeft, goingRight, hasAlreadyHit;

    SpriteRenderer renderer;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        hasAlreadyHit = false;
        Destroy(this.gameObject, 5.0f);
    }

    private void FixedUpdate()
    {
        if (Boss.isFlipped)
        {
            if (!released)
            {
                released = true;
                goingLeft = true;
            }
        }
        if (!Boss.isFlipped)
        {
            if (!released)
            {
                released = true;
                goingRight = true;
            }
        }
        if (goingRight && !goingLeft)
        {
            transform.position += Vector3.right * 10 * Time.deltaTime;
            goingLeft = false;
            renderer.flipX = false;
        }

        if (goingLeft && !goingRight)
        {
            transform.position += Vector3.left * 10 * Time.deltaTime;
            goingRight = false;
            renderer.flipX = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!hasAlreadyHit)
            {
                player.damage();
                hasAlreadyHit = true;

                Destroy(gameObject);
            }
        }
    }
}
