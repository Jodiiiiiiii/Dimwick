using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryCrate : Interactable
{
    protected override void OnInteraction()
    {
        _player.CollectRandomSecondary();

        Destroy(gameObject);
    }
}
