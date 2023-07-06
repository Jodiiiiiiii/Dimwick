using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicators : MonoBehaviour
{
    public GameObject[] hearts;
    [HideInInspector] public PlayerController player;

    // private variables
    private int _currentHP;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Dimwick").GetComponent<PlayerController>();

        _currentHP = player.GetHP();
    }

    // Update is called once per frame
    void Update()
    {
        _currentHP = player.GetHP();
        if (_currentHP < 0) _currentHP = 0;
        if (_currentHP > hearts.Length) _currentHP = hearts.Length;

        // iterate through all and set proper ones to active or inactive
        for(int i = 0; i < hearts.Length; i++)
        {
            if (_currentHP > i) hearts[i].SetActive(true);
            else hearts[i].SetActive(false);
        }
    }
}
