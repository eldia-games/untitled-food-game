using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    #region "Variables publicas"

    [Header("Effects Config")]
    [Tooltip("Configures flash and shrink parameters via ScriptableObject")]
    public EnemyEffectsConfig effectsConfig;

    public Animator _anim;
    public PlayerStats PlayerStats;
    [Header("AfterImage Config")]
    [Tooltip("Material semitransparente para el efecto fantasma")]
    public Material afterImageMaterial;
    [Tooltip("Duración en segundos de cada afterimage antes de destruirse")]
    public float afterImageDuration = 0.5f;
    [Tooltip("Intervalo en segundos entre cada afterimage generado")]
    public float afterImageSpawnInterval = 0.1f;
    [Header("AfterImage Rainbow Settings")]
    [Tooltip("Velocidad a la que rota el color (vueltas por segundo)")]
    public float rainbowSpeed = 1f;

    #endregion

    #region "Variables privadas"

    private Coroutine afterImageCoroutine;
    private List<Material> flashMats = new List<Material>();
    private Vector3 originalScale;
    private Coroutine scaleRoutine;

    #endregion

    #region "Variables de configuración"

    //private Interactor _interactor;

    private float MovementSpeed => PlayerStats.MovementSpeed;
    private float StaminaSlide { get => PlayerStats.StaminaSlide; set => PlayerStats.StaminaSlide = value; }
    private float StaminaRegen => PlayerStats.StaminaRegen;
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

    #endregion
    private GameObject weaponType;
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region privateVariables

    private InputHandler _handler;
    private int moving;

    private int weaponIndexOld;
    private bool attackAvailable = true;

    private bool clickToAttack = false;

    private bool slideForce = false;

    private bool pushForce = false;

    private float pushForceFactor;

    private bool healCooldown = true;

    private bool interactAvailable = true;

    private bool invencibility = false;

    private bool lookAtMouse = false;

    public new Camera camera;
    public GameObject player;
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
        try
        {
            //Gamemanager is not null
            GameManager.Instance.gameObject.GetComponent<GameManager>();
            this.enabled = false;
        }
        catch(Exception)
        {
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
            Debug.Log("Error: " + e);
        }

        switch(weaponIndex)
        {
            case 2:
                //Bow
                weaponType = PlayerStats.weaponType[0];
                break;
            case 4:
                //Staff
                weaponType = PlayerStats.weaponType[1];
                break;
        }

        //weapons: 0 sword, 1 double axe, 2 bow, 3 mug, 4 staff, 5 none
        _anim.SetFloat("Weapon", weaponIndex);

        //VelAttack to animator
        _anim.SetFloat("VelAttack", velAttack);

        //VelSlide to animator
        _anim.SetFloat("VelSlide", velSlide);
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
            Debug.Log("Is player in main map? : " + e);
        }

        // Se guarda la escala inicial
        originalScale = transform.localScale;

        // Se recogen todos los Renderers del prefab
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            // Se clona el array de materiales
            var mats = r.materials;  // esto instancia cada material en el array
            r.materials = mats;      // re-asignamos para que use esas instancias

            // Se añaden todas esas instancias a nuestra lista
            flashMats.AddRange(mats);

            // Se forza emission keyword en cada material
            foreach (var m in mats)
                m.EnableKeyword("_EMISSION");
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
            //lookAtPosition = player.transform.position + transform.forward*_handler.input.y+ transform.right* _handler.input.x;
            lookAtDirection = (transform.forward*_handler.input.y+ transform.right* _handler.input.x).normalized;
            
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

            // Iniciar la corrutina para generar afterimages
            afterImageCoroutine = StartCoroutine(SpawnAfterImages());

            StartCoroutine(SlideForceCooldown());
            //addforce to dodge
            StartCoroutine(SlideCooldown());
        }

        //attack
        if (_handler.attack)
        {
            clickToAttack = true;
            //Fixed rotation player attack
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, player.transform.position);
            if (plane.Raycast(ray, out float distance))
            {
                mousePosition = ray.GetPoint(distance);
                //lookAtPosition = (mousePosition - player.transform.position).normalized + player.transform.position;
                lookAtDirection = (mousePosition - player.transform.position).normalized;
            }    
        }

        if (clickToAttack)
        {
            lookAtDirection = (mousePosition - player.transform.position).normalized;
            if (attackAvailable && lookAtMouse)
            {
                clickToAttack = false;
                attackAvailable = false;
                _anim.SetTrigger("Attack");

                StartCoroutine(AttackCooldown());
                OnAttack();
            }
            if(!lookAtMouse)
            {
                StartCoroutine(AttackWaitMouse());
            }
        }
        else
        {
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
            OnHeal();
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

            try{
                UIManager.Instance.RegenMana(Time.deltaTime * manaRegen);
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
        //Debug.Log(lookAtDirection);
        if(lookAtDirection.x!=0 || lookAtDirection.y!=0|| lookAtDirection.z!=0)
            RotatePlayerOverTimeToDirection(player, lookAtDirection, 10.0f);

    }

    IEnumerator AttackWaitMouse()
    {
        yield return new WaitForSeconds(0.3f);
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
            StaminaSlide = elapsedTime / velSlide * StaminaRegen;
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

        if (afterImageCoroutine != null)
            StopCoroutine(afterImageCoroutine);
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
        _anim.SetFloat("Weapon", weaponIndexOld);
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
        print("I am hurt");
    }

    void OnTriggerEnter(Collider collision)
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
            if(collision.gameObject.tag == "Boss")
            {
                //get the Enemy script from the object hit !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                
                Boss enemy = collision.gameObject.GetComponent<Boss>();
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
            if(collision.gameObject.tag == "Boss")
            {
                //get the Enemy script from the object hit !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                
                Boss enemy = collision.gameObject.GetComponent<Boss>();
                ////if the enemy script is not null, call the TakeDamage function from the enemy script
                if(enemy != null)
                {
                    enemy.OnHurt(damage * damageModifier, PushForce * pushModifier, transform.position);
                }
            }
        }
    }

    private IEnumerator FlashCoroutine()
    {
        float timer   = 0f;
        float maxPow  = effectsConfig.flashMaxPower;
        float duration = effectsConfig.flashDuration;

        // Begin at full power
        foreach (var m in flashMats)
            m.SetFloat("_FlashPower", maxPow);

        // Fade out
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = 1f - (timer / duration);
            float pow = t * maxPow;

            foreach (var m in flashMats)
                m.SetFloat("_FlashPower", pow);

            yield return null;
        }

        // Ensure off
        foreach (var m in flashMats)
            m.SetFloat("_FlashPower", 0f);
    }

    
    private IEnumerator ShrinkCoroutine()
    {
        float halfDur = effectsConfig.shrinkDuration * 0.5f;
        float factor  = Mathf.Clamp(effectsConfig.shrinkFactor, 0.1f, 1f);
        Vector3 targetScale = originalScale * factor;

        float timer = 0f;

        // Fase de encoger
        while (timer < halfDur)
        {
            timer += Time.deltaTime;
            float t = timer / halfDur;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Fase de volver
        timer = 0f;
        while (timer < halfDur)
        {
            timer += Time.deltaTime;
            float t = timer / halfDur;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
        scaleRoutine = null;
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

            if (scaleRoutine != null)
                StopCoroutine(scaleRoutine);
            scaleRoutine = StartCoroutine(ShrinkCoroutine());

            if (flashMats.Count > 0)
                StartCoroutine(FlashCoroutine());
            else
                Debug.LogWarning("No hay materials para hacer flash. ¿Renderers encontrados?");

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

    public void OnHeal()
    {
        bool beerFound = false;
        if(HP >= (float)maxLife)
        {
            print("You are full life, you dont need to heal");
            return;
        }
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

                    _anim.SetFloat("Weapon", 3); //Mug
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
                _anim.SetInteger("InteractionType", 0);
                break;
            case InteractionType.FirePlaceInteraction :
                _anim.SetTrigger("Interact");
                _anim.SetFloat("InteractionType", 1);
                StartCoroutine(InteractCooldown());
                break;

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

    private IEnumerator SpawnAfterImages()
    {
        // Mientras slideForce esté activo, generamos afterimages
        while (slideForce)
        {
            SpawnAfterImage();
            yield return new WaitForSeconds(afterImageSpawnInterval);
        }
    }

    #region "AfterImage"

    private void SpawnAfterImage()
    {
        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            // Bakeamos la malla
            Mesh bakedMesh = new Mesh();
            smr.BakeMesh(bakedMesh);

            // Creamos el GameObject fantasma
            GameObject ghost = new GameObject("AfterImage");
            ghost.transform.SetPositionAndRotation(smr.transform.position, smr.transform.rotation);
            ghost.transform.localScale = smr.transform.lossyScale;

            // MeshFilter
            var mf = ghost.AddComponent<MeshFilter>();
            mf.mesh = bakedMesh;

            // MeshRenderer + materiales tintados
            var mr = ghost.AddComponent<MeshRenderer>();
            var origMats = smr.sharedMaterials;
            var ghostMats = new Material[origMats.Length];

            // Calcula el color hue al momento actual
            float hue = Mathf.Repeat(Time.time * rainbowSpeed, 1f);
            Color tint = Color.HSVToRGB(hue, 1f, 1f);

            for (int i = 0; i < origMats.Length; i++)
            {
                var m = new Material(afterImageMaterial);
                if (origMats[i].HasProperty("_MainTex"))
                    m.mainTexture = origMats[i].mainTexture;
                // Conserva el alpha del material base
                Color baseCol = m.color;
                tint.a = baseCol.a;
                m.color = tint;
                ghostMats[i] = m;
            }
            mr.materials = ghostMats;

            // Lanza la corrutina de fade, que al final destruirá el fantasma
            StartCoroutine(FadeAndDestroyAfterImage(mr));
        }
    }

    private IEnumerator FadeAndDestroyAfterImage(MeshRenderer mr)
    {
        // Cacheamos el GameObject y los materiales 
        // (para que sobrevivan aunque el meshrenderer se destruya)
        GameObject ghostGO = mr.gameObject;
        Material[] mats = mr.materials;
        float elapsed = 0f;

        while (elapsed < afterImageDuration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(mats[0].color.a, 0f, elapsed / afterImageDuration);

            // Fade en cada material
            for (int i = 0; i < mats.Length; i++)
            {
                Color c = mats[i].color;
                c.a = a;
                mats[i].color = c;
            }

            yield return null;
        }

        // Al terminar el fade, destruimos el ghost
        Destroy(ghostGO);
    }

    #endregion


    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(mousePosition, 0.5f);
    }
}
