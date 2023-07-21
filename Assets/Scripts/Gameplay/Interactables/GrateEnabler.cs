using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrateEnabler : MonoBehaviour
{
    [Tooltip("object containing all enemies in the scene as children")]
    public GameObject EnemyContainer;
    
    [HideInInspector] public GrateTransition TransitionInteractable;
    [HideInInspector] public Animator Anim;

    // Start is called before the first frame update
    void Start()
    {
        TransitionInteractable = GetComponent<GrateTransition>();
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(EnemyContainer.GetComponentsInChildren<Transform>().Length == 1) // only parent empty remains
        {
            Anim.enabled = true;
            TransitionInteractable.enabled = true;
        }
    }
}
