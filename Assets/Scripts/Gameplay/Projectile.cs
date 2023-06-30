using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    [Header("Motion")]
    public float MaxSpeed = 1f;
    public float MinSpeed = 0.5f;

    [Header("Light")]
    public Light2D Light;
    public float MaxLightIntensity = 0.5f;
    public float MinLightIntensity = 0f;
    public float MaxLightRange = 1.0f;
    public float MinLightRange = 0f;

    [Header("Hit Stats")]
    public bool DestroyOnCollision = false;
    public float HitstunTime = 2f;
    public float KnockbackSpeed = 1f;
    [Tooltip("only matters for player projectiles (since player has discrete health)")]
    public float Damage = 1f;

    [Header("Misc")]
    public float Lifespan = 5f;
    

    // private variables
    private Rigidbody2D _rb;
    private float _lifespanTimer = 0f;
    private bool _stopped = false;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // update lighting
        Light.intensity = Mathf.Lerp(MaxLightIntensity, MinLightIntensity, _lifespanTimer / Lifespan);
        Light.pointLightOuterRadius = Mathf.Lerp(MaxLightRange, MinLightRange, _lifespanTimer / Lifespan);

        // update velocity
        if (_stopped)
            _rb.velocity = Vector2.zero;
        else
            _rb.velocity = (Vector2) transform.right * Mathf.Lerp(MaxSpeed, MinSpeed, _lifespanTimer / Lifespan);

        // lifespan updates
        if (_lifespanTimer > Lifespan)
            Destroy(gameObject);
        _lifespanTimer += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _stopped = true;

        // if destroy on collision or opposite projectiles collided
        if (DestroyOnCollision 
            || (gameObject.CompareTag("PlayerBullet") && (collision.collider.CompareTag("EnemyBullet") || collision.collider.CompareTag("Enemy"))) 
            || (gameObject.CompareTag("EnemyBullet") && collision.collider.CompareTag("PlayerBullet")))
            Destroy(gameObject);
    }
}
