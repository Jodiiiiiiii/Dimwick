using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArsenalManagementHelper : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image IconTop;
    [SerializeField] private TextMeshProUGUI TextNameTop;
    [SerializeField] private TextMeshProUGUI TextTop;
    [SerializeField] private Image IconBottom;
    [SerializeField] private TextMeshProUGUI TextNameBottom;
    [SerializeField] private TextMeshProUGUI TextBottom;

    [Header("Primary")]
    [SerializeField] private Sprite RapidFlareIcon;
    [SerializeField] private string RapidFlareText;
    [SerializeField] private Sprite FlareBurstIcon;
    [SerializeField] private string FlareBurstText;

    [Header("Secondary")]
    [SerializeField] private Sprite FlameShotIcon;
    [SerializeField] private string FlameShotText;
    [SerializeField] private Sprite FlameSlashIcon;
    [SerializeField] private string FlameSlashText;

    [Header("Utility")]
    [SerializeField] private Sprite LightBlinkIcon;
    [SerializeField] private string LightBlinkText;
    [SerializeField] private Sprite LightBlastIcon;
    [SerializeField] private string LightBlastText;

    // Start is called before the first frame update
    void Start()
    {
        // disable all when entering the scene (until click on crate)
        IconTop.enabled = false;
        TextNameTop.enabled = false;
        TextTop.enabled = false;
        IconBottom.enabled = false;
        TextNameBottom.enabled = false;
        TextBottom.enabled = false;
    }

    public void PrimaryCrateClick()
    {
        // top
        IconTop.sprite = RapidFlareIcon;
        TextNameTop.text = "Rapid Flare";
        TextTop.text = RapidFlareText;

        // bottom
        IconBottom.sprite = FlareBurstIcon;
        TextNameBottom.text = "Flare Burst";
        TextBottom.text = FlareBurstText;

        // enable all components
        IconTop.enabled = true;
        TextNameTop.enabled = true;
        TextTop.enabled = true;
        IconBottom.enabled = true;
        TextNameBottom.enabled = true;
        TextBottom.enabled = true;
    }

    public void SecondaryCrateClick()
    {
        // top
        IconTop.sprite = FlameShotIcon;
        TextNameTop.text = "Flame Shot";
        TextTop.text = FlameShotText;

        // bottom
        IconBottom.sprite = FlameSlashIcon;
        TextNameBottom.text = "Flame Slash";
        TextBottom.text = FlameSlashText;

        // enable all components
        IconTop.enabled = true;
        TextNameTop.enabled = true;
        TextTop.enabled = true;
        IconBottom.enabled = true;
        TextNameBottom.enabled = true;
        TextBottom.enabled = true;
    }

    public void UtilityCrateClick()
    {
        // top
        IconTop.sprite = LightBlinkIcon;
        TextNameTop.text = "Light Blink";
        TextTop.text = LightBlinkText;

        // bottom
        IconBottom.sprite = LightBlastIcon;
        TextNameBottom.text = "Light Blast";
        TextBottom.text = LightBlastText;

        // enable all components
        IconTop.enabled = true;
        TextNameTop.enabled = true;
        TextTop.enabled = true;
        IconBottom.enabled = true;
        TextNameBottom.enabled = true;
        TextBottom.enabled = true;
    }
}
