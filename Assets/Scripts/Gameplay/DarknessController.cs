using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DarknessController : MonoBehaviour
{
    [Header("Sleep")]
    public float WakeupRange = 15f;
    // boss meter to show when boss wakes up
    public GameObject BossMeter;

    [Header("HP")]
    public float MaxHP = 200f;
    [Tooltip("percent of health healed by heal 'attack'")]
    public float HealRatio = 0.1f;
    [Tooltip("period over which an individual heal takes place")]
    public float HealDuration = 2f;

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

    [Header("Victory Transition")]
    public Light2D GlobalLight;
    public float VictoryTransitionTime = 4f;
    public float LightIncreaseRate = 3f;
    [Tooltip("to be disabled during victory")]
    public Light2D DarkSpotLight;
    [HideInInspector] public GameObject EnemiesParent;

    [HideInInspector] public Animator Anim;
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] private GameObject _player;

    private float _hp;
    // movement
    private Vector2 _targetVelocity = Vector2.zero;
    private float _goalSwapTimer;
    private bool _dashReady = false;
    // attacking
    private float _attackCooldownTimer;
    // healing
    private float _healTimer = 0f;
    // victory transition
    private bool _isVictoryTransition = false;
    private float _victoryTransitionTimer = 0f;
    // sleep state
    private bool _isSleeping = true;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
        _player = GameObject.Find("Dimwick");
        EnemiesParent = GameObject.Find("Enemies");

        _hp = MaxHP;

        _goalSwapTimer = GoalSwapPeriod;
        _attackCooldownTimer = Random.Range(MinAttackCooldown, MaxAttackCooldown);

        BossMeter.SetActive(false); // inactive by default
    }

    // Update is called once per frame
    void Update()
    {
        if(_isSleeping)
        {
            if (Vector2.Distance(_player.transform.position, transform.position) < WakeupRange)
            {
                _isSleeping = false;
                BossMeter.SetActive(true);
            }
        }
        else // AWAKE
        {
            if (_isVictoryTransition)
            {
                GlobalLight.intensity += Time.deltaTime * LightIncreaseRate;

                if (_victoryTransitionTimer < 0f)
                {
                    SceneManager.LoadScene("Start"); // REPLACE WITH VICTORY SCREEN
                }

                _victoryTransitionTimer -= Time.deltaTime;
            }
            else
            {
                #region MOVEMENT
                if (_goalSwapTimer <= 0) // set new target
                {
                    if (_dashReady)
                    {
                        Vector2 playerDirection = ((Vector2)_player.transform.position - (Vector2)transform.position).normalized;
                        _targetVelocity = playerDirection * DashMoveSpeed;
                        _goalSwapTimer = DashDuration;
                        _dashReady = false;
                    }
                    else // default movemenet
                    {
                        float randAngle = Random.Range(0f, 360f);
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
                    float rand = Random.Range(0, 4);
                    if (rand < 1) // spawn attack
                    {
                        // replace later with random of attack options
                        Instantiate(SpawnProjectile, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                    }
                    else if (rand < 2) // darkness attack
                    {
                        Anim.SetTrigger("darkWave");
                    }
                    else if (rand < 3) // dash attack
                    {
                        _dashReady = true;
                    }
                    else // heal attack
                    {
                        _healTimer = HealDuration;
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

                #region HEALING
                if (_healTimer > 0f)
                {
                    _hp += HealRatio * MaxHP * Time.deltaTime / HealDuration;
                    if (_hp > MaxHP) _hp = MaxHP; // cap at max HP
                    _healTimer -= Time.deltaTime;
                }
                #endregion

                if (_hp <= 0)
                {
                    _isVictoryTransition = true;
                    Destroy(EnemiesParent);
                    _victoryTransitionTimer = VictoryTransitionTime;
                    DarkSpotLight.intensity = 0f;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet") || collision.CompareTag("FlameSlash")) // flame slash uses trigger so it doesnt move
        {
            collision.gameObject.TryGetComponent<Projectile>(out Projectile projectile);
            if (projectile != null)
            {
                _hp -= projectile.Damage;
                Destroy(collision.gameObject);
            }
            else
                Debug.LogError("Invalid player projectile collison");
        }
    }

    #region PUBLIC GETTERS
    public float GetHPRatio()
    {
        return _hp / MaxHP;
    }
    #endregion
}
