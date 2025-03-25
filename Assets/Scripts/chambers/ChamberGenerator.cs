using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberGenerator : MonoBehaviour
{

    private GameManager _GameManager;
    
    
    void Start()
    {
        _GameManager = GameManager.Instance;
    
        TypeChamberGenerator[] generators = this.GetComponents<TypeChamberGenerator>();
        RoomType type = _GameManager.room;
        int level = _GameManager.tile.x + _GameManager.tile.y;
        for (int i = 0; i < generators.Length; i++)
        {
            if (type == generators[i].getChamberType())
            {
                generators[i].createChamber(level);
                return;
            }
        }
    
    
    
    
    }

    //private persistence persistenceobject;
    //void Start()
    //{
    //    persistenceobject = persistence.Instance;
    //
    //    TypeChamberGenerator[] generators = this.GetComponents<TypeChamberGenerator>();
    //    RoomType type = persistenceobject.getType();
    //    int level = persistenceobject.getLevel();
    //    for (int i = 0; i < generators.Length; i++)
    //    {
    //        if (type == generators[i].getChamberType())
    //        {
    //            generators[i].createChamber(level);
    //            return;
    //        }
    //    }
    //
    //
    //
    //
    //}
}
