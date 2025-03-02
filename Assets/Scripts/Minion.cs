using UnityEngine;
using UnityEngine.AI;

public class Minion : BaseEnemy
{
    public MeshRenderer attackMesh;

    protected override void Start()
    {
        base.Start();
        if (attackMesh != null)
            attackMesh.enabled = false;
    }

    // Sobrescribimos AttackEvent, que el Animator llamará
    public override void AttackEvent()
    {
        base.AttackEvent(); 
        if (attackMesh != null)
            attackMesh.enabled = true;

        // Desactiva el ataque luego de 0.2s
        Invoke(nameof(StopAttackMesh), 0.2f);
    }

    private void StopAttackMesh()
    {
        if (attackMesh != null)
            attackMesh.enabled = false;

        StopAttack(); 
    }
}
