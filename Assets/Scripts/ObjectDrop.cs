using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrop : MonoBehaviour
{
    public Items item;

    public Collider hitbox;

    void OnTriggerEnter(Collider hitInfo)
    {
        if(hitInfo.tag == "Player")
        {
            print("To inventory");
            hitInfo.gameObject.GetComponent<InventoryManager>().AddItem(item,item.quantity, item.stackeable);  
            Destroy(gameObject);
        }

    }

}
