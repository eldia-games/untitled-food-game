using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    public Animator _anim;
    private float _translationSpeed = 5f;
    private float _runningSpeed = 10f;
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
        if(_handler.attack && attackAvailable)
        {
            attackAvailable = false;
            Vector3 mousePosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
            //print(mousePosition);
            player.transform.LookAt(mousePosition);
            player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
        
            _anim.SetTrigger("Attack");
            
            StartCoroutine(AttackCooldown());
        }
        if(_handler.slide && slideAvailable)
        {
            _anim.SetTrigger("Slide");
            slideAvailable = false;
            StartCoroutine(SlideCooldown());
        }
        print(_handler.input);
        _anim.SetFloat("Xaxis",_handler.input.x, 0.2f, Time.deltaTime);
            
        _anim.SetFloat("Zaxis",_handler.input.y, 0.1f, Time.deltaTime);
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        attackAvailable = true;
    }

    IEnumerator SlideCooldown()
    {
        yield return new WaitForSeconds(1f);
        slideAvailable = true;
    }
}
