using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessController : MonoBehaviour
{
    [Header("HP")]
    public float MaxHP = 5f;

    [Header("Attacking")]
    public float MinAttackCooldown = 3f;
    public float MaxAttackCooldown = 8f;

    [Header("Projectile Attack")]
    public GameObject SpawnProjectile;

    [HideInInspector] public Animator Anim;

    private float _attackCooldownTimer;
    private bool _isDashing = false;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();

        _attackCooldownTimer = Random.Range(MinAttackCooldown, MaxAttackCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        #region MOVEMENT
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
                _isDashing = true;
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
