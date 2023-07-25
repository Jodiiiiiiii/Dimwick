using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [Header("Light")]
    public Light2D Light;

    [Header("Stats")]
    public float MaxPeriod = 1.2f;
    public float MinPeriod = 0.5f;
    public float MaxIntensity = 1f;
    public float MinIntensity = 0.8f;
    public float MaxRange = 2f;
    public float MinRange = 1.8f;
    [Tooltip("smoothing rate seeking goal values")]
    public float GoalSharpness = 1f;

    private float _newGoalTimer;
    private float _goalRange;
    private float _goalIntensity;

    // Start is called before the first frame update
    void Start()
    {
        _newGoalTimer = Random.Range(MinPeriod, MaxPeriod);
        _goalRange = Random.Range(MinRange, MaxRange);
        _goalIntensity = Random.Range(MinIntensity, MaxIntensity);
    }

    // Update is called once per frame
    void Update()
    {
        // timer Handling
        if(_newGoalTimer <= 0f)
        {
            _newGoalTimer = Random.Range(MinPeriod, MaxPeriod);
            _goalRange = Random.Range(MinRange, MaxRange);
            _goalIntensity = Random.Range(MinIntensity, MaxIntensity);
        }
        else
        {
            _newGoalTimer -= Time.deltaTime;
        }

        // smoothing lerp towards goal values
        Light.intensity = Mathf.Lerp(Light.intensity, _goalIntensity, 1 - Mathf.Exp(-GoalSharpness * Time.deltaTime));
        Light.pointLightOuterRadius = Mathf.Lerp(Light.pointLightOuterRadius, _goalRange, 1 - Mathf.Exp(-GoalSharpness * Time.deltaTime));
    }
}
