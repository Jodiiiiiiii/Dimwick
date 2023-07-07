using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTorch : Interactable
{
    protected override void OnInteraction()
    {
        _player.ConsumeHealTorch();

        Destroy(gameObject);
    }
}
