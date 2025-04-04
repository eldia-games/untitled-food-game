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
    private GameObject weaponType;
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

    private bool invencibility = false;

    private bool lookAtMouse = false;

    public new Camera camera;
    public GameObject player;
    private Vector3 lookAtPosition;
    private Vector3 lookAtDirection;

    private Vector3 pushDirection;

    private SphereCollider _colliderMeleeSpin;
    private BoxCollider _colliderMelee;
    private Interactor _interactor;

    private Rigidbody rb;

    private Vector3 mousePosition;

    public int NoGameManagerWeaponIndex = 0;

    #endregion


    // Start is called before the first frame update

    void Start()
    {
        try{
            //Gamemanager is not null
            GameManager.Instance.gameObject.GetComponent<GameManager>();
            this.enabled = false;
        }catch(Exception){
        }

        rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        _handler = GetComponent<InputHandler>();
        _interactor= GetComponent<Interactor>();
        camera = Camera.main;

        //HP = (float)maxLife;
        MP = (float)maxMana;
        try{
            weaponIndex = GameManager.Instance.getCurrentWeaponType();
        }
        catch(Exception e){
            weaponIndex = NoGameManagerWeaponIndex;
        }
        if(weaponIndex == 2)
            weaponType = PlayerStats.weaponType[0];
        if(weaponIndex == 4)
            weaponType = PlayerStats.weaponType[1];

        //weapons: 0 sword, 1 double axe, 2 bow, 3 mug, 4 staff, 5 none
        _anim.SetFloat("Weapon", weaponIndex);
        _anim.SetInteger("weaponType", weaponIndex);
        try{
        UIManager.Instance.SetMaxHealth(maxLife);
        UIManager.Instance.SetMaxMana(maxMana);

        UIManager.Instance.SetHealth(HP);
        UIManager.Instance.SetMana(MP);
        }
        catch(Exception e){
            Debug.Log("Error: " + e);
        }
        StaminaSlide = 10;
        _colliderMeleeSpin = player.GetComponent<SphereCollider>();
        _colliderMeleeSpin.enabled = false;
        _colliderMelee = player.GetComponent<BoxCollider>();
        _colliderMelee.enabled = false;
        try{
            InventoryManager.Instance.setPlayer(player);
        }
        catch(Exception e){
            Debug.Log("Error: " + e);
        }
    }

    void Update()
    {
        //If player is on air (ray cast) set gravity to gravity, else set to 10
        Vector3 offset = new Vector3(0, 0.5f, 0.5f);
        //rotate offset based on player direction
        offset = player.transform.TransformDirection(offset);
        if (Physics.Raycast(transform.position + offset, Vector3.down, 0.6f))
        {
            Debug.DrawRay(transform.position + offset, Vector3.down, Color.red);
            Physics.gravity = new Vector3(0,-5,0);
            //Debug.Log("Grounded");
            
        }
        else
        {
            Debug.DrawRay(transform.position + offset, Vector3.down, Color.green);
            Physics.gravity = new Vector3(0, -200, 0);
            //Debug.Log("Not Grounded");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Weapons

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
        
        // Adjust movement direction based on the rotation of the gameobject
        Vector3 movementDirection = new Vector3(_handler.input.x, 0, _handler.input.y).normalized;
        Vector3 adjustedMovement = transform.TransformDirection(movementDirection) * MovementSpeed;
        rb.velocity = adjustedMovement;
        //Vector3 movement = new Vector3(_handler.input.x, 0, _handler.input.y).normalized * MovementSpeed;
        //rb.velocity = movement;
        //transform.Translate(Vector3.forward * (_handler.input.y * Time.deltaTime * MovementSpeed));
        //transform.Translate(Vector3.right * (_handler.input.x * Time.deltaTime * MovementSpeed));


        //player rotate to wasd
        if (attackAvailable && moving ==1)
        {
            player.transform.position = transform.position;
            lookAtPosition = player.transform.position + transform.forward*_handler.input.y+ transform.right* _handler.input.x;
            lookAtDirection = (lookAtPosition - player.transform.position).normalized;
            
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Actions

        //slide
        if (_handler.slide && StaminaSlide == 10)
        {
            invencibility = true;
            _anim.SetTrigger("Slide");
            StaminaSlide = 0;
            slideForce = true;
            StartCoroutine(SlideForceCooldown());
            //addforce to dodge
            StartCoroutine(SlideCooldown());
        }

        //attack
        if (_handler.attack && attackAvailable)
        {
            // Asignar lookAtMouse a true inmediatamente para disparar la animación sin demora
            lookAtMouse = true;

            // Rotación fija para ataque utilizando la posición del ratón
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, player.transform.position);
            if (plane.Raycast(ray, out float distance))
            {
                mousePosition = ray.GetPoint(distance);
                lookAtPosition = (mousePosition - player.transform.position).normalized + player.transform.position;
                lookAtDirection = (mousePosition - player.transform.position).normalized;
            }
            
            _anim.SetTrigger("Attack");
            StartCoroutine(AttackCooldown());
            OnAttack();
        }
        else if (!_handler.attack)
        {
            // Reinicia lookAtMouse cuando no se mantiene el input de ataque
            lookAtMouse = false;
        }

        //Make the slide movement
        if (slideForce)
        {
            //print("sliding");
            movementDirection = new Vector3(_handler.input.x, 0, _handler.input.y).normalized;
            adjustedMovement = transform.TransformDirection(movementDirection) * MovementSpeed * 2.0f;
            rb.velocity = adjustedMovement;
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
            MP += Time.deltaTime * manaRegen / 10;

            try{
                UIManager.Instance.RegenMana(Time.deltaTime * manaRegen / 10);
            }
            catch(Exception e){
                Debug.Log("Error: " + e);
            }
        }

        //push force
        if (pushForce)
        {
            //print("getting pushed");
            transform.Translate(pushDirection * Time.deltaTime * pushForceFactor);
        }

        //RotatePlayerOverTime(player, lookAtPosition, 10.0f);
        RotatePlayerOverTimeToDirection(player, lookAtDirection, 10.0f);

    }

    IEnumerator AttackWaitMouse()
    {
        yield return new WaitForSeconds(1f);
        lookAtMouse = true;
    }

    private string RotatePlayerOverTime(GameObject player, Vector3 lookAtPosition, float v)
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - player.transform.position);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, v * Time.deltaTime);
        player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
        return null;
    }

    private string RotatePlayerOverTimeToDirection(GameObject player, Vector3 lookAtDirection, float v)
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookAtDirection);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, v * Time.deltaTime);
        player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
        return null;
    }


    IEnumerator InteractCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        interactAvailable = true;
    }

    IEnumerator AttackCooldown()
    {
        attackAvailable = false;
        yield return new WaitForSeconds(1 / velAttack);
        attackAvailable = true;
        _colliderMeleeSpin.enabled = false;
        _colliderMelee.enabled = false;
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
        invencibility = false;
    }

    IEnumerator PushForceCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        pushForce = false;
    }

    IEnumerator HurtCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        invencibility = false;
    }

    IEnumerator HealCooldown()
    {
        //NOT NEADED HEAL VELOCITY
        yield return new WaitForSeconds(1.0f);
        weaponIndex = weaponIndexOld;
        _anim.SetFloat("Weapon", weaponIndex);
        _anim.SetInteger("weaponType", weaponIndex);
        healCooldown = true;
    }

    IEnumerator DeadCooldown()
    {
        yield return new WaitForSeconds(4.0f);
        try{
            UIManager.Instance.ShowEndGameCanvas();
        }
        catch(Exception e){
            Debug.Log("Error: " + e);
        }
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
                try{
                    UIManager.Instance.LoseMana(manaCost);
                }
                catch(Exception e){
                    Debug.Log("Error: " + e);
                }
                CreateBullet();
            }
            else
            {
                //Ventana emergente no suficiente mana
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
        if (_colliderMeleeSpin != null && _colliderMeleeSpin.enabled)
        {
            if(collision.gameObject.tag == "Enemy")
            {
                //get the Enemy script from the object hit !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                
                BaseEnemy enemy = collision.gameObject.GetComponent<BaseEnemy>();
                ////if the enemy script is not null, call the TakeDamage function from the enemy script
                if(enemy != null)
                {
                    enemy.OnHurt(damage * damageModifier, PushForce * pushModifier, transform.position);
                }
            }
        }
        if (_colliderMelee != null && _colliderMelee.enabled)
        {
            if(collision.gameObject.tag == "Enemy")
            {
                //get the Enemy script from the object hit !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            
                BaseEnemy enemy = collision.gameObject.GetComponent<BaseEnemy>();
                ////if the enemy script is not null, call the TakeDamage function from the enemy script
                if(enemy != null)
                {
                    enemy.OnHurt(damage * damageModifier, PushForce * pushModifier, transform.position);
                }
            }
        }
    }


    public void OnHurt(float damage, float pushForce, Vector3 position)
    {
        print("hurt");
        //Make player take damage if not in invincibility
        if(!invencibility)
        {
            invencibility = true;
            HP -= damage;
            try{
                UIManager.Instance.LoseHealth(damage);
            }
            catch(Exception e){
                Debug.Log("Error: " + e);
            }
            _anim.SetTrigger("Hurt");
            _anim.SetFloat("HP", HP);
            TakePush(pushForce, position);
            StartCoroutine(HurtCooldown());
            //Audio take damage sound
            //AudioManager.Instance.PlaySound("PlayerHurt", transform.position);
        }
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
        bool beerFound = false;
        //Search in the scene for the inventory manager
        for(int i = 0; i < InventoryManager.Instance.items.Count; i++)
        {
            if (InventoryManager.Instance.items[i].item.itemName == "Beer")
            {
                beerFound = true;
                //Remove the item from the inventory
                //InventoryManager.Instance.UseItem(InventoryManager.Instance.items[i].item, 1);
                InventoryManager.Instance.UseItem(InventoryManager.Instance.items[i].item, 1);

                healCooldown = false;
                print("heal");
                //Make player heal
                HP += heal;

                UIManager.Instance.GainHealth(heal);

                weaponIndexOld = weaponIndex;
                weaponIndex = 3;
                _anim.SetFloat("Weapon", weaponIndex);
                _anim.SetInteger("weaponType", weaponIndex);
                _anim.SetTrigger("Attack");
                _anim.SetFloat("HP", HP);
                StartCoroutine(HealCooldown());
                break;
            }   
        }
        if (!beerFound)
        {
            print("You need a beer to heal");
        }
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
        StartCoroutine(DeadCooldown());
        invencibility = true;
        //activar mensaje o cutscene de muerte
    }

    private void CreateBullet()
    {
        // Utiliza la dirección calculada a partir del ratón
        Vector3 shootDirection = lookAtDirection;
        
        // (Opcional) Actualiza la rotación del jugador instantáneamente para que apunte al objetivo
        player.transform.rotation = Quaternion.LookRotation(shootDirection);
        
        // Calcula la rotación para la bala
        Quaternion bulletRotation = Quaternion.Euler(-90, Quaternion.LookRotation(shootDirection).eulerAngles.y, 0);
        
        // Instancia la bala en la posición actual, con un pequeño offset en la dirección de disparo
        var bullet = Instantiate(
            weaponType,
            transform.position + shootDirection * 2 + transform.up,
            bulletRotation);
        
        // Configura las propiedades de la bala
        bullet.GetComponent<Bullet>().damage = (int)damage;
        bullet.GetComponent<Bullet>().pushForce = PushForce;
        bullet.GetComponent<Bullet>().damageModifier = damageModifier;
        bullet.GetComponent<Bullet>().pushModifier = pushModifier;

        // Destruye la bala después de 5 segundos
        Destroy(bullet, 5.0f);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(mousePosition, 0.5f);
    }
}
