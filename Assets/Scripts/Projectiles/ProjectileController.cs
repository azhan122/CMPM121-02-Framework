using UnityEngine;
using System;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    public float lifetime;
    public event Action<Hittable,Vector3> OnHit;
    public ProjectileMovement movement;
    public bool piercing; // If piercing property is true/false
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.shotnum += 1;
    }

    // Update is called once per frame
    void Update()
    {
        movement.Movement(transform);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("projectile")) return;
        if (collision.gameObject.CompareTag("unit"))
        {
            var ec = collision.gameObject.GetComponent<EnemyController>();
            if (ec != null)
            {
                OnHit(ec.hp, transform.position);
                if (piercing) // Alyssa: If piercing active, do not despawn
                {
                    return;
                }
            }
            else
            {
                var pc = collision.gameObject.GetComponent<PlayerController>();
                if (pc != null)
                {
                    OnHit(pc.hp, transform.position);
                }
                // Alyssa: Put piercing check here if there will be enemies you want that can do piercing damage
            }
        }
        Destroy(gameObject);
    }

    public void SetScale(float scale)
    {
        var col = GetComponent<Collider2D>();
        if (col is CircleCollider2D circle)
        {
            circle.radius *= scale;
        }
        else if (col is BoxCollider2D box)
        {
            box.size *= scale;
        }
    }

    public void SetLifetime(float lifetime)
    {
        StartCoroutine(Expire(lifetime));
    }

    IEnumerator Expire(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
