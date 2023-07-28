using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceWithRandomCrate : MonoBehaviour
{

    [Header("Crate Prefabs")]
    public GameObject PrimaryCrate;
    public GameObject SecondaryCrate;
    public GameObject UtilityCrate;

    // Start is called before the first frame update
    void Start()
    {
        //isntantly replace whatewver object this script is attached to with random crate
        float rand = Random.Range(0f, 3f);
        if (rand < 1)
            Instantiate(PrimaryCrate, transform.position, PrimaryCrate.transform.rotation);
        else if (rand < 2)
            Instantiate(SecondaryCrate, transform.position, SecondaryCrate.transform.rotation);
        else
            Instantiate(UtilityCrate, transform.position, UtilityCrate.transform.rotation);

        Destroy(gameObject);
    }
}
