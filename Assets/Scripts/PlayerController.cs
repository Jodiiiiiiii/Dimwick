using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    // constants
    private const int RIGHT_DIRECTION = 0;
    private const int DOWN_DIRECTION = 1;
    private const int LEFT_DIRECTION = 2;
    private const int UP_DIRECTION = 3;
    private const float SIT_SPEED_THRESHOLD = 0.5f;
    private const float MAX_FLAME = 100f;

    public Camera cam;

    [Header("Movement")]
    public float MaxWalkSpeed = 10f;
    [Tooltip("rate at which character reached max speed")]
    public float MovementSharpness = 10f;

    [Header("Lighting")]
    public Light2D CandleLight;
    public Light2D Flashlight;
    public float MaxFlashlightIntensity = 1f;
    public float MinFlashlightIntensity = 0f;
    public float MaxCandlelightIntensity = 1f;
    public float MinCandlelightIntensity = 0f;
    public float FlameDecayRate = 0.1f;
    public float FlameRegenRate = 1.0f;

    [Header("UI")]
    public UI_Meter FlameMeter;

    // components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;

    // private variables
    private Vector2 _targetVelocity = Vector2.zero;
    private Vector2 _facing = Vector2.right;
    private bool _isSitting = true;
    private float _flameIntensity = MAX_FLAME;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle movement direction input
        switch(InputHelper.GetOctoDirectionHeld())
        {
            case InputHelper.OctoDirection.Up:
                _targetVelocity = Vector2.up * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.Right:
                _targetVelocity = Vector2.right * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.Down:
                _targetVelocity = Vector2.down * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.Left:
                _targetVelocity = Vector2.left * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.UpRight:
                _targetVelocity = (Vector2.up + Vector2.right).normalized * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.DownRight:
                _targetVelocity = (Vector2.down + Vector2.right).normalized * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.DownLeft:
                _targetVelocity = (Vector2.down + Vector2.left).normalized * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.UpLeft:
                _targetVelocity = (Vector2.up + Vector2.left).normalized * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.None:
                _targetVelocity = Vector2.zero;
                break;
        }

        // update velocity based on target and current velocities
        rb.velocity = Vector2.Lerp(rb.velocity, _targetVelocity, 1 - Mathf.Exp(-MovementSharpness * Time.deltaTime));

        // update sitting state
        if (rb.velocity.magnitude < SIT_SPEED_THRESHOLD)
            _isSitting = true;
        else
            _isSitting = false;
        // sutting animator state
        animator.SetBool("sit", _isSitting);

        // update facing direction
        _facing = ((Vector2) cam.ScreenToWorldPoint(Input.mousePosition) - rb.position).normalized;
        float facingAngle = Vector2.SignedAngle(Vector2.up, _facing);
        // direction animator state
        if (facingAngle >= 45f && facingAngle <= 135f)
            animator.SetInteger("direction", LEFT_DIRECTION);
        else if (facingAngle >= -45f && facingAngle <= 45f)
            animator.SetInteger("direction", UP_DIRECTION);
        else if (facingAngle >= -135f && facingAngle < -45f)
            animator.SetInteger("direction", RIGHT_DIRECTION);
        else
            animator.SetInteger("direction", DOWN_DIRECTION);

        // update flashlight rotation bassed on facing direction
        Flashlight.transform.rotation = Quaternion.Euler(Vector3.forward * facingAngle);

        // update flame values
        _flameIntensity -= FlameDecayRate * Time.deltaTime;
        Flashlight.intensity = Mathf.Lerp(MinFlashlightIntensity, MaxFlashlightIntensity, _flameIntensity / MAX_FLAME);
        CandleLight.intensity = Mathf.Lerp(MinCandlelightIntensity, MaxCandlelightIntensity, _flameIntensity / MAX_FLAME);
        FlameMeter.SetValue(_flameIntensity / MAX_FLAME);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Campfire"))
            _flameIntensity += FlameRegenRate * Time.deltaTime;
    }
}
