using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UtilityAbility : MonoBehaviour
{
    [SerializeField] private Image Icon;

    [Header("Icon Sprites")]
    [SerializeField] private Sprite LightBlink;
    [SerializeField] private Sprite LightBlast;
    [SerializeField] private Sprite None;

    private PlayerController _player;

    // components
    private Slider _overheatSlider;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Dimwick").GetComponent<PlayerController>();

        _overheatSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        // update width based on input values
        _overheatSlider.value = _player.GetUtilityOverheat();

        if (_player.GetUtility() == Utility.LightBlink)
            Icon.sprite = LightBlink;
        else if (_player.GetUtility() == Utility.LightBlast)
            Icon.sprite = LightBlast;
        else
            Icon.sprite = None;
    }
}
