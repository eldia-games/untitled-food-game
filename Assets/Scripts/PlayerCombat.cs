using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    public Animator _anim;
    private float _translationSpeed = 5f;
    private float _runningSpeed = 10f;

    private float cooldownAttack = 1.01f;
    private float cooldownSlide = 1f;
    private InputHandler _handler;
    private int moving;

    private bool attackAvailable = true;
    private bool slideAvailable = true;
    public new Camera camera;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _handler = GetComponent<InputHandler>();
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
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
        player.transform.LookAt(player.transform.position + new Vector3(_handler.input.x, 0, _handler.input.y));
        player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
        }


        //actions
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
        if(_handler.slide && slideAvailable)
        {
            _anim.SetTrigger("Slide");
            slideAvailable = false;
            StartCoroutine(SlideCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(cooldownAttack);
        attackAvailable = true;
    }

    IEnumerator SlideCooldown()
    {
        yield return new WaitForSeconds(cooldownSlide);
        slideAvailable = true;
    }
}
