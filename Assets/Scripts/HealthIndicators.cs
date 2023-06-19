using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicators : MonoBehaviour
{
    public GameObject[] hearts;
    [HideInInspector] public PlayerController player;

    // private variables
    private int _currentHP;
    private int _currentHearts = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Dimwick").GetComponent<PlayerController>();

        _currentHP = player.GetHP();
         
        // instantiate new hearts
        while(_currentHearts < _currentHP)
        {
            hearts[_currentHearts].SetActive(true);
            _currentHearts++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _currentHP = player.GetHP();
        if (_currentHP < 0) _currentHP = 0;
        if (_currentHP > hearts.Length) _currentHP = hearts.Length;

        if (_currentHearts < _currentHP)
        {
            hearts[_currentHearts].SetActive(true);
            _currentHearts++;
        }
        else if (_currentHearts > _currentHP)
        {
            _currentHearts--;
            hearts[_currentHearts].SetActive(false);
        }

        
    }
}
