using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

#region PUBLIC ENUMERATIONS
public enum CharacterState
{
    SceneEnterFall,
    Default,
    Hitstun
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
#endregion

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

    [Header("Invincibility")]
    [Tooltip("invincible player takes no damage but all other states apply - used for victory transition")]
    public bool IsInvincible = false;

    [Header("Scene Enter - Fall")]
    public float SceneEnterFallTime = 2f;
    public float SceneEnterFallDistance = 10f;

    [Header("Movement Inputs")]
    public float MaxWalkSpeed = 10f;
    [Tooltip("rate at which character reached max speed")]
    public float MovementSharpness = 10f;
    [Tooltip("rate at which character reaches goal rotation (mouse location)")]
    public float AimingSharpness = 10f;

    [Header("Health/Flame")]
    public int HealthRecoverPerTorch = 1;
    public float FlameRecoverPerTorch = MAX_FLAME / 2;

    [Header("Invulnerability")]
    public float InvulnerabilityTime = 1.5f;

    [Header("Lighting")]
    public Light2D CandleLight;
    public Light2D FlashlightStandard;
    public Light2D FlashlightUp;
    public ParticleSystem FlameParticles;
    public Light2D FlameLight;
    [Tooltip("offset of light transform from center when looking left or right")]
    public float MaxFlashlightIntensity = 1f;
    public float MinFlashlightIntensity = 0f;
    public float MaxCandlelightIntensity = 1f;
    public float MinCandlelightIntensity = 0f;
    public float MaxFlashlightRange = 10f;
    public float MinFlashlightRange = 1f;
    public float MaxCandlelightRange = 5f;
    public float MinCandlelightRange = 1f;
    public int MaxFlameParticleEmissionRate = 50;
    public int MinFlameParticleEmissionRate = 5;
    public float MaxFlameLightIntensity = 3f;
    public float MinFlameLightIntensity = 1.5f;
    public float FlameDecayRate = 0.1f;
    public float FlameRegenRate = 1.0f;

    [Header("Contact Damage")]
    public float ContactDamageHitstun = 1f;
    public float ContactDamageKnockbackSpeed = 4f;

    [Header("Aiming")]
    public GameObject AimPivot;
    public SpriteRenderer WeaponSprite;
    public Animator WeaponAnimator;
    public RuntimeAnimatorController PrimaryAnimator;
    public RuntimeAnimatorController SecondaryAnimator;

    [Header("Weapons")]
    public Primary Primary = Primary.None;
    public Secondary Secondary = Secondary.None;
    public Utility Utility = Utility.None;
    [Tooltip("Cooldown for firing shot after swapping weapons")]
    public float WeaponSwapCooldownDelay = 0.25f;

    [Header("Overheat")]
    public float OverheatDuration = 5f;
    public float HeatDecayRate = 0.1f;
    public float OverheatFlashRate = 0.2f;

    [Header("Primary - RapidFlare")]
    public GameObject Bullet_RapidFlare;
    public float Cooldown_RapidFlare = 0.1f;
    public float HeatPer_RapidFlare = 0.05f;
    public float SpreadAngle_RapidFlare = 15f;

    [Header("Primary - FlareBurst")]
    public GameObject Bullet_FlareBurst;
    public float Cooldown_FlareBurst = 0.5f;
    public float HeatPer_FlareBurst = 0.4f;
    public float SpreadAngle_FlareBurst = 15f;
    public int BulletCount_FlareBurst = 10;

    [Header("Secondary - FlameGun")]
    public GameObject Bullet_FlameGun;
    public float Cooldown_FlameGun = 0.5f;
    public float HeatPer_FlameGun = 0.2f;
    public float SpreadAngle_FlameGun = 5f;

    [Header("Secondary - FlameSlash")]
    public GameObject Bullet_FlameSlash;
    public float Cooldown_FlameSlash = 0.5f;
    public float HeatPer_FlameSlash = 0.4f;
    [Tooltip("distance from player origin that flame slash originates")]
    public float Displacement_FlameSlash = 1.0f;
    [Tooltip("additional angle to rotate slash by relative to player origin")]
    public float AngleOffset_FlameSlash = 10f;

    [Header("Utility - LightBlink")]
    public GameObject Bullet_LightBlink;
    [Tooltip("Includes Walls/Edges which player can collide with and should not teleport into")]
    public LayerMask BlinkLayerMask;
    public float MaxBlinkRange = 5f;

    [Header("Utility - LightBlast")]
    public GameObject Bullet_LightBlast;

    [Header("Audio - Footsteps")]
    public AudioClip[] StepClips;
    [Range(0f, 1f)] public float StepVolume;
    [Tooltip("Time between each step")]
    public float StepInterval = 0.2f;

    [Header("Audio - Abilities")]
    public AudioClip RapidFlareClip;
    [Range(0f, 1f)] public float RapidFlareVolume;
    public AudioClip FlareBurstClip;
    [Range(0f, 1f)] public float FlareBurstVolume;
    public AudioClip FlameShotClip;
    [Range(0f, 1f)] public float FlameShotVolume;
    public AudioClip FlameSlashClip;
    [Range(0f, 1f)] public float FlameSlashVolume;
    public AudioClip LightBlinkClip;
    [Range(0f, 1f)] public float LightBlinkVolume;
    public AudioClip LightBlastClip;
    [Range(0f, 1f)] public float LightBlastVolume;

    [Header("Audio - Misc.")]
    public AudioClip CratePickupClip;
    [Range(0f, 1f)] public float CratePickupVolume;
    public AudioClip HealClip;
    [Range(0f, 1f)] public float HealVolume;
    public AudioClip ToggleEquippedClip;
    [Range(0f, 1f)] public float ToggleEquippedVolume;
    public AudioClip PlayerDamageClip;
    [Range(0f, 1f)] public float PlayerDamageVolume;
    public AudioClip GameOverClip;
    [Range(0f, 1f)] public float GameOverVolume;

    // components
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public CapsuleCollider2D Collider;
    [HideInInspector] public FlashEffect FlashEffect;

    public CharacterState CurrentCharacterState { get; private set; } = CharacterState.Default;

    // private variables
    private Vector2 _targetVelocity = Vector2.zero;
    private Vector2 _targetFacing = Vector2.down;
    private Vector2 _facing = Vector2.down;
    private float _facingAngle = 180f;
    private bool _isSitting = true;
    private float _flameIntensity = MAX_FLAME;
    private bool _isFlameRegen = false;
    private int _hp = MAX_HP;
    // Scene Enter state
    private float _sceneEnterFallTimer = 0f;
    private Vector2 _topFallPos, _botFallPos;
    private bool _isFalling = false;
    // Hitstun
    private bool _isHitStunned = false;
    private float _hitStunTimer = 0f;
    // invulnerability
    private bool _isInvulnerable = false;
    private float _invulnerabilityTimer = 0f;
    // Primary weapons
    private bool _isPrimaryEquipped = true;
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
    // utility weapons
    private bool _isUtilityOverheat = false;
    private float _utilityOverheatTimer = 0f;
    // audio
    private float _stepTimer;

    // Start is called before the first frame update
    void Start()
    {
        // componesnts
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Collider = GetComponent<CapsuleCollider2D>();
        FlashEffect = GetComponent<FlashEffect>();

        // default weapon animator to primary
        WeaponAnimator.runtimeAnimatorController = PrimaryAnimator;

        // load current player data from game manager
        _hp = GameManager.instance.GetHealth();
        _flameIntensity = GameManager.instance.GetFlame();
        Primary = GameManager.instance.GetPrimary();
        Secondary = GameManager.instance.GetSecondary();
        Utility = GameManager.instance.GetUtility();

        // audio
        _stepTimer = StepInterval;

        // enter scene enter falling state
        TransitionToState(CharacterState.SceneEnterFall);
    }

    #region CHARACTER STATES
    public void TransitionToState(CharacterState newState)
    {
        CharacterState tmpInitialState = CurrentCharacterState;
        OnStateExit(tmpInitialState, newState);
        CurrentCharacterState = newState;
        OnStateEnter(newState, tmpInitialState);
    }

    public void OnStateEnter(CharacterState state, CharacterState fromState)
    {
        switch(state)
        {
            case CharacterState.SceneEnterFall:
                Collider.enabled = false;
                _isFalling = true;

                // set position information
                _botFallPos = Rb.position;
                Rb.position = Rb.position + Vector2.up * SceneEnterFallDistance;
                _topFallPos = Rb.position;

                _sceneEnterFallTimer = 0f;
                _facing = Vector2.down;

                break;
            case CharacterState.Default:
                break;
            case CharacterState.Hitstun:
                _isHitStunned = true;

                // start invulnerability
                _isInvulnerable = true;
                _invulnerabilityTimer = InvulnerabilityTime;
                FlashEffect.StartFlash();
      
                break;
        }
    }

    public void OnStateExit(CharacterState state, CharacterState toState)
    {
        switch(state)
        {
            case CharacterState.SceneEnterFall:
                Collider.enabled = true;
                _isFalling = false;
                GameManager.instance.PlaySound(StepClips[Random.Range(0, StepClips.Length)], StepVolume); // landing sound
                break;
            case CharacterState.Default:
                break;
            case CharacterState.Hitstun:
                _isHitStunned = false;
                break;
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.SceneEnterFall:

                #region SCENE ENTER FALL EFFECT
                Rb.position = Vector2.Lerp(_topFallPos, _botFallPos, Mathf.Clamp(_sceneEnterFallTimer / SceneEnterFallTime, 0, 1));

                if (Rb.position == _botFallPos)
                    TransitionToState(CharacterState.Default);
                else
                    _sceneEnterFallTimer += Time.deltaTime;
                #endregion

                break;
            case CharacterState.Default:

                #region MOVEMENT_INPUTS
                // update facing direction (mouse inputs)
                _targetFacing = ((Vector2)cam.ScreenToWorldPoint(Input.mousePosition) - (Vector2)AimPivot.transform.position).normalized;
                _facing = Vector2.Lerp(_facing, _targetFacing, 1 - Mathf.Exp(-AimingSharpness * Time.deltaTime));
                _facingAngle = Vector2.SignedAngle(Vector2.up, _facing);

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

                // update velocity based on target and current velocities
                Rb.velocity = Vector2.Lerp(Rb.velocity, _targetVelocity, 1 - Mathf.Exp(-MovementSharpness * Time.deltaTime));
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
                    if (_isPrimaryEquipped && _primaryCooldownTimer > _primaryCooldown && !_isPrimaryOverheat)
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
                    else if (!_isPrimaryEquipped && _secondaryCooldownTimer > _secondaryCooldown && !_isSecondaryOverheat)
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
                if (_isPrimaryEquipped)
                {
                    switch (Primary)
                    {
                        case Primary.RapidFlare:
                            _primaryCooldown = Cooldown_RapidFlare;
                            _primaryHeatPer = HeatPer_RapidFlare;

                            if (isFireReady)
                            {
                                Instantiate(Bullet_RapidFlare, WeaponSprite.transform.position,
                                    Quaternion.Euler(0, 0, _facingAngle + 90 + Random.Range(-SpreadAngle_RapidFlare, SpreadAngle_RapidFlare)));

                                GameManager.instance.PlaySound(RapidFlareClip, RapidFlareVolume);
                            }
                            break;
                        case Primary.FlareBurst:
                            _primaryCooldown = Cooldown_FlareBurst;
                            _primaryHeatPer = HeatPer_FlareBurst;

                            if (isFireReady)
                            {
                                for (int i = 0; i < BulletCount_FlareBurst; i++)
                                    Instantiate(Bullet_FlareBurst, WeaponSprite.transform.position,
                                        Quaternion.Euler(0, 0, _facingAngle + 90 + Random.Range(-SpreadAngle_FlareBurst, SpreadAngle_FlareBurst)));

                                GameManager.instance.PlaySound(FlareBurstClip, FlareBurstVolume);
                            }
                            break;
                        case Primary.None:
                            _primaryHeat = 0f;
                            break;
                    }
                }
                else // secondary
                {
                    switch (Secondary)
                    {
                        case Secondary.FlameGun:
                            _secondaryCooldown = Cooldown_FlameGun;
                            _secondaryHeatPer = HeatPer_FlameGun;

                            if (isFireReady)
                            {
                                Instantiate(Bullet_FlameGun, WeaponSprite.transform.position,
                                    Quaternion.Euler(0, 0, _facingAngle + 90 + Random.Range(-SpreadAngle_FlameGun, SpreadAngle_FlameGun)));

                                GameManager.instance.PlaySound(FlameShotClip, FlameShotVolume);
                            }
                            break;
                        case Secondary.FlameSlash:
                            _secondaryCooldown = Cooldown_FlameSlash;
                            _secondaryHeatPer = HeatPer_FlameSlash;

                            if (isFireReady)
                            {
                                Instantiate(Bullet_FlameSlash, WeaponSprite.transform.position + Displacement_FlameSlash * new Vector3(_facing.x, _facing.y, 0),
                                    Quaternion.Euler(0, 0, _facingAngle + 90 + AngleOffset_FlameSlash));

                                GameManager.instance.PlaySound(FlameSlashClip, FlameSlashVolume);
                            }
                            break;
                        case Secondary.None:
                            _secondaryHeat = 0f;
                            break;
                    }
                }

                // Utility Ability Handling (nothing for now)
                if (InputHelper.GetSpacePress() && !_isUtilityOverheat)
                {
                    switch(Utility)
                    {
                        case Utility.LightBlink:

                            // create initial light
                            Instantiate(Bullet_LightBlink, AimPivot.transform.position, Bullet_LightBlink.transform.rotation);

                            // raycast towards mouse for teleport
                            float raycastDistance = Mathf.Min(MaxBlinkRange, Vector2.Distance(AimPivot.transform.position, cam.ScreenToWorldPoint(Input.mousePosition)));
                            float raycastRadius = Mathf.Max(Collider.bounds.extents.x, Collider.bounds.extents.y);
                            RaycastHit2D hit = Physics2D.CircleCast(AimPivot.transform.position, raycastRadius, _targetFacing, raycastDistance, BlinkLayerMask);

                            if (hit.collider == null) // no obstructions
                            {
                                Rb.position = Rb.position + _targetFacing * raycastDistance;
                                // create destination light
                                Instantiate(Bullet_LightBlink, AimPivot.transform.position + (Vector3) _targetFacing * raycastDistance, 
                                    Bullet_LightBlink.transform.rotation);
                            }
                            else // obstruction collision
                            {
                                Rb.position = Rb.position + _targetFacing * (hit.distance - raycastRadius);
                                // create destination light
                                Instantiate(Bullet_LightBlink, AimPivot.transform.position + (Vector3) _targetFacing * (hit.distance - raycastRadius),
                                    Bullet_LightBlink.transform.rotation);
                            }

                            _isUtilityOverheat = true;
                            // start overheat timer
                            _utilityOverheatTimer = 0f;

                            GameManager.instance.PlaySound(LightBlinkClip, LightBlinkVolume);

                            break;
                        case Utility.LightBlast:

                            // create blast 'projectile'
                            Instantiate(Bullet_LightBlast, AimPivot.transform.position, Bullet_LightBlast.transform.rotation);

                            _isUtilityOverheat = true;
                            // start overheat timer
                            _utilityOverheatTimer = 0f;

                            GameManager.instance.PlaySound(LightBlastClip, LightBlastVolume);

                            break;
                        case Utility.None:
                            break;
                    }
                }

                // handle primary overheat state
                if (_isPrimaryOverheat)
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
                if (_isSecondaryOverheat)
                {
                    if (_secondaryOverheatTimer > OverheatDuration)
                    {
                        _isSecondaryOverheat = false;
                        _secondaryHeat = 0;
                    }
                    else
                        _secondaryOverheatTimer += Time.deltaTime;
                }
                // handle utility overheat state
                if(_isUtilityOverheat)
                {
                    if (_utilityOverheatTimer > OverheatDuration)
                    {
                        _isUtilityOverheat = false;
                    }
                    else
                        _utilityOverheatTimer += Time.deltaTime;
                }

                // Primary/secondary equipped swap
                if (InputHelper.GetRightClickDown())
                {
                    _isPrimaryEquipped = !_isPrimaryEquipped;

                    // apply weapon swap cooldown delay to cooldown timers
                    _primaryCooldownTimer = _primaryCooldown - WeaponSwapCooldownDelay;
                    _secondaryCooldownTimer = _secondaryCooldown - WeaponSwapCooldownDelay;

                    GameManager.instance.PlaySound(ToggleEquippedClip, ToggleEquippedVolume);
                }
                #endregion

                break;
            case CharacterState.Hitstun:

                #region HITSTUN HANDLING
                if (_hitStunTimer < 0f) // ends hitstun
                    TransitionToState(CharacterState.Default);
                else
                {
                    // no controls for movement or attacking

                    // update velocity based on target and current velocities
                    Rb.velocity = _targetVelocity; // no lerping (snappier)

                    _hitStunTimer -= Time.deltaTime;
                }
                #endregion

                break;
        }

        #region INVULNERABILITY
        if(_isInvulnerable)
        {
            _invulnerabilityTimer -= Time.deltaTime;
            if (_invulnerabilityTimer < 0)
            {
                _isInvulnerable = false;
                FlashEffect.StopFlash();
            }
        }
        #endregion

        #region LIGHTING
        // update flame values
        if (_isFlameRegen)
            _flameIntensity += FlameRegenRate * Time.deltaTime;
        else
            _flameIntensity -= FlameDecayRate * Time.deltaTime;
        _flameIntensity = Mathf.Clamp(_flameIntensity, 0, MAX_FLAME); // clamp within range

        // Update candle light
        CandleLight.pointLightOuterRadius = Mathf.Lerp(MinCandlelightRange, MaxCandlelightRange, _flameIntensity / MAX_FLAME);
        CandleLight.intensity = Mathf.Lerp(MinCandlelightIntensity, MaxCandlelightIntensity, _flameIntensity / MAX_FLAME);

        // update flashlight

        // enable proper light depending on facing direction
        _facingAngle = Vector2.SignedAngle(Vector2.up, _facing); // update facing angle
        if (_facingAngle >= -45f && _facingAngle <= 45f) // if facing up
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

        // Update flame particles
        FlameLight.intensity = Mathf.Lerp(MinFlameLightIntensity, MaxFlameLightIntensity, _flameIntensity / MAX_FLAME);
        var emission = FlameParticles.emission;
        emission.rateOverTime = Mathf.Lerp(MinFlameParticleEmissionRate, MaxFlameParticleEmissionRate, _flameIntensity / MAX_FLAME);
        #endregion

        #region ANIMATION
        // set proper weapon sprite animation
        WeaponAnimator.runtimeAnimatorController = _isPrimaryEquipped ? PrimaryAnimator : SecondaryAnimator;
        // Set weapon visual rotation
        AimPivot.transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, _facingAngle + 90);
        // toggle weapon and flashlight visibility with whether you have a weapon equipped
        if ((_isPrimaryEquipped && Primary == Primary.None) || (!_isPrimaryEquipped && Secondary == Secondary.None))
            AimPivot.SetActive(false);
        else
            AimPivot.SetActive(true);

        // sitting state
        if (Rb.velocity.magnitude < SIT_SPEED_THRESHOLD && (InputHelper.GetOctoDirectionHeld() == InputHelper.OctoDirection.None || _isFalling))
            _isSitting = true;
        else
            _isSitting = false;
        Animator.SetBool("sit", _isSitting);

        // falling state
        Animator.SetBool("isFalling", _isFalling);

        // isStunned state
        Animator.SetBool("isStunned", _isHitStunned);

        // direction state
        if (_facingAngle >= 45f && _facingAngle <= 135f)
            Animator.SetInteger("direction", LEFT_DIRECTION);
        else if (_facingAngle >= -45f && _facingAngle <= 45f)
            Animator.SetInteger("direction", UP_DIRECTION);
        else if (_facingAngle >= -135f && _facingAngle < -45f)
            Animator.SetInteger("direction", RIGHT_DIRECTION);
        else
            Animator.SetInteger("direction", DOWN_DIRECTION);
        #endregion

        #region STEP AUDIO
        if (!_isSitting)
        {
            if (_stepTimer <= 0)
            {
                _stepTimer = StepInterval;
                GameManager.instance.PlaySound(StepClips[Random.Range(0, StepClips.Length)], StepVolume);
            }
            else
                _stepTimer -= Time.deltaTime;
        }
        else
            _stepTimer = 0; // instantly play sound on stand up
        #endregion

        #region GAME OVER
        if (_hp <= 0)
            SceneManager.LoadScene("Game Over");
        #endregion

        #region DEBUG_CONTROLS
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Primary == Primary.FlareBurst)
                Primary = Primary.RapidFlare;
            else
                Primary = Primary.FlareBurst;
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Secondary == Secondary.FlameGun)
                Secondary = Secondary.FlameSlash;
            else
                Secondary = Secondary.FlameGun;
        }    
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (Utility == Utility.LightBlast)
                Utility = Utility.LightBlink;
            else
                Utility = Utility.LightBlast;
        }
        #endif
        #endregion
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // enemy contact damage
        if (!_isInvulnerable && collision.gameObject.CompareTag("Enemy"))
        {
            // decrement health
            if(!IsInvincible)
                _hp--;
            // start hitstun timer
            _hitStunTimer = ContactDamageHitstun;
            // set knockback based on bullet rotation and knockback stats
            _targetVelocity = ((Vector2)AimPivot.transform.position - (Vector2)collision.transform.position) * ContactDamageKnockbackSpeed;

            TransitionToState(CharacterState.Hitstun);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Campfire"))
            _isFlameRegen = true;

        // Enemy bullet damage
        if (!_isInvulnerable && collision.CompareTag("EnemyBullet")) // apply damage and hitstun
        {
            collision.gameObject.TryGetComponent<Projectile>(out Projectile projectile);
            if (projectile != null)
            {
                // decrement health
                if(!IsInvincible)
                    _hp--;
                // start hitstun timer
                _hitStunTimer = projectile.HitstunTime;
                // set knockback based on bullet rotation and knockback stats
                _targetVelocity = Quaternion.Euler(0, 0, collision.transform.rotation.eulerAngles.z) * Vector2.right * projectile.KnockbackSpeed;

                Destroy(collision.gameObject);

                TransitionToState(CharacterState.Hitstun);
            }
            else
                Debug.LogError("Invalid enemy projectile collison");
        }
        // spawn projectile bullet damage
        if(!_isInvulnerable && collision.CompareTag("SpawnProjectile")) // apply damage and hitstun
        {
            collision.gameObject.TryGetComponent<SpawnProjectile>(out SpawnProjectile projectile);
            if (projectile != null)
            {
                // decrement health
                if (!IsInvincible)
                    _hp--;
                // start hitstun timer
                _hitStunTimer = projectile.HitstunTime;
                // set knockback based on bullet rotation and knockback stats
                _targetVelocity = Quaternion.Euler(0, 0, collision.transform.rotation.eulerAngles.z) * Vector2.right * projectile.KnockbackSpeed;

                TransitionToState(CharacterState.Hitstun);
            }
            else
                Debug.Log("Invalid spawn projectile collision");
        }    
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Campfire"))
            _isFlameRegen = false;
    }

    #region PUBLIC_GETTERS
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
        if (_isPrimaryEquipped)
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
        if (_isPrimaryEquipped)
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

    public float GetUtilityOverheat()
    {
        // no intermediate heat state required (overheat or no overheat only)
        if (_isUtilityOverheat)
            return (int)(Time.time / OverheatFlashRate) % 2 == 0 ? 0f : 1f;
        else
            return 0f;
    }

    public bool IsPrimaryEquipped() { return _isPrimaryEquipped; }

    public Primary GetPrimary() { return Primary; }

    public Secondary GetSecondary() { return Secondary; }

    public Utility GetUtility() { return Utility; }

    // returns camera focus point of the player
    public Vector3 GetFocusPosition()
    {
        if (CurrentCharacterState == CharacterState.SceneEnterFall)
            return _botFallPos;
        else
            return transform.position; // might need to be changed for camera tracking bug later (?)
    }

    public Vector3 GetAimPivotPosition()
    {
        return AimPivot.transform.position;
    }
    #endregion

    #region PUBLIC MODIFIERS
    public void SetInvincibility(bool isInvincible) { IsInvincible = isInvincible; }

    public void ConsumeHealTorch()
    {
        _hp = Mathf.Clamp(_hp + HealthRecoverPerTorch, 0, MAX_HP);
        _flameIntensity = Mathf.Clamp(_flameIntensity + FlameRecoverPerTorch, 0, MAX_FLAME);

        GameManager.instance.PlaySound(HealClip, HealVolume);
    }

    public void CollectRandomPrimary()
    {
        switch(Primary)
        {
            case Primary.None:
                Primary = Random.Range(0f, 1f) > 0.5 ? Primary.FlareBurst : Primary.RapidFlare;
                break;
            case Primary.FlareBurst:
                Primary = Primary.RapidFlare;
                break;
            case Primary.RapidFlare:
                Primary = Primary.FlareBurst;
                break;
        }

        GameManager.instance.PlaySound(CratePickupClip, CratePickupVolume);
    }

    public void CollectRandomSecondary()
    {
        switch (Secondary)
        {
            case Secondary.None:
                Secondary = Random.Range(0f, 1f) > 0.5 ? Secondary.FlameGun : Secondary.FlameSlash;
                break;
            case Secondary.FlameGun:
                Secondary = Secondary.FlameSlash;
                break;
            case Secondary.FlameSlash:
                Secondary = Secondary.FlameGun;
                break;
        }

        GameManager.instance.PlaySound(CratePickupClip, CratePickupVolume);
    }

    public void CollectRandomUtility()
    {
        switch (Utility)
        {
            case Utility.None:
                Utility = Random.Range(0f, 1f) > 0.5 ? Utility.LightBlast : Utility.LightBlink;
                break;
            case Utility.LightBlast:
                Utility = Utility.LightBlink;
                break;
            case Utility.LightBlink:
                Utility = Utility.LightBlast;
                break;
        }

        GameManager.instance.PlaySound(CratePickupClip, CratePickupVolume);
    }

    // should be called whenever a new scene is loaded to preserve player data
    public void SavePlayerData()
    {
        GameManager.instance.SetHealth(_hp);
        GameManager.instance.SetFlame(_flameIntensity);
        GameManager.instance.SetPrimary(Primary);
        GameManager.instance.SetSecondary(Secondary);
        GameManager.instance.SetUtility(Utility);
    }
    #endregion
}
