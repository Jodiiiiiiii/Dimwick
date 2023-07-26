using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{
    [Header("Motion")]
    public float MaxSpeed = 1f;
    public float MinSpeed = 0.5f;

    [Header("Enemy Prefabs")]
    public GameObject MeleeEnemy;
    public GameObject RangedEnemy;
    public GameObject TurretEnemy;
    [HideInInspector] public GameObject EnemiesParent;

    [Header("Lifespan")]
    public float Lifespan = 3f;

    [Header("Hit Stats")]
    public float HitstunTime = 0.4f;
    public float KnockbackSpeed = 6f;

    [HideInInspector] public Rigidbody2D Rb;

    private bool _isStopped = false;
    private float _lifespanTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();

        EnemiesParent = GameObject.Find("Enemies");
    }

    // Update is called once per frame
    void Update()
    {
        // update velocity
        if (_isStopped)
            Rb.velocity = Vector2.zero;
        else
            Rb.velocity = (Vector2)transform.right * Mathf.Lerp(MaxSpeed, MinSpeed, _lifespanTimer / Lifespan);

        // lifespan updates
        if (_lifespanTimer >= Lifespan)
        {
            float rand = Random.Range(0, 3);
            if (rand < 1)
                Instantiate(MeleeEnemy, transform.position, MeleeEnemy.transform.rotation, EnemiesParent.transform);
            else if (rand < 2)
                Instantiate(RangedEnemy, transform.position, RangedEnemy.transform.rotation, EnemiesParent.transform);
            else
                Instantiate(TurretEnemy, transform.position, TurretEnemy.transform.rotation, EnemiesParent.transform);

            Destroy(gameObject);
        }
        _lifespanTimer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Edge"))
            _isStopped = true;

        if (collision.CompareTag("LightBlast"))
            _lifespanTimer = Lifespan; // immediately burst the spawn projectile

    }
}
