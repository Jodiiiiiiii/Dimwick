using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAfterDelay : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioClip _soundClip;
    [Range(0f, 1f)] [SerializeField] private float _soundVolume = 1f;

    [Header("Modifiers")]
    [SerializeField] private float _delay = 1f;
    [Tooltip("whether or not music stops right as sound is played")]
    [SerializeField] private bool _cutMusic = false;

    private float _timer;
    private bool _hasPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        _timer = _delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer < 0 && !_hasPlayed)
        {
            GameManager.instance.PlaySound(_soundClip, _soundVolume);
            _hasPlayed = true;

            if (_cutMusic)
                GameManager.instance.StopMusic();
        }
        else
            _timer -= Time.deltaTime;
    }
}
