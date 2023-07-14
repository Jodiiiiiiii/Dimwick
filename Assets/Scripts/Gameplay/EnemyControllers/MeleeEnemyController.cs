using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    [Header("Melee Enemy - Roll")]
    public float RollSpeed = 6f;
    public float RollDuration = 1f;

    [Header("Melee Enemy - Movement")]
    public float GoalSwapPeriod = 5f;
    public float MinSpeed = 2f;
    public float MaxSpeed = 3f;
    [Tooltip("ratio for influence of rotation component of movement vs direct player tracking")]
    public float RotationFactor = 0.2f;

    // private variables
    private Vector2 _targetVelocity = Vector2.zero;
    // rolling
    private bool _isRolling = false;
    private float _rollingTimer;
    // movement
    private float _goalSwapTimer;
    private float _goalSpeed;
    private bool _clockwise;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // rolling
        _rollingTimer = RollDuration;
        // movement
        _goalSwapTimer = GoalSwapPeriod;
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
        // set roll state
        _isRolling = true;
        _rollingTimer = RollDuration;
        _targetVelocity = ((Vector2)_player.GetComponent<PlayerController>().GetAimPivotPosition() - (Vector2)transform.position).normalized * RollSpeed;
    }

    protected override void UpdateMovement()
    {
        if(_isRolling)
        {
            // handle rolling timer
            if (_rollingTimer < 0)
                _isRolling = false;
            else
                _rollingTimer -= Time.deltaTime;
        }
        else // standard (non-rolling) motion
        {
            // conduct goal swap every period
            _goalSwapTimer -= Time.deltaTime;
            if (_goalSwapTimer < 0f)
            {
                _goalSwapTimer = GoalSwapPeriod;
                _goalSpeed = Random.Range(MinSpeed, MaxSpeed);
                _clockwise = Random.Range(0f, 1f) > 0.5f ? true : false;
            }

            Vector2 playerDirection = ((Vector2)_player.GetComponent<PlayerController>().GetAimPivotPosition() - (Vector2)transform.position).normalized;
            Vector2 clockwiseDirection = -1 * Vector2.Perpendicular(playerDirection);

            // generate target velocity
            _targetVelocity = (playerDirection + RotationFactor * (_clockwise ? 1 : -1) * clockwiseDirection) * _goalSpeed;
        }

        // update velocity based on target and current velocities
        Rb.velocity = Vector2.Lerp(Rb.velocity, _targetVelocity, 1 - Mathf.Exp(-MovementSharpness * Time.deltaTime));
    }
}
