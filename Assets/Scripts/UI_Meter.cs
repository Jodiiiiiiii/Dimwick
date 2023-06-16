using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Meter : MonoBehaviour
{
    // constants
    private const float MIN_VALUE = 0f;
    private const float MAX_VALUE = 1f;

    // components
    private Slider _slider;

    // private variables
    public float _value = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        // update width based on input values
        _slider.value = _value;
    }

    /// <summary>
    /// 1 is max, 0 is min
    /// </summary>
    public void SetValue(float newValue)
    {
        _value = Mathf.Clamp(newValue, MIN_VALUE, MAX_VALUE);
    }
}
