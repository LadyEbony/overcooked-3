using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cabinetSlice : MonoBehaviour
{
    public Food food;

    void Start()
    {
        food = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Cook cook = other.GetComponent<Cook>();
        if (cook != null)
        {
            cook.inCabinetSlice = this;
            //Debug.Log("cook.inCabinetSlice = this;");
        }
        
        // if (sliceFood != null) sliceFood.slicing();
        
    }

    private void OnTriggerExit(Collider other)
    {
        Cook cook = other.GetComponent<Cook>();
        if (cook != null)
        {
            cook.inCabinetSlice = null;
            //Debug.Log("cook.inCabinetSlice = null;");
        }
        //if (sliceFood != null) sliceFood.sliceInit();
    }
 
}
