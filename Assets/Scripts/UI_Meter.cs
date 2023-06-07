using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Meter : MonoBehaviour
{
    // constants
    private const float MIN_VALUE = 0f;
    private const float MAX_VALUE = 1f;

    // components
    private RectTransform _rect;

    // private variables
    public float _value = 1f;
    private float _initialWidth;
    private float _initialHeight;

    // Start is called before the first frame update
    void Start()
    {
        _rect = GetComponent<RectTransform>();
        _initialWidth = _rect.sizeDelta.x;
        _initialHeight = _rect.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        // update width based on input values
        _rect.sizeDelta = new Vector2(_value * _initialWidth, _initialHeight);
    }

    /// <summary>
    /// 1 is max, 0 is min
    /// </summary>
    public void SetValue(float newValue)
    {
        _value = Mathf.Clamp(newValue, MIN_VALUE, MAX_VALUE);
    }
}
