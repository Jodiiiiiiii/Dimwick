using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryCrate : Interactable
{
    protected override void OnInteraction()
    {
        _player.CollectRandomPrimary();

        Destroy(gameObject);
    }
}
