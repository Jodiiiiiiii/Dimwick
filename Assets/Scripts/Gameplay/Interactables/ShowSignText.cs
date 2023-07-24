using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowSignText : Interactable
{
    [Header("Text Component")]
    public GameObject TextBox;
    public string Text;

    protected override void OnInteraction()
    {
        // update text
        TextBox.GetComponentsInChildren<TextMeshProUGUI>()[0].text = Text;

        TextBox.SetActive(true);
    }
}
