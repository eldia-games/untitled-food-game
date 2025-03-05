using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectDrop : MonoBehaviour
{
    public Items item;

    public int indexLoot;
    public Collider hitbox;

    void OnTriggerEnter(Collider hitInfo)
    {
        if(hitInfo.tag == "Player")
        {
            print("To inventory");
            hitInfo.gameObject.GetComponent<InventoryManager>().AddItem(item, indexLoot, item.quantity, item.stackeable);  
            Destroy(gameObject);
        }

    }

}
