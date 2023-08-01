using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBossEncounter : MonoBehaviour
{
    [Header("Object to activate")]
    public GameObject Barricade;

    public AudioClip BarricadeClip;
    public float BarricadeVolume = 0.5f;

    public AudioClip[] BossMusic;

    private Collider2D _collider;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider2D>();

        Barricade.SetActive(false); // deactivate object by default
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // lock player in room
            Barricade.SetActive(true);
            GameManager.instance.PlaySound(BarricadeClip, BarricadeVolume);

            // boss music
            GameManager.instance.SetMusicTracks(BossMusic);
            GameManager.instance.StopMusic();

            _collider.enabled = false; // prevent reactivating trigger for sounds/music reset
        }
    }
}
