using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // constants
    private const int RIGHT_DIRECTION = 0;
    private const int DOWN_DIRECTION = 1;
    private const int LEFT_DIRECTION = 2;
    private const int UP_DIRECTION = 3;
    private const float SIT_SPEED_THRESHOLD = 0.5f;
    private const float MAX_FLAME = 100f;
    private const int MAX_HP = 8;

    public Camera cam;

    [Header("Movement")]
    public float MaxWalkSpeed = 10f;
    [Tooltip("rate at which character reached max speed")]
    public float MovementSharpness = 10f;

    [Header("Lighting")]
    public Light2D CandleLight;
    public Light2D FlashlightStandard;
    public Light2D FlashlightUp;
    [Tooltip("offset of light transform from center when looking left or right")]
    public float SideLightOffset = 1f;
    public float MaxFlashlightIntensity = 1f;
    public float MinFlashlightIntensity = 0f;
    public float MaxCandlelightIntensity = 1f;
    public float MinCandlelightIntensity = 0f;
    public float MaxFlashlightRange = 10f;
    public float MinFlashlightRange = 1f;
    public float MaxCandlelightRange = 5f;
    public float MinCandlelightRange = 1f;
    public float FlameDecayRate = 0.1f;
    public float FlameRegenRate = 1.0f;

    [Header("UI")]
    public UI_Meter FlameMeter;

    [Header("Aiming")]
    public GameObject AimPivot;
    public SpriteRenderer WeaponSprite;

    // components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;

    // private variables
    private Vector2 _targetVelocity = Vector2.zero;
    private Vector2 _facing = Vector2.right;
    private bool _isSitting = true;
    private float _flameIntensity = MAX_FLAME;
    private int _hp = MAX_HP;
    private Vector3 _flashlightNaturalPos;

    // Start is called before the first frame update
    void Start()
    {
        // componesnts
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // decrement health for tutorial only
        if(SceneManager.GetActiveScene().name == "Tutorial")
            _hp--;

        _flashlightNaturalPos = FlashlightStandard.transform.localPosition;
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
        if (rb.velocity.magnitude < SIT_SPEED_THRESHOLD && InputHelper.GetOctoDirectionHeld() == InputHelper.OctoDirection.None)
            _isSitting = true;
        else
            _isSitting = false;
        // sutting animator state
        animator.SetBool("sit", _isSitting);

        // update facing direction
        _facing = ((Vector2) cam.ScreenToWorldPoint(Input.mousePosition) - (Vector2) AimPivot.transform.position).normalized;
        float facingAngle = Vector2.SignedAngle(Vector2.up, _facing);

        // Weapon rotation
        AimPivot.transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, facingAngle + 90);

        // direction animator state
        if (facingAngle >= 45f && facingAngle <= 135f)
            animator.SetInteger("direction", LEFT_DIRECTION);
        else if (facingAngle >= -45f && facingAngle <= 45f)
            animator.SetInteger("direction", UP_DIRECTION);
        else if (facingAngle >= -135f && facingAngle < -45f)
            animator.SetInteger("direction", RIGHT_DIRECTION);
        else
            animator.SetInteger("direction", DOWN_DIRECTION);

        // update flame values
        _flameIntensity -= FlameDecayRate * Time.deltaTime;
        _flameIntensity = Mathf.Clamp(_flameIntensity, 0, MAX_FLAME); // clamp within range
        FlameMeter.SetValue(_flameIntensity / MAX_FLAME);

        // Update candle light
        CandleLight.pointLightOuterRadius = Mathf.Lerp(MinCandlelightRange, MaxCandlelightRange, _flameIntensity / MAX_FLAME);
        CandleLight.intensity = Mathf.Lerp(MinCandlelightIntensity, MaxCandlelightIntensity, _flameIntensity / MAX_FLAME);

        // update flashlight
        
        // enable proper light depending on facing direction
        if (facingAngle >= -45f && facingAngle <= 45f) // if facing up
        {
            // light values
            FlashlightUp.intensity = Mathf.Lerp(MinFlashlightIntensity, MaxFlashlightIntensity, _flameIntensity / MAX_FLAME);
            FlashlightUp.pointLightOuterRadius = Mathf.Lerp(MinFlashlightRange, MaxFlashlightRange, _flameIntensity / MAX_FLAME);

            FlashlightUp.gameObject.SetActive(true);
            FlashlightStandard.gameObject.SetActive(false);

            // hidden behind player
            WeaponSprite.sortingOrder = -1;
        }
        else
        {
            // light values
            FlashlightStandard.intensity = Mathf.Lerp(MinFlashlightIntensity, MaxFlashlightIntensity, _flameIntensity / MAX_FLAME);
            FlashlightStandard.pointLightOuterRadius = Mathf.Lerp(MinFlashlightRange, MaxFlashlightRange, _flameIntensity / MAX_FLAME);
            // flashlight positions
            if (facingAngle >= 45f && facingAngle <= 135f) // facing left
                FlashlightStandard.transform.localPosition = _flashlightNaturalPos + Vector3.left * SideLightOffset;
            else if (facingAngle >= -135f && facingAngle < -45f) // facing right
                FlashlightStandard.transform.localPosition = _flashlightNaturalPos + Vector3.right * SideLightOffset;
            else // facing down
                FlashlightStandard.transform.localPosition = _flashlightNaturalPos;

            FlashlightStandard.gameObject.SetActive(true);
            FlashlightUp.gameObject.SetActive(false);

            // visible in front of player
            WeaponSprite.sortingOrder = 1;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Campfire"))
            _flameIntensity += FlameRegenRate * Time.deltaTime;
    }

    public int getHP() { return _hp; }
}
