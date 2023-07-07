using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private bool _isInteractive = false;

    protected PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Dimwick").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_isInteractive && InputHelper.GetInteractPress())
            OnInteraction();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _isInteractive = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _isInteractive = false;
    }

    /// <summary>
    /// should handle interaction event and destroying this game object if necessary
    /// </summary>
    protected abstract void OnInteraction();
}
