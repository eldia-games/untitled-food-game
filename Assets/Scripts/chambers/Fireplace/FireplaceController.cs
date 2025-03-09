using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireplaceController : MonoBehaviour, IChamberController
{

    [SerializeField] private GameObject player;

    private bool healed = false;
    private int healLevel=0;




    public void initiallise(int level)
    {
        healLevel = 7 * level;
    }
    public void Start()
    {
        player.GetComponent<PlayerCombat>().enabled = true;
    }

    public void Heal(GameObject fireplace)
    {
        if (!healed)
        {
            healed = true;
            //healed.GetComponent<Animator>().SetBool("On", false); //apagar la hoguera
            //curar al jugador
            PlayerStatsController.Instance.healprct(healLevel);
            fireplace.GetComponent<Interactable>().Desactive();

        }
    }



}
