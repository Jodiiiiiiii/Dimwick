using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableColliderAfterDelay : MonoBehaviour
{
    public Collider2D Collider;
    public float DelayTime = 0.25f;

    private float _timer;

    private void Start()
    {
        // start timer
        _timer = DelayTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer <= 0)
            Collider.enabled = false;
        else
            _timer -= Time.deltaTime;
    }
}
