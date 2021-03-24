using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser : MonoBehaviour
{
    Enemy enemy;
    Player player;

    [SerializeField]
    bool released, goingLeft, goingRight, hasAlreadyHit;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        enemy = GameObject.FindGameObjectWithTag("Monster").GetComponent<Enemy>();
    }

    private void Start()
    {
        hasAlreadyHit = false;
        Destroy(this.gameObject, 5.0f);
    }

    private void FixedUpdate()
    {
        if (enemy.FaceRight && !released)
        {
            goingRight = true;
            released = true;
        }

        if (!enemy.FaceRight && !released)
        {
            goingLeft = true;
            released = true;
        }

        if (goingRight && !goingLeft)
        {
            transform.position += Vector3.right * 4.0f * Time.deltaTime;
            goingLeft = false;            
        }

        if (goingLeft && !goingRight)
        {
            transform.position += Vector3.left * 4.0f * Time.deltaTime;
            goingRight = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (!hasAlreadyHit)
            {
                player.damage();
                hasAlreadyHit = true;
            }
        }
    }
}
