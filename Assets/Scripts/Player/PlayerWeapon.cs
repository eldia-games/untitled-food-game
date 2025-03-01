using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public GameObject axe1hand;
    public GameObject axe2hand;
    public GameObject crossbow;
    public GameObject mug;
    public GameObject staff;
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        axe1hand.GetComponent<MeshRenderer>().enabled = false;
        axe2hand.GetComponent<MeshRenderer>().enabled = false;
        mug.GetComponent<MeshRenderer>().enabled = false;
        crossbow.GetComponent<MeshRenderer>().enabled = false;
        staff.GetComponent<MeshRenderer>().enabled = false;
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        int weapon = (int)_anim.GetFloat("Weapon");

        //weapons: 0 axe, 1 double axe, 2 bow, 3 mug, 4 staff, 5 none
        switch(weapon)
        {
            case 0:
                axe1hand.GetComponent<MeshRenderer>().enabled = true;
                axe2hand.GetComponent<MeshRenderer>().enabled = false;
                mug.GetComponent<MeshRenderer>().enabled = false;
                crossbow.GetComponent<MeshRenderer>().enabled = false;
                staff.GetComponent<MeshRenderer>().enabled = false;
                break;
            case 1:
                axe1hand.GetComponent<MeshRenderer>().enabled = false;
                axe2hand.GetComponent<MeshRenderer>().enabled = true;
                mug.GetComponent<MeshRenderer>().enabled = false;
                crossbow.GetComponent<MeshRenderer>().enabled = false;
                staff.GetComponent<MeshRenderer>().enabled = false;
                break;
            case 2:
                axe1hand.GetComponent<MeshRenderer>().enabled = false;
                axe2hand.GetComponent<MeshRenderer>().enabled = false;
                mug.GetComponent<MeshRenderer>().enabled = false;
                crossbow.GetComponent<MeshRenderer>().enabled = true;
                staff.GetComponent<MeshRenderer>().enabled = false;
                break;
            case 3:
                axe1hand.GetComponent<MeshRenderer>().enabled = false;
                axe2hand.GetComponent<MeshRenderer>().enabled = false;
                mug.GetComponent<MeshRenderer>().enabled = true;
                crossbow.GetComponent<MeshRenderer>().enabled = false;
                staff.GetComponent<MeshRenderer>().enabled = false;
                break;
            case 4:
                axe1hand.GetComponent<MeshRenderer>().enabled = false;
                axe2hand.GetComponent<MeshRenderer>().enabled = false;
                mug.GetComponent<MeshRenderer>().enabled = false;
                crossbow.GetComponent<MeshRenderer>().enabled = false;
                staff.GetComponent<MeshRenderer>().enabled = true;
                break;
            case 5:
                axe1hand.GetComponent<MeshRenderer>().enabled = false;
                axe2hand.GetComponent<MeshRenderer>().enabled = false;
                mug.GetComponent<MeshRenderer>().enabled = false;
                crossbow.GetComponent<MeshRenderer>().enabled = false;
                staff.GetComponent<MeshRenderer>().enabled = false;
                break;
        }
    }
}
