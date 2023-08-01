using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMusicOnStart : MonoBehaviour
{
    [SerializeField] private AudioClip[] _musicTracks;
    [SerializeField] private bool _stopCurrentTrack = false;

    // Start is called before the first frame update
    void Start()
    {
        if(_stopCurrentTrack)
            GameManager.instance.StopMusic();

        GameManager.instance.SetMusicTracks(_musicTracks);

        Destroy(gameObject);
    }
}
