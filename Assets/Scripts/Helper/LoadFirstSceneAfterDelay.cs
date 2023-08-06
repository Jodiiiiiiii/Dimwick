using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFirstSceneAfterDelay : MonoBehaviour
{
    public SceneManagementHelper sceneManager;
    public float TransitionDelay = 5f;

    private float _timer = 0f;

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > TransitionDelay)
            sceneManager.LoadSceneByDifficulty();
    }
}
