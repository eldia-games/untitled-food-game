using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberGenerator : MonoBehaviour
{
    //private  List generators;
    private persistence persistenceInstance;

    
    void Start()
    {
        persistenceInstance = persistence.Instance;

        TypeChamberGenerator[] generators = this.GetComponents<TypeChamberGenerator>();
        RoomType type = persistenceInstance.getType();
        for (int i = 0; i < generators.Length; i++)
        {
            if (type == generators[i].getChamberType())
            {
                generators[i].createChamber(persistenceInstance.getLevel());
            }
        }




    }
}
