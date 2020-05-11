using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cabinetFood : MonoBehaviour
{
    public Food food;

    private void OnTriggerEnter(Collider other)
    {
        Cook cook = other.GetComponent<Cook>();
        if (cook != null) 
        {
           cook.inCabinetFood = this;
        }
            
        //    && (food != null))
        //{
        //    GameObject.Instantiate(food, other.GetComponent<Transform>().GetChild(3));
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        Cook cook = other.GetComponent<Cook>();
        if (cook != null)
        {
            cook.inCabinetFood = null;
        }
    }
}
