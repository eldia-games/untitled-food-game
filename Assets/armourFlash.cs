using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armourFlash : BaseEnemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update() { }

    public override void SetPlayer(GameObject player) { }

    protected override void CheckPlayerVisibility() { }

    protected override void HandleCombat() { }

    protected override void HandlePatrol() { }

    protected override void ReturnToInitialPosition() { }

    protected override void StartAttack() { }

    public override void AttackEvent() { }

    public override void StopAttack() { }

    public override void AttackEndEvent() { }

    public override void AttackStartAnimationEvent() { }

    protected override void UpdateTimers() { }

    protected override void UpdateAnimations() { }

    public override void RotateTowards(Vector3 targetPos) { }

    public override void LookAt(Vector3 targetPos) { }

    public override void Die() { }

    public override void DestroyEnemy() { }

    public override void TauntStartEvent() { }

    public override void TauntEndEvent() { }

    public override void FootstepAnimationEvent() { }

    public override Vector3 PredictFuturePosition(float projectileSpeed) { return Vector3.zero; }

    protected override void OnDrawGizmosSelected() { }

    public override void OnGUI() { }

}
