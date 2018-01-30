using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    int dir_multi = 1;
    public Vector3 move_delta = new Vector3(0.1f, 0, 0);

    void Update()
    {
        transform.position += move_delta * dir_multi;
    }

    void OnBecameInvisible()
    {
        dir_multi *= -1;
    }
}