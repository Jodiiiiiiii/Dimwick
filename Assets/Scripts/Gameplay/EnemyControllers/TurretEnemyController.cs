using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemyController : EnemyController
{
    [Header("Turret Enemy Projectiles")]
    public GameObject ProjectilePrefab;
    public float SpreadAngle = 3f;

    // private variables
    private Vector2 _targetVelocity = Vector2.zero;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void UpdateMovement()
    {
        // turret enemy does not move freely on its own
        _targetVelocity = Vector2.zero;

        // update velocity based on target and current velocities
        Rb.velocity = Vector2.Lerp(Rb.velocity, _targetVelocity, 1 - Mathf.Exp(-KnockbackMovementSharpness * Time.deltaTime));
    }

    protected override void Attack()
    {
        Instantiate(ProjectilePrefab, transform.position, Quaternion.Euler(0, 0,
            Vector2.SignedAngle(Vector2.right, (Vector2)_player.transform.position - (Vector2)transform.position)
            + Random.Range(-SpreadAngle, SpreadAngle)));
    }
}
