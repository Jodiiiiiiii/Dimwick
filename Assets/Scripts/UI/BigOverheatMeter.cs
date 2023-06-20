using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigOverheatMeter : MonoBehaviour
{
    private PlayerController _player;

    // components
    private Slider _slider;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Dimwick").GetComponent<PlayerController>();

        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        // update width based on input values
        _slider.value = _player.GetEquippedOverheat();
    }
}
