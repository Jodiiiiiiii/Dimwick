using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    [Header("Ranged Enemy - Projectiles")]
    public GameObject ProjectilePrefab;
    public float SpreadAngle = 3f;

    [Header("Ranged Enemy - Movement")]
    public float MinGoalRange = 5f;
    public float MaxGoalRange = 9f;
    public float MinSpeed = 1.5f;
    public float MaxSpeed = 2f;

    // private variables
    private Vector2 _targetVelocity = Vector2.zero;
    private float _goalRange;
    private float _goalSpeed;
    private bool _clockwise;

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

    protected override void Attack()
    {
        Instantiate(ProjectilePrefab, transform.position, Quaternion.Euler(0, 0,
            Vector2.SignedAngle(Vector2.right, (Vector2)_player.transform.position - (Vector2)transform.position)
            + Random.Range(-SpreadAngle, SpreadAngle)));
    }

    protected override void UpdateMovement()
    {
        // update velocity based on target and current velocities
        Rb.velocity = Vector2.Lerp(Rb.velocity, _targetVelocity, 1 - Mathf.Exp(-MovementSharpness * Time.deltaTime));
    }
}
