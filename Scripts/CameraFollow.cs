using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public Player player;

    public float smoothspeed = 0.125f;
    public Vector3 offset;

    private void FixedUpdate()
    {
        if(target != null)
        {
            Vector3 desiredPosition = target.position + offset;

            Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothspeed);
            transform.position = smoothPosition;
        }
    }
}
