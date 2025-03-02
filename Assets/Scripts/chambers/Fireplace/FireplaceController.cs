using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireplaceController : MonoBehaviour, IChamberController
{

    private bool healed = false;
    private int healLevel=0;




    public void initiallise(int level)
    {
        healLevel = 10 * level;
    }

    public void Heal(GameObject fireplace)
    {
        if (!healed)
        {
            healed = true;
            //healed.GetComponent<Animator>().SetBool("On", false); //apagar la hoguera
            //curar al jugador
            fireplace.GetComponent<Interactable>().Desactive();

        }
    }



}
