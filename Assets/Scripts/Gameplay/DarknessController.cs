using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessController : MonoBehaviour
{
    [Header("HP")]
    public float MaxHP = 5f;

    [Header("Movement")]
    public float GoalSwapPeriod = 5f;
    public float BaseMoveSpeed = 2f;
    public float DashDuration = 1.5f;
    public float DashMoveSpeed = 8f;
    public float MovementSharpness = 10f;

    [Header("Attacking")]
    public float MinAttackCooldown = 3f;
    public float MaxAttackCooldown = 8f;

    [Header("Projectile Attack")]
    public GameObject SpawnProjectile;

    [HideInInspector] public Animator Anim;
    [HideInInspector] public Rigidbody2D Rb;

    private Vector2 _targetVelocity = Vector2.zero;
    private float _goalSwapTimer;
    private float _attackCooldownTimer;
    private bool _dashReady = false;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();

        _goalSwapTimer = GoalSwapPeriod;
        _attackCooldownTimer = Random.Range(MinAttackCooldown, MaxAttackCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        #region MOVEMENT
        if(_goalSwapTimer <= 0) // set new target
        {
            float randAngle = Random.Range(0f, 360f);
            if (_dashReady)
            {
                _targetVelocity = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle)) * DashMoveSpeed;
                _goalSwapTimer = DashDuration;
                _dashReady = false;
            }
            else // default movemenet
            {
                _targetVelocity = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle)) * BaseMoveSpeed;
                _goalSwapTimer = GoalSwapPeriod;
            }
        }
        else
        {
            _goalSwapTimer -= Time.deltaTime;
        }

        Rb.velocity = Vector2.Lerp(Rb.velocity, _targetVelocity, 1 - Mathf.Exp(-MovementSharpness * Time.deltaTime));
        #endregion

        #region ATTACKING
        if (_attackCooldownTimer < 0)
        {
            float rand = Random.Range(0, 3);
            if (rand < 1) // spawn attack
            {
                // replace later with random of attack options
                Instantiate(SpawnProjectile, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            }
            else if (rand < 2) // darkness attack
            {
                Anim.SetTrigger("darkWave");
            }
            else // dash attack
            {
                _dashReady = true;
            }

            // restart attack cooldown
            _attackCooldownTimer = Random.Range(MinAttackCooldown, MaxAttackCooldown);
        }
        else
        {
            _attackCooldownTimer -= Time.deltaTime;

            Anim.ResetTrigger("darkWave");
        }
        #endregion
    }
}
