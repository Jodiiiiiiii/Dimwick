using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectOnTrigger : MonoBehaviour
{
    [Header("Object to activate")]
    public GameObject Object;

    // Start is called before the first frame update
    void Start()
    {
        Object.SetActive(false); // deactivate object by default
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Object.SetActive(true);
    }
}
