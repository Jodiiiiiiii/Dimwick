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
    [Tooltip("-1 means motion never stops")]
    public float StopMotionAfter = -1f;

    [Header("Fading")]
    public float FadePeriod = 1f;

    // private variables
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private float _lifespanTimer = 0f;
    private bool _stopped = false;
    private bool _isFading = false;
    private float _fadingTimer;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        TryGetComponent(out _collider);
        TryGetComponent(out _spriteRenderer);
    }

    // Update is called once per frame
    void Update()
    {
        // fading
        if (_isFading)
        {
            // destroy at end of fade
            if (_fadingTimer <= 0)
                Destroy(gameObject);

            // fade out
            if(_spriteRenderer != null)
                _spriteRenderer.color = new Color(1, 1, 1, _fadingTimer / FadePeriod);
            // update timer
            _fadingTimer -= Time.deltaTime;
        }
        else
        {
            // update lighting
            Light.intensity = Mathf.Lerp(MaxLightIntensity, MinLightIntensity, _lifespanTimer / Lifespan);
            Light.pointLightOuterRadius = Mathf.Lerp(MaxLightRange, MinLightRange, _lifespanTimer / Lifespan);

            // lifespan updates
            if (_lifespanTimer > Lifespan)
            {
                _isFading = true;
                _fadingTimer = FadePeriod;
                _stopped = true;
            }
            if (StopMotionAfter != -1 && _lifespanTimer > StopMotionAfter)
                _stopped = true;

            _lifespanTimer += Time.deltaTime;

            // update velocity
            if (_stopped)
            {
                _rb.velocity = Vector2.zero;
                if(_collider != null)
                    _collider.enabled = false; // disable collider so it cannot keep hitting enemies
            }
            else
                _rb.velocity = (Vector2)transform.right * Mathf.Lerp(MaxSpeed, MinSpeed, _lifespanTimer / Lifespan);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
            _stopped = true;

        // walls destroy enemy projectiles, enemy projectiles destroy player projectiles
        if ((DestroyOnCollision && collision.CompareTag("Wall"))
            || (gameObject.CompareTag("EnemyBullet") && collision.CompareTag("LightBlast")))
            Destroy(gameObject);
    }
}
