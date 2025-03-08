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
        int type = persistenceInstance.getType();
        if (type <= generators.Length)
        {
            generators[type].createChamber(persistenceInstance.getLevel());
        }



    }
}
