using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cabinetWork : MonoBehaviour
{
    public Food food;       //The food on the cabinetWork

    // Start is called before the first frame update
    void Start()
    {
        food = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Cook cook = other.GetComponent<Cook>();
        if (cook != null)
        {
            cook.inCabinetWork = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Cook cook = other.GetComponent<Cook>();
        if (cook != null)
        {
            cook.inCabinetWork = null;
        }
    }
}
