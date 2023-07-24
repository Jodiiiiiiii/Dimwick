using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInactiveOnE : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // disable active game object if E is pressed
        if (InputHelper.GetInteractPress() && gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
