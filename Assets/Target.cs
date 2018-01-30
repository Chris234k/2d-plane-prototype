using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 4f;

    Vector3 start_scale;
    Vector3 tween_scale;

    void Start()
    {
        start_scale = transform.localScale;
        tween_scale = start_scale * 1.2f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Projectile proj = collision.gameObject.GetComponent<Projectile>();

        if ( proj != null )
        {
            health -= proj.damage;

            StopAllCoroutines();
            transform.localScale = start_scale;
            StartCoroutine(Hit());

            Destroy(proj.gameObject);
        }
    }

    IEnumerator Hit()
    {
        float time = 0.1f;
        float elapsed_time = 0;

        while ( transform.localScale.x < tween_scale.x )
        {
            elapsed_time += Time.deltaTime;
            float t = elapsed_time / time;

            float scale = Mathf.Lerp(start_scale.x, tween_scale.x, t);
            transform.localScale = Vector3.one * scale; // Uniform scaling

            yield return new WaitForEndOfFrame();
        }

        transform.localScale = start_scale * 2;
        elapsed_time = 0;

        while ( transform.localScale.x > start_scale.x )
        {
            elapsed_time += Time.deltaTime;
            float t = elapsed_time / time;

            float scale = Mathf.Lerp(tween_scale.x, start_scale.x, t);
            transform.localScale = Vector3.one * scale;

            yield return new WaitForEndOfFrame();
        }

        transform.localScale = start_scale;
    }
}