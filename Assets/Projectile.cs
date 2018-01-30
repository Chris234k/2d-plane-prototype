using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 1f;

    Vector2 accel = new Vector2(1f, 0), vel;
    Vector2 gravity = new Vector2(0, -2f);
    float friction = 0.99f;

    public void Spawn(Vector2 dir, Vector2 vel)
    {
        accel = (200f * dir) + vel;
    }

    void Update()
    {
        accel += gravity;
        accel *= Time.deltaTime;

        vel += accel;
        vel *= friction;

        transform.position += (Vector3)vel;
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}