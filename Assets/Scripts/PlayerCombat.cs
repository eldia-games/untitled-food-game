using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    public Animator _anim;
    private float _translationSpeed = 5f;
    private float cooldownAttack = 1.01f;
    private float cooldownSlide = 1.0f;
    private InputHandler _handler;
    private int moving;

    private bool attackAvailable = true;

    [Range(0,10)]
    public float SlideMP = 10;

    [Range(0,100)]
    public float HP = 100;
    private bool slideForce = false;
    public new Camera camera;
    public GameObject player;
    private Vector3 lookAtPosition;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _handler = GetComponent<InputHandler>();
        camera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        //print(_handler.input);
        
        //player movement to wasd
        if(_handler.input.x>0.1f || _handler.input.y>0.1f || _handler.input.x<-0.1f || _handler.input.y<-0.1f)
            moving = 1;
        else 
            moving = 0;
        _anim.SetFloat("Moving", moving);

        transform.Translate(Vector3.forward * (_handler.input.y * Time.deltaTime * _translationSpeed));
        transform.Translate(Vector3.right * (_handler.input.x * Time.deltaTime * _translationSpeed));

        //player rotate to wasd
        if(attackAvailable)
        {
        lookAtPosition = player.transform.position + new Vector3(_handler.input.x, 0, _handler.input.y);
        player.transform.LookAt(lookAtPosition);
        player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
        }

        //if hurt
        //_anim.SetTrigger("Hurt", ...);

        //if HP <= 0 die
        _anim.SetFloat("HP",HP);
        if(HP<=0)
        {
            OnDie();
        }

        //actions
        //slide
        if(_handler.slide && SlideMP==10)
        {
            _anim.SetTrigger("Slide");
            SlideMP = 0;
            slideForce = true;
            StartCoroutine(SlideForce());
            //addforce to dodge
            StartCoroutine(SlideCooldown());
        }
        
        //attack
        if(_handler.attack)
                {
                    
                    Vector3 mousePosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
                    //print(mousePosition);
                    player.transform.LookAt(mousePosition);
                    player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
                
                    if(attackAvailable)
                    {
                        attackAvailable = false;
                        _anim.SetTrigger("Attack");
                    
                        StartCoroutine(AttackCooldown());
                    }
                }

        //Make the slide movement
        if(slideForce)
        {
            //print("sliding");
            transform.Translate(Vector3.forward * (_handler.input.y * Time.deltaTime * _translationSpeed) * 2f);
            transform.Translate(Vector3.right * (_handler.input.x * Time.deltaTime * _translationSpeed) * 2f);
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(cooldownAttack);
        attackAvailable = true;
    }
    IEnumerator SlideCooldown()
    {
        float elapsedTime = 0f;
        while (elapsedTime < cooldownSlide)
        {
            SlideMP = elapsedTime / cooldownSlide * 10;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SlideMP = 10;
    }
    IEnumerator SlideForce()
    {
        yield return new WaitForSeconds(0.2f);
        slideForce = false;
    }

    public void OnAttack()
    {

    }

    public void OnHurt()
    {

    }

    public void OnHeal()
    {
        
    }

    public void OnDie()
    {
        print("You died");
        //desactivar el script de movimiento y el de input
        enabled = false;
        _handler.enabled = false;
    }
    
}
