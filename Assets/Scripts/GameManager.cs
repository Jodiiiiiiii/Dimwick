using System.Collections;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public enum Difficulty
{
    Demo,
    Hard
}

/// <summary>
/// Manages all save data throughout scenes and between sessions
/// </summary>
public class GameManager : MonoBehaviour
{
    // constants
    private const int MAX_PLAYER_HP = 8;
    private const float MAX_PLAYER_FLAME = 100;

    // instance
    public static GameManager instance;

    // components
    private AudioSource _audioSource;

    // audio
    //[SerializeField] private AudioClip secureCollectibleAudio;

    // save data
    [System.Serializable]
    private class PlayerData
    {
        public int Health;
        public float Flame;
        public Primary Primary;
        public Secondary Secondary;
        public Utility Utility;

        public Difficulty Difficulty;
    }
    private PlayerData _data;

    // UNITY METHODS ----------------------------------------------------------------------------

    private void Awake() // called each time a scene is loaded/reloaded
    {
        // setup SavePointManager as a singleton class
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // initialize and load data
            _data = new PlayerData();
            string path = Application.persistentDataPath + "/savedata.json";
            if (File.Exists(path))
            {
                // read json file into data object
                string json = File.ReadAllText(path);
                _data = JsonUtility.FromJson<PlayerData>(json);

                // Override save file (currently no save file usage)
                InitializeDefaultPlayerData();
            }
            else // default save file configuration
            {
                InitializeDefaultPlayerData();

                // default difficulty upon first initialization
                _data.Difficulty = Difficulty.Demo;
            }

            // components
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() // only called once (at program boot-up)
    {}

    private void Update()
    {
        // quit application from any scene with escape
        if(Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

            Application.Quit();
        }
    }

    private void OnApplicationQuit()
    {
        // save SavePointData to json file
        string json = JsonUtility.ToJson(_data);
        File.WriteAllText(Application.persistentDataPath + "/savedata.json", json);
    }

    /// <summary>
    /// initializes save data to a default state (use before starting a new run)
    /// </summary>
    public void InitializeDefaultPlayerData()
    {
        _data.Health = MAX_PLAYER_HP;
        _data.Flame = MAX_PLAYER_FLAME; // 1 is max flame
        _data.Primary = Primary.None;
        _data.Secondary = Secondary.None;
        _data.Utility = Utility.None;

        // does not reset difficulty (handled through setter functions)
    }

    #region GETTERS
    public int GetHealth()
    {
        return _data.Health;
    }

    public float GetFlame()
    {
        return _data.Flame;
    }

    public Primary GetPrimary()
    {
        return _data.Primary;
    }

    public Secondary GetSecondary()
    {
        return _data.Secondary;
    }

    public Utility GetUtility()
    {
        return _data.Utility;
    }

    public Difficulty GetDifficulty()
    {
        return _data.Difficulty;
    }
    #endregion

    #region SETTERS
    public void SetHealth(int health)
    {
        _data.Health = health;
    }

    public void SetFlame(float flame)
    {
        _data.Flame = flame;
    }

    public void SetPrimary(Primary primary)
    {
        _data.Primary = primary;
    }

    public void SetSecondary(Secondary secondary)
    {
        _data.Secondary = secondary;
    }

    public void SetUtility(Utility utility)
    {
        _data.Utility = utility;
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        _data.Difficulty = difficulty;
    }
    #endregion

    #region AUDIO
    public void PlaySound_PlayerDamage()
    {
        //audioSource.PlayOneShot(SOUND);
    }
    #endregion
}
