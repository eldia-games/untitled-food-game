using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PowerUpStats", menuName = "PowerUpStats")]
public class PowerUpStats : ScriptableObject
{
    #region InputVariables

    [Header("Movement")]
    //movement
    public float MovementSpeed;

    [Header("Stamina")]
    //Stamina
    [Range(0, 10)]
    public float stamina;


    [Header("HP")]
    //HP
    public int maxLife;



    [Header("Damage")]
    //Damage
    public float damage;

    //VelAttack
    public float velAttack;

    //PushForce
    public float pushForce ;


    [Header("MP")]
    //MP
    public int maxMana ;

    

   

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }


}
