using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityCrate : Interactable
{
    protected override void OnInteraction()
    {
        _player.CollectRandomUtility();

        Destroy(gameObject);
    }
}
