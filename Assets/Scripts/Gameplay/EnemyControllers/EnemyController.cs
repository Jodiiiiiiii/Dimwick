using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Sleep,
    Default,
    Hitstun
}

public abstract class EnemyController : MonoBehaviour
{
    [Header("HP")]
    public float MaxHP = 5f;

    [Header("Sleep")]
    public float WakeupRadius = 10f;

    [Header("Attacking")]
    [Tooltip("layers for checking player line of sight raycast")]
    public LayerMask RaycastLayermask;
    public float RaycastRadius = 0.5f;
    public float RaycastRange = 3f;
    public float MinAttackCooldown = 4f;
    public float MaxAttackCooldown = 6f;

    [Header("Hitstun")]
    [Tooltip("rate at which enemy reaches max speed (movement and knockback)")]
    public float MovementSharpness = 10f;
    [Tooltip("percentage of knockback that is ignored for turret enemy type")]
    public float KnockbackReductionFactor = 0.75f;

    // public and hidden
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] public Animator Anim;
    [HideInInspector] public FlashEffect FlashEffect;
    [HideInInspector] public EnemyState CurrentEnemyState = EnemyState.Sleep;

    // protected variables
    protected GameObject _player;
    private CapsuleCollider2D _playerCollider;
    // Animation States
    protected bool _isHitStunned = false;
    protected bool _isSleeping = true;
    protected bool _isAttacking = false;
    // private variables
    private float _hp;
    private float _hitStunTimer = 0f;
    private Vector2 _knockbackVelocity = Vector2.zero;
    private float _attackCooldownTimer;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        FlashEffect = GetComponent<FlashEffect>();

        _player = GameObject.Find("Dimwick");
        _playerCollider = _player.GetComponent<CapsuleCollider2D>();

        _hp = MaxHP;
        _attackCooldownTimer = MaxAttackCooldown; // start with max cooldown
    }

    #region ENEMY STATES
    public void TransitionToState(EnemyState newState)
    {
        EnemyState tmpInitialState = CurrentEnemyState;
        OnStateExit(tmpInitialState, newState);
        CurrentEnemyState = newState;
        OnStateEnter(newState, tmpInitialState);
    }

    public void OnStateEnter(EnemyState state, EnemyState fromState)
    {
        switch (state)
        {
            case EnemyState.Sleep:
                _isSleeping = true;
                break;
            case EnemyState.Default:
                break;
            case EnemyState.Hitstun:
                _isHitStunned = true;
                FlashEffect.StartFlash();
                break;
        }
    }

    public void OnStateExit(EnemyState state, EnemyState toState)
    {
        switch (state)
        {
            case EnemyState.Sleep:
                _isSleeping = false;
                break;
            case EnemyState.Default:
                break;
            case EnemyState.Hitstun:
                _isHitStunned = false;

                if(toState != EnemyState.Hitstun)
                    _knockbackVelocity = Vector2.zero; // reset knockback before next hit
                break;
        }
    }
    #endregion

    // Update is called once per frame
    protected virtual void Update()
    {
        switch (CurrentEnemyState)
        {
            case EnemyState.Sleep:

                // wake up if player within range
                if (Vector2.Distance(_player.transform.position, transform.position) < WakeupRadius)
                    TransitionToState(EnemyState.Default);

                break;
            case EnemyState.Default:

                if (_attackCooldownTimer > 0f)
                {
                    _attackCooldownTimer -= Time.deltaTime;
                }
                else
                {
                    // check for line of sight to player
                    RaycastHit2D hit = Physics2D.CircleCast(transform.position, RaycastRadius,
                        (Vector2)_player.transform.position + _playerCollider.offset - Vector2.up * _playerCollider.size.y - (Vector2)transform.position,
                        RaycastRange, RaycastLayermask);

                    // shows when enemy is attempting to lock on to player
                    Debug.DrawRay(transform.position,
                        ((Vector2)_player.transform.position + _playerCollider.offset - Vector2.up * _playerCollider.size.y - (Vector2)transform.position).normalized * RaycastRange,
                        Color.green);

                    if (hit.collider != null && hit.collider.CompareTag("Player"))
                    {
                        // create/start attack
                        Attack();

                        _attackCooldownTimer = Random.Range(MinAttackCooldown, MaxAttackCooldown); ; // put attack on cooldown
                        _isAttacking = true; // used for animation state
                    }
                }

                UpdateMovement();

                break;
            case EnemyState.Hitstun:

                #region HITSTUN HANDLING
                if (_hitStunTimer < 0f) // ends hitstun
                    TransitionToState(EnemyState.Default);
                else
                {
                    // no controls for movement or attacking

                    // update velocity based on target and knockback velocity
                    Rb.velocity = _knockbackVelocity; // no lerping (snappier)

                    _hitStunTimer -= Time.deltaTime;
                }
                #endregion

                break;
        }

        #region ANIMATION
        Anim.SetBool("isSleeping", _isSleeping);
        Anim.SetBool("isHitstun", _isHitStunned);
        if (_isAttacking)
        {
            Anim.SetTrigger("attack");
            _isAttacking = false;
        }
        else
        {
            Anim.ResetTrigger("attack");
        }
        #endregion

        // destroy object when dies (Replace with death animation??)
        if (_hp < 0f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerBullet") || collision.CompareTag("FlameSlash")) // flame slash uses trigger so it doesnt move
        {
            collision.gameObject.TryGetComponent<Projectile>(out Projectile projectile);
            if (projectile != null)
                ApplyHit(projectile);
            else
                Debug.LogError("Invalid player projectile collison");
        }
    }

    private void ApplyHit(Projectile projectile)
    {
        TransitionToState(EnemyState.Hitstun);

        // decrement hp
        _hp -= projectile.Damage;
        // start hitstun timer
        _hitStunTimer += projectile.HitstunTime;

        // set knockback based on bullet rotation and knockback stats
        Vector2 knockback = Quaternion.Euler(0, 0, projectile.transform.rotation.eulerAngles.z)
            * Vector2.right * projectile.KnockbackSpeed * (1 - KnockbackReductionFactor);
        _knockbackVelocity += knockback;
    }

    /// <summary>
    /// start attack (create projectile or update movement states)
    /// </summary>
    protected abstract void Attack();

    /// <summary>
    /// Update velocity of rigidbody
    /// </summary>
    protected abstract void UpdateMovement();
}
