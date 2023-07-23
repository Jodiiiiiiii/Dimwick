using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossMeter : MonoBehaviour
{
    private DarknessController _darkness;

    // components
    private Slider _slider;

    // Start is called before the first frame update
    void Start()
    {
        _darkness = GameObject.Find("Darkness").GetComponent<DarknessController>();

        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        _slider.value = _darkness.GetHPRatio();
    }
}
