using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    public GameObject[] birds;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < birds.Length; i++)
            for (int j = 0; j < birds.Length; j++)
                Physics2D.IgnoreCollision(birds[i].GetComponent<Collider2D>(), birds[j].GetComponent<Collider2D>(), true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
