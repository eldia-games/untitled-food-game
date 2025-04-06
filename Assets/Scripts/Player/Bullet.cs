using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;

    public int damage = 1;

    public float pushForce = 1;

    public Rigidbody rb;

    public Collider hitbox;

    public float damageModifier = 1;

    public float pushModifier = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = -transform.up * speed;
    }

    void OnTriggerEnter(Collider hitInfo)
    {
        if(hitInfo.tag == "Enemy")
        {
            BaseEnemy enemy = hitInfo.GetComponent<BaseEnemy>();
            if(enemy != null)
            {
                enemy.OnHurt(damage * damageModifier, pushForce * pushModifier, transform.position);
                Destroy(this);
            }
        } else if(hitInfo.tag == "Boss")
        {
            Boss enemy = hitInfo.GetComponent<Boss>();
            if(enemy != null)
            {
                enemy.OnHurt(damage * damageModifier, pushForce * pushModifier, transform.position);
                Destroy(this);
            }
        }
        else if(hitInfo.tag == "Wall")
        {
            Destroy(this);
        }
        else {
            
        }
    }
}
