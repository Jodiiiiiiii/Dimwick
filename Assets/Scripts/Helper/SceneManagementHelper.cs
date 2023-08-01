using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneManagementHelper : MonoBehaviour
{

    #region SCENE TRANSITION FUNCTIONS
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void LoadSceneByDifficulty()
    {
        switch(GameManager.instance.GetDifficulty())
        {
            case Difficulty.Demo:
                SceneManager.LoadScene("Demo1");
                break;
            case Difficulty.Hard:
                // currently no hard scene
                break;
        }
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
    #endregion

    #region PLAYER DATA FUNCTIONS
    // should be called every time a new run is started
    public void ResetPlayerData()
    {
        GameManager.instance.InitializeDefaultPlayerData();
    }

    public void SetDifficulty(string difficulty)
    {
        if (difficulty == "Demo")
            GameManager.instance.SetDifficulty(Difficulty.Demo);
        else if (difficulty == "Hard")
            GameManager.instance.SetDifficulty(Difficulty.Hard);
        else
            Debug.LogError("Invalid difficulty type. double check spelling on button function");
    }
    #endregion

    #region AUDIO FUNCTIONS
    public void UIClickSound()
    {
        GameManager.instance.UIClickSound();
    }
    #endregion
}
