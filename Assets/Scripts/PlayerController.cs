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

    [Header("Aiming")]
    public GameObject AimPivot;
    public SpriteRenderer WeaponSprite;
    public Animator WeaponAnimator;
    public RuntimeAnimatorController PrimaryAnimator;
    public RuntimeAnimatorController SecondaryAnimator;

    [Header("Weapons")]
    public Primary primary = Primary.None;
    public Secondary secondary = Secondary.None;
    public Utility utility = Utility.None;

    [Header("Overheat")]
    public float OverheatDuration = 5f;
    public float HeatDecayRate = 0.1f;
    public float OverheatFlashRate = 0.2f;

    [Header("Primary - RapidFlare")]
    public GameObject Bullet_RapidFlare;
    public float Cooldown_RapidFlare = 0.1f;
    public float HeatPer_RapidFlare = 0.05f;
    public float SpreadAngle_RapidFlare = 15f;

    [Header("Secondary - FlameGun")]
    public GameObject Bullet_FlameGun;
    public float Cooldown_FlameGun = 0.5f;
    public float HeatPer_FlameGun = 0.2f;
    public float SpreadAngle_FlameGun = 5f;

    // components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;

    // private variables
    private Vector2 _targetVelocity = Vector2.zero;
    private Vector2 _facing = Vector2.right;
    private bool _isSitting = true;
    private float _flameIntensity = MAX_FLAME;
    private int _hp = MAX_HP;
    // Primary weapons
    private bool _isprimaryEquipped = true;
    private float _primaryCooldownTimer = 0f;
    private float _primaryCooldown = 1f;
    private float _primaryHeat = 0f;
    private float _primaryHeatPer = 0.1f;
    private bool _isPrimaryOverheat = false;
    private float _primaryOverheatTimer = 0f;
    // secondary weapons
    private float _secondaryCooldownTimer = 0f;
    private float _secondaryCooldown = 1f;
    private float _secondaryHeat = 0f;
    private float _secondaryHeatPer = 0.1f;
    private bool _isSecondaryOverheat = false;
    private float _secondaryOverheatTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // componesnts
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // decrement health for tutorial only
        if(SceneManager.GetActiveScene().name == "Tutorial")
            _hp--;

        // default weapon animator to primary
        WeaponAnimator.runtimeAnimatorController = PrimaryAnimator;
    }

    // Update is called once per frame
    void Update()
    {
        #region MOVEMENT_INPUTS
        // update facing direction (mouse inputs)
        _facing = ((Vector2)cam.ScreenToWorldPoint(Input.mousePosition) - (Vector2)AimPivot.transform.position).normalized;
        float facingAngle = Vector2.SignedAngle(Vector2.up, _facing);

        // Handle movement direction input
        switch (InputHelper.GetOctoDirectionHeld())
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
        #endregion

        #region ATTACKING
        // update attack cooldown timers
        _primaryCooldownTimer += Time.deltaTime;
        _secondaryCooldownTimer += Time.deltaTime;

        // heat decay over time
        _primaryHeat = Mathf.Clamp(_primaryHeat - HeatDecayRate * Time.deltaTime, 0, 1);
        _secondaryHeat = Mathf.Clamp(_secondaryHeat - HeatDecayRate * Time.deltaTime, 0, 1);

        // Check for attack input
        bool isFireReady = false;
        if (InputHelper.GetLeftClick())
        {
            if(_isprimaryEquipped && _primaryCooldownTimer > _primaryCooldown && !_isPrimaryOverheat)
            {
                // restart cooldown for next shot
                _primaryCooldownTimer = 0;
                // increment heat
                _primaryHeat += _primaryHeatPer;
                if (_primaryHeat > 1) // enter overheat state
                {
                    _primaryHeat = 1;
                    _isPrimaryOverheat = true;
                    // start overheat timer
                    _primaryOverheatTimer = 0f;
                }

                isFireReady = true;
            }
            else if (!_isprimaryEquipped && _secondaryCooldownTimer > _secondaryCooldown && !_isSecondaryOverheat)
            {
                // restart cooldown for next shot
                _secondaryCooldownTimer = 0;
                // increment heat
                _secondaryHeat += _secondaryHeatPer;
                if (_secondaryHeat > 1) // enter overheat state
                {
                    _secondaryHeat = 1;
                    _isSecondaryOverheat = true;
                    // start overheat timer
                    _secondaryOverheatTimer = 0f;
                }

                isFireReady = true;
            }
        }

        // Create projectiles and sets weapon cooldown/heatPer stats
        if(_isprimaryEquipped)
        {
            switch (primary)
            {
                case Primary.RapidFlare:
                    _primaryCooldown = Cooldown_RapidFlare;
                    _primaryHeatPer = HeatPer_RapidFlare;

                    if (isFireReady)
                    {
                        Instantiate(Bullet_RapidFlare, WeaponSprite.transform.position,
                            Quaternion.Euler(0, 0, facingAngle + 90 + Random.Range(-SpreadAngle_RapidFlare, SpreadAngle_RapidFlare)));
                    }
                    break;
                case Primary.FlareBurst:
                    break;
                case Primary.None:
                    break;
            }
        }
        else // secondary
        {
            switch (secondary)
            {
                case Secondary.FlameGun:
                    _secondaryCooldown = Cooldown_FlameGun;
                    _secondaryHeatPer = HeatPer_FlameGun;

                    if(isFireReady)
                    {
                        Instantiate(Bullet_FlameGun, WeaponSprite.transform.position,
                            Quaternion.Euler(0, 0, facingAngle + 90 + Random.Range(-SpreadAngle_FlameGun, SpreadAngle_FlameGun)));
                    }
                    break;
                case Secondary.FlameSlash:
                    break;
                case Secondary.None:
                    break;
            }
        }

        // handle primary overheat state
        if(_isPrimaryOverheat)
        {
            if (_primaryOverheatTimer > OverheatDuration)
            {
                _isPrimaryOverheat = false;
                _primaryHeat = 0;
            }
            else
                _primaryOverheatTimer += Time.deltaTime;
        }
        // handle secondary overheat state
        if(_isSecondaryOverheat)
        {
            if (_secondaryOverheatTimer > OverheatDuration)
            {
                _isSecondaryOverheat = false;
                _secondaryHeat = 0;
            }
            else
                _secondaryOverheatTimer += Time.deltaTime;
        }

        // Utility Ability Handling (nothing for now)
        #endregion

        // set proper weapon sprite animation
        WeaponAnimator.runtimeAnimatorController = _isprimaryEquipped ? PrimaryAnimator : SecondaryAnimator;

        // Primary/secondary swap
        if (InputHelper.GetRightClickDown())
            _isprimaryEquipped = !_isprimaryEquipped;

        // update velocity based on target and current velocities
        rb.velocity = Vector2.Lerp(rb.velocity, _targetVelocity, 1 - Mathf.Exp(-MovementSharpness * Time.deltaTime));

        // update sitting state
        if (rb.velocity.magnitude < SIT_SPEED_THRESHOLD && InputHelper.GetOctoDirectionHeld() == InputHelper.OctoDirection.None)
            _isSitting = true;
        else
            _isSitting = false;
        // sutting animator state
        animator.SetBool("sit", _isSitting);

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
            WeaponSprite.sortingLayerName = "Player_Behind";
        }
        else
        {
            // light values
            FlashlightStandard.intensity = Mathf.Lerp(MinFlashlightIntensity, MaxFlashlightIntensity, _flameIntensity / MAX_FLAME);
            FlashlightStandard.pointLightOuterRadius = Mathf.Lerp(MinFlashlightRange, MaxFlashlightRange, _flameIntensity / MAX_FLAME);

            FlashlightStandard.gameObject.SetActive(true);
            FlashlightUp.gameObject.SetActive(false);

            // visible in front of player
            WeaponSprite.sortingLayerName = "Player_InFront";
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Campfire"))
            _flameIntensity += FlameRegenRate * Time.deltaTime;
    }

    public int GetHP() { return _hp; }

    /// <summary>
    /// returns 0 (min) to 1 (max)
    /// </summary>
    public float GetFlame() { return Mathf.Clamp(_flameIntensity / MAX_FLAME, 0, 1); }

    /// <summary>
    /// returns 0 (min) to 1 (max)
    /// </summary>
    public float GetEquippedOverheat()
    {
        if (_isprimaryEquipped)
        {
            if (_isPrimaryOverheat)
                return (int)(Time.time / OverheatFlashRate) % 2 == 0 ? 0f : 1f;
            else
                return Mathf.Clamp(_primaryHeat, 0, 1);
        }
        else
        {
            if (_isSecondaryOverheat)
                return (int)(Time.time / OverheatFlashRate) % 2 == 0 ? 0f : 1f;
            else
                return Mathf.Clamp(_secondaryHeat, 0, 1);
        }
    }

    /// <summary>
    /// returns 0 (min) to 1 (max)
    /// </summary>
    public float GetUnequippedOverheat()
    {
        if (_isprimaryEquipped)
        {
            if (_isSecondaryOverheat)
                return (int)(Time.time / OverheatFlashRate) % 2 == 0 ? 0f : 1f;
            else
                return Mathf.Clamp(_secondaryHeat, 0, 1);
        }
        else
        {
            if (_isPrimaryOverheat)
                return (int)(Time.time / OverheatFlashRate) % 2 == 0 ? 0f : 1f;
            else
                return Mathf.Clamp(_primaryHeat, 0, 1);
        }
    }

    public enum Primary
    {
        None, RapidFlare, FlareBurst
    }

    public enum Secondary
    {
        None, FlameGun, FlameSlash
    }

    public enum Utility
    {
        None, LightBlast, LightBlink
    }
}
