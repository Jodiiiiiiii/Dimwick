using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GrateTransition : Interactable
{
    public string SceneName = "Start";

    protected override void OnInteraction()
    {
        // save player states in game manager
        GameObject.Find("Dimwick").GetComponent<PlayerController>().SavePlayerData();

        // load scene transition
        SceneManager.LoadScene(SceneName);
    }
}
