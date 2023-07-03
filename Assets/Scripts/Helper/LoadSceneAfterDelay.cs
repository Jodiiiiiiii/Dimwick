using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAfterDelay : MonoBehaviour
{
    public float TransitionDelay = 5f;
    public string TransitionSceneName = "Start";

    private float _timer = 0f;

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > TransitionDelay)
            SceneManager.LoadScene(TransitionSceneName);
    }
}
