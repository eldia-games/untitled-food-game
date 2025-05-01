using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PowerUpDrop : Spawneable
{
    // Start is called before the first frame update
   
   
    public powerUpType type;

    void OnTriggerEnter(Collider hitInfo)
    {
        if (hitInfo.tag == "Player")
        {

            PowerUpStatsController.Instance.PowerUp(type);
            playSound();
            Destroy(gameObject);
            //UIManager.Instance.ResetPlayerHealthMana();
        }

    }


}
