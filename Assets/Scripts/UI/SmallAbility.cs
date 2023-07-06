using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallAbility : MonoBehaviour
{
    [SerializeField] private Image Icon;

    [Header("Icon Sprites")]
    [SerializeField] private Sprite RapidFlare;
    [SerializeField] private Sprite FlareBurst;
    [SerializeField] private Sprite FlameGun;
    [SerializeField] private Sprite FlameSlash;
    [SerializeField] private Sprite None;

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
        _slider.value = _player.GetUnequippedOverheat();

        // update icon sprite
        if (_player.IsPrimaryEquipped())
        {
            if (_player.GetSecondary() == Secondary.FlameGun)
                Icon.sprite = FlameGun;
            else if (_player.GetSecondary() == Secondary.FlameSlash)
                Icon.sprite = FlameSlash;
            else
                Icon.sprite = None;
        }
        else
        {
            if (_player.GetPrimary() == Primary.RapidFlare)
                Icon.sprite = RapidFlare;
            else if (_player.GetPrimary() == Primary.FlareBurst)
                Icon.sprite = FlareBurst;
            else
                Icon.sprite = None;
        }
    }
}
