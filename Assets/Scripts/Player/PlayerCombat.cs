using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    public Animator _anim;

    public PlayerStats PlayerStats;

    //private Interactor _interactor;

    private float MovementSpeed => PlayerStats.MovementSpeed;
    private float StaminaSlide { get => PlayerStats.StaminaSlide; set => PlayerStats.StaminaSlide = value; }
    private float velSlide => PlayerStats.velSlide;
    private int maxLife => PlayerStats.maxLife;
    private float heal => PlayerStats.heal;
    private float HP { get => PlayerStats.HP; set => PlayerStats.HP = value; }
    private float damage => PlayerStats.damage;
    private float velAttack => PlayerStats.velAttack;
    private float PushForce => PlayerStats.pushForce;
    private float damageModifier => PlayerStats.damageModifier;
    private float pushModifier => PlayerStats.pushModifier;
    private int maxMana => PlayerStats.maxMana;
    private float MP { get => PlayerStats.MP; set => PlayerStats.MP = value; }
    private float manaCost => PlayerStats.manaCost;
    private float manaRegen => PlayerStats.manaRegen;
    private int weaponIndex { get => PlayerStats.weaponIndex; set => PlayerStats.weaponIndex = value; }
    private GameObject weaponType => PlayerStats.weaponType;
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region privateVariables

    private InputHandler _handler;
    private int moving;

    private int weaponIndexOld;
    private bool attackAvailable = true;

    private bool slideForce = false;

    private bool pushForce = false;

    private float pushForceFactor;

    private bool healCooldown = true;

    private bool interactAvailable = true;

    public new Camera camera;
    public GameObject player;
    private Vector3 lookAtPosition;

    private Vector3 pushDirection;

    private SphereCollider _colliderMeleeSpin;
    private BoxCollider _colliderMelee;
    private Interactor _interactor;

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _handler = GetComponent<InputHandler>();
        _interactor= GetComponent<Interactor>();
        camera = Camera.main;

        //HP = (float)maxLife;
        MP = (float)maxMana;
        StaminaSlide = 10;
        _colliderMeleeSpin = player.GetComponent<SphereCollider>();
        _colliderMeleeSpin.enabled = false;
        _colliderMelee = player.GetComponent<BoxCollider>();
        _colliderMelee.enabled = false;
        this.enabled = false;
        InventoryManager.Instance.setPlayer(player);
    }

    void Update()
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Weapons

        //weapons: 0 axe, 1 double axe, 2 bow, 3 mug, 4 staff, 5 none
        _anim.SetFloat("Weapon", weaponIndex);

        //VelAttack to animator
        _anim.SetFloat("VelAttack", velAttack);

        //VelSlide to animator
        _anim.SetFloat("VelSlide", velSlide);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Movement

        //player movement to wasd
        if (_handler.input.x > 0.1f || _handler.input.y > 0.1f || _handler.input.x < -0.1f || _handler.input.y < -0.1f)
            moving = 1;
        else
            moving = 0;
        _anim.SetFloat("Moving", moving);
        
        transform.Translate(Vector3.forward * (_handler.input.y * Time.deltaTime * MovementSpeed));
        transform.Translate(Vector3.right * (_handler.input.x * Time.deltaTime * MovementSpeed));
        //player rotate to wasd
        if (attackAvailable)
        {
            player.transform.position = transform.position;
            lookAtPosition = player.transform.position + transform.forward*_handler.input.y+ transform.right* _handler.input.x;
            
            player.transform.LookAt(lookAtPosition);
            player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Actions

        //slide
        if (_handler.slide && StaminaSlide == 10)
        {
            _anim.SetTrigger("Slide");
            StaminaSlide = 0;
            slideForce = true;
            StartCoroutine(SlideForceCooldown());
            //addforce to dodge
            StartCoroutine(SlideCooldown());
        }

        //attack
        if (_handler.attack)
        {

            Vector3 mousePosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
            //print(mousePosition);
            player.transform.LookAt(mousePosition);
            player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);

            if (attackAvailable)
            {
                attackAvailable = false;
                _anim.SetTrigger("Attack");

                StartCoroutine(AttackCooldown());
                OnAttack();
            }
        }

        //Make the slide movement
        if (slideForce)
        {
            //print("sliding");
            transform.Translate(-transform.forward * (_handler.input.y * Time.deltaTime * MovementSpeed) * 2f);
            transform.Translate(-transform.right * (_handler.input.x * Time.deltaTime * MovementSpeed) * 2f);
        }

        //Interact
        if (_handler.interact && interactAvailable)
        {
            print("interacting");
            //interact with objects
            onInteract();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///HP and MP

        //if heal
        if (_handler.heal && healCooldown && HP < (float)maxLife)
        {
            OnHeal(heal);
        }

        //if HP <= 0 die
        _anim.SetFloat("HP", HP);
        if (HP <= 0)
        {
            OnDie();
        }

        //if MP <= 0 MP = 0 manaregen
        if (MP <= 0)
        {
            MP = 0;
        }
        else if (MP >= maxMana)
        {
            MP = maxMana;
        }
        else
        {
            MP += Time.deltaTime * manaRegen;
        }

        //push force
        if (pushForce)
        {
            //print("getting pushed");
            transform.Translate(pushDirection * Time.deltaTime * pushForceFactor);
        }

    }

    IEnumerator InteractCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        interactAvailable = true;
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1 / velAttack);
        attackAvailable = true;
        _colliderMeleeSpin.enabled = false;
    }
    IEnumerator SlideCooldown()
    {
        float elapsedTime = 0f;
        while (elapsedTime < velSlide)
        {
            StaminaSlide = elapsedTime / velSlide * 10;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        StaminaSlide = 10;
    }
    IEnumerator SlideForceCooldown()
    {
        yield return new WaitForSeconds(0.2f * (1 / velSlide));
        slideForce = false;
    }

    IEnumerator PushForceCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        pushForce = false;
    }

    IEnumerator HealCooldown()
    {
        //NOT NEADED HEAL VELOCITY
        yield return new WaitForSeconds(1.0f);
        weaponIndex = weaponIndexOld;
        _anim.SetFloat("Weapon", weaponIndex);
        healCooldown = true;
    }

    public void OnAttack()
    {
        //print("attacking");
        //Make enemies take damage if they are in range
        //depends on the weapon to use the correct damage and mp cost if magic
        //force to push enemies away

        //Bow Attack of player
        if (weaponIndex == 2)
        {
            CreateBullet();
        }
        else if (weaponIndex == 4)
        {
            //Magic attack of player
            if (MP >= manaCost)
            {
                MP -= manaCost;
                CreateBullet();
            }
        }
        else if(weaponIndex == 1)
        {
            //Close attack of player spin
            _colliderMeleeSpin.enabled = true;

        }
        else if(weaponIndex == 0)
        {
            //Close attack of player melee
            _colliderMelee.enabled = true;
        } 
    }

    void OnCollisionEnter(Collision collision)
    {
        //If the collider is melee, make damage to the enemy
        if ( _colliderMeleeSpin.enabled)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                //collision.gameObject.GetComponent<Enemy>().OnHurt(damage, PushForce, transform.position);
            }
        }
        if ( _colliderMelee.enabled)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                //collision.gameObject.GetComponent<Enemy>().OnHurt(damage, PushForce, transform.position);
            }
        }
    }


    public void OnHurt(float damage, float pushForce, Vector3 position)
    {
        print("hurt");
        //Make player take damage
        HP -= damage;
        _anim.SetTrigger("Hurt");
        _anim.SetFloat("HP", HP);
        TakePush(pushForce, position);
    }

    private void TakePush(float Force, Vector3 position)
    {
        //Make player take push force
        pushForce = true;
        StartCoroutine(PushForceCooldown());
        pushForceFactor = Force;
        pushDirection = transform.position - position;
        //ignore y axis
        pushDirection.y = 0;
        pushDirection.Normalize();
    }

    public void OnHeal(float heal)
    {
        healCooldown = false;
        print("heal");
        //Make player heal
        HP += heal;
        weaponIndexOld = weaponIndex;
        weaponIndex = 3;
        _anim.SetFloat("Weapon", weaponIndex);
        _anim.SetTrigger("Attack");
        _anim.SetFloat("HP", HP);
        StartCoroutine(HealCooldown());
    }

    public void onInteract()
    {
        interactAvailable = false;
        //Only activate Interact on getInteract if the object is interactable
        switch (_interactor.GetInteractionType()) {
            case InteractionType.None:
                break;

            case InteractionType.NormalInteraction :
                _anim.SetTrigger("Interact");
                break;
            //case InteractionType.FirePlaceInteraction :
            //    _anim.SetTrigger("Interact");
            //    StartCoroutine(InteractCooldown());
            //    break;

        }
        StartCoroutine(InteractCooldown());
        _interactor.interact();
        //interact with objects}

    }

    private void OnDie()
    {
        print("You died");
        //desactivar el script de movimiento y el de input
        enabled = false;
        _handler.enabled = false;
        //activar mensaje o cutscene de muerte
    }

    private void CreateBullet()
    {
        // Create the Bullet from the Bullet Prefab
        //Move the bullet in front of the player
        //rotate bullet -90 x and direction of the player
        var bullet = (GameObject)Instantiate(
            weaponType,
            transform.position + player.transform.forward * 2 + transform.up,
            Quaternion.Euler(-90, player.transform.eulerAngles.y, 0));

        bullet.GetComponent<Bullet>().damage = (int)damage;
        bullet.GetComponent<Bullet>().pushForce = PushForce;
        bullet.GetComponent<Bullet>().damageModifier = damageModifier;
        bullet.GetComponent<Bullet>().pushModifier = pushModifier;

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

}
