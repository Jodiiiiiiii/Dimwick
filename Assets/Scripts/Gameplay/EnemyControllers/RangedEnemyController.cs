using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    [Header("Ranged Enemy - Projectiles")]
    public GameObject ProjectilePrefab;
    public float SpreadAngle = 3f;

    [Header("Ranged Enemy - Movement")]
    public float GoalSwapPeriod = 5f;
    public float MinGoalRange = 5f;
    public float MaxGoalRange = 9f;
    public float MinSpeed = 1.5f;
    public float MaxSpeed = 2f;
    [Tooltip("error threshold from range goal within which enemy switches to rotating instead of seeking")]
    public float GoalToleranceThreshold = 0.5f;

    // private variables
    private Vector2 _targetVelocity = Vector2.zero;
    private float _goalSwapTimer;
    private float _goalRange;
    private float _goalSpeed;
    private bool _clockwise;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _goalSwapTimer = GoalSwapPeriod;
        _goalRange = Random.Range(MinGoalRange, MaxGoalRange);
        _goalSpeed = Random.Range(MinSpeed, MaxSpeed);
        _clockwise = Random.Range(0f, 1f) > 0.5f ? true : false;
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
        // conduct goal swap every period
        _goalSwapTimer -= Time.deltaTime;
        if(_goalSwapTimer < 0f)
        {
            _goalSwapTimer = GoalSwapPeriod;
            _goalRange = Random.Range(MinGoalRange, MaxGoalRange);
            _goalSpeed = Random.Range(MinSpeed, MaxSpeed);
            _clockwise = Random.Range(0f, 1f) > 0.5f ? true : false;
        }

        float distance = Vector2.Distance(_player.transform.position, transform.position);
        Vector2 playerDirection = ((Vector2)_player.transform.position - (Vector2)transform.position).normalized;
        Vector2 clockwiseDirection = -1 * Vector2.Perpendicular(playerDirection);

        // generate target velocity
        // _targetVelocity = ((distance > _goalRange ? 1 : -1) * playerDirection + (_clockwise ? 1 : -1) * clockwiseDirection).normalized * _goalSpeed;
        _targetVelocity = Mathf.Abs(distance - _goalRange) > GoalToleranceThreshold ? 
            (distance > _goalRange ? 1 : -1) * playerDirection * _goalSpeed : (_clockwise ? 1 : -1) * clockwiseDirection * _goalSpeed;

        // update velocity based on target and current velocities
        Rb.velocity = Vector2.Lerp(Rb.velocity, _targetVelocity, 1 - Mathf.Exp(-MovementSharpness * Time.deltaTime));
    }
}
