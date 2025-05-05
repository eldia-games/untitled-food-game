using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectDrop : Spawneable
{
    public Items item;

    public int indexLoot;
    public int quantity=0;

    void OnTriggerEnter(Collider hitInfo)
    {
        if(hitInfo.tag == "Player")
        {
            print("To inventory");
            InventoryList.Instance.AddItem(item, indexLoot, quantity!=0?quantity:item.quantity, item.stackeable);
            playSound();
            Destroy(gameObject);
        }

    }


}
