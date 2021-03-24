using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterfall : MonoBehaviour
{
    public AudioSource source;

    private void Start()
    {
        source = gameObject.GetComponentInParent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            source.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            source.Pause();
        }
    }


}
