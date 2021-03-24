using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class collectible : MonoBehaviour
{
    [SerializeField]
    Player player;

    Animator animator;

    [SerializeField]
    AudioClip collectibleSound;

    [SerializeField]
    GameObject spawnObject;

    Rigidbody2D mRigidbody2D;
    PolygonCollider2D mPolygonCollider2D;

    private void Start()
    {
        mRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        mPolygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
        if(gameObject.name == "Chest")
        {
            animator = GetComponent<Animator>();
        }
        if(gameObject.tag == "Collectible")
        {

        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.name == "Chest")
        {
            if(player != null)
            {
                if (Vector2.Distance(player.transform.position, gameObject.transform.position) <= 1)
                {
                    animator.SetTrigger("open");
                }
            }
        }
    }

    public void spawnCollectible()
    {
        Instantiate(spawnObject, new Vector3(transform.position.x, transform.position.y + 0.5f, 0), Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (player.Playerslive < 5)
            {
                player.increaseHealth();
                gameObject.SetActive(false);
                Audio.Instance.itemSound(collectibleSound);
                Destroy(gameObject, 3);
            }
        }

        if (collision.gameObject.tag == "Ground")
        {
            mRigidbody2D.bodyType = RigidbodyType2D.Static;
        }
    }

}
