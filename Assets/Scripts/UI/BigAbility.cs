using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigAbility : MonoBehaviour
{
    [SerializeField] private Image Icon;

    [Header("Icon Sprites")]
    [SerializeField] private Sprite RapidFlare;
    [SerializeField] private Sprite FlareBurst;
    [SerializeField] private Sprite FlameGun;
    [SerializeField] private Sprite FlameSlash;

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
        _overheatSlider.value = _player.GetEquippedOverheat();

        // update icon sprite
        if(_player.IsPrimaryEquipped())
        {
            if (_player.GetPrimary() == Primary.RapidFlare)
                Icon.sprite = RapidFlare;
            else if (_player.GetPrimary() == Primary.FlareBurst)
                Icon.sprite = FlareBurst;
        }
        else
        {
            if (_player.GetSecondary() == Secondary.FlameGun)
                Icon.sprite = FlameGun;
            else if (_player.GetSecondary() == Secondary.FlameSlash)
                Icon.sprite = FlameSlash;
        }
    }
}
