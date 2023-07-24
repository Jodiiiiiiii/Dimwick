using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigAbility : MonoBehaviour
{
    [SerializeField] private Image Icon;
    private Image BackColor;

    [Header("Icon Sprites")]
    [SerializeField] private Sprite RapidFlare;
    [SerializeField] private Sprite FlareBurst;
    [SerializeField] private Sprite FlameGun;
    [SerializeField] private Sprite FlameSlash;
    [SerializeField] private Sprite NonePrimary;
    [SerializeField] private Sprite NoneSecondary;

    private PlayerController _player;

    // components
    private Slider _overheatSlider;

    // Start is called before the first frame update
    void Start()
    {
        BackColor = GetComponent<Image>();

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
            {
                Icon.sprite = RapidFlare;
                BackColor.sprite = NonePrimary;
            }
            else if (_player.GetPrimary() == Primary.FlareBurst)
            {
                Icon.sprite = FlareBurst;
                BackColor.sprite = NonePrimary;
            }
            else
            {
                Icon.sprite = NonePrimary;
                BackColor.sprite = NonePrimary;
            }
        }
        else
        {
            if (_player.GetSecondary() == Secondary.FlameGun)
            {
                Icon.sprite = FlameGun;
                BackColor.sprite = NoneSecondary;
            }
            else if (_player.GetSecondary() == Secondary.FlameSlash)
            {
                Icon.sprite = FlameSlash;
                BackColor.sprite = NoneSecondary;
            }
            else
            {
                Icon.sprite = NoneSecondary;
                BackColor.sprite = NoneSecondary;
            }
        }
    }
}
