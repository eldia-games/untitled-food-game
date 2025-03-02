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
        print("hit");
        //check if the object hit has the tag "Enemy" or "Player"
        if(hitInfo.tag == "Enemy")
        {
            //get the Enemy script from the object hit !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            
            //Enemy enemy = hitInfo.GetComponent<Enemy>();
            ////if the enemy script is not null, call the TakeDamage function from the enemy script
            //if(enemy != null)
            //{
            //    enemy.OnHurt(damage * damageModifier, pushForce * pushModifier);
            //}
        }
        else if(hitInfo.tag == "Player")
        {
            //get the Player script from the object hit
            PlayerCombat player = hitInfo.GetComponent<PlayerCombat>();
            //if the player script is not null, call the OnHurt function from the player script
            if(player != null)
            {
                player.OnHurt(damage * damageModifier, pushForce * pushModifier, transform.position);
            }
        }
        //destroy the bullet
        Destroy(gameObject);

    }
}
