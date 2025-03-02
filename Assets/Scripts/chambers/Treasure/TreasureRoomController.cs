using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoomController : MonoBehaviour, IChamberController
{

    [SerializeField] private List<GameObject> chests;
    private bool openedChest = false;



 

    public void initiallise(int level)
    {
        //generar los items de los cofres
    }

    public void OpenChest(GameObject chest)
    {
        if (!openedChest)
        {
            openedChest = true;
            chest.GetComponent<Animator>().SetBool("isClosed", false);
            for (int i = 0; i < chests.Count;i++)
            {
                chest.GetComponent<Interactable>().Desactive();
            }
            //spawnear el item

        }
    }
}
