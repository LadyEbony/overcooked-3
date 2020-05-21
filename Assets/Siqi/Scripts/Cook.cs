using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cook : MonoBehaviour
{
    public Food dish01;  // the dish01 prefab

    public Food food;   //The food in the hands of the cook

    public cabinetFood inCabinetFood;  // The cook in the CabinetFood trigger
    public cabinetSlice inCabinetSlice; //The cook in the CabinetSlice trigger
    public cabinetWork inCabinetWork; // The cook in the CabinetWork trigger
    public cabinetArrow inCabinetArrow; // The cook in the CabinetWork trigger
    public DeliveryTile inDeliveryTile; // The cook in the DeliveryTile trigger
    public cabinetPot   inCabinetPot; // The cook in the CabinetWork trigger

    // Start is called before the first frame update
    void Start()
    {
        inCabinetFood = null;
        inCabinetSlice = null;
        inCabinetWork = null;
        inDeliveryTile = null;
        inCabinetPot = null;

        food = null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(inCabinetFood != null)  
            // delete the handed food, and then get food from CabinetFood
            {
                if (food != null) GameObject.Destroy(food.gameObject);
                Transform parent = this.GetComponent<Transform>().GetChild(1);
                food = GameObject.Instantiate(inCabinetFood.food, parent.position,parent.rotation, parent);
            }
            else if ((inCabinetWork != null) && (inCabinetWork.food == null) && (food != null))
            // put food to CabinetWork
            {
                inCabinetWork.food = food;
                inCabinetWork.food.transform.parent = inCabinetWork.transform.GetChild(1);
                inCabinetWork.food.transform.position = inCabinetWork.transform.GetChild(1).transform.position;
                inCabinetWork.food.transform.rotation = inCabinetWork.transform.GetChild(1).transform.rotation;
                food = null;
            }
            else if ((inCabinetWork != null) && (inCabinetWork.food != null) && (inCabinetWork.food.foodStatus != FOODStatus.COOKING) && (food == null))
            // get food from CabinetWork
            {
                food = inCabinetWork.food;
                Transform parent = this.GetComponent<Transform>().GetChild(1);
                food.transform.parent = parent;
                food.transform.position = parent.position;
                food.transform.rotation = parent.rotation;
                inCabinetWork.food = null;
            }
            else if ((inCabinetWork != null) && (inCabinetWork.food != null) && (inCabinetWork.food.foodStatus == FOODStatus.SLICED)
                && (food != null)&& (food.foodStatus == FOODStatus.SLICED))
            // fix two foods to a dish
            {
                if(((inCabinetWork.food.foodType == FOODType.FISH)&&(food.foodType == FOODType.APPLE))
                  || ((inCabinetWork.food.foodType == FOODType.APPLE) && (food.foodType == FOODType.FISH)))
                {
                    Destroy(inCabinetWork.food.gameObject);
                    Destroy(food.gameObject);
                    inCabinetWork.food = null;
                    food = null;

                    Transform parent = inCabinetWork.GetComponent<Transform>().GetChild(1);
                    inCabinetWork.food = GameObject.Instantiate(dish01, parent.position, parent.rotation, parent);
                    inCabinetWork.food.foodType = FOODType.DISH01;
                    inCabinetWork.food.foodStatus = FOODStatus.COOKED;
                    
                }

            }
            else if ((inCabinetSlice != null) && (inCabinetSlice.food == null) 
                      && (food != null)&&(food.foodStatus == FOODStatus.ORIGINAL))  
            // put food to CabinetSlice, then Slice food
            {
                inCabinetSlice.food = food;
                Transform parent = inCabinetSlice.transform.GetChild(2);
                inCabinetSlice.food.transform.parent = parent;
                inCabinetSlice.food.transform.position = parent.position; //localPosition.Set(0, 0, 0);
                inCabinetSlice.food.transform.rotation = parent.rotation; //localRotation.Set(0,0,0,1);
                inCabinetSlice.food.slicing();
                food = null;
            }
            else if ((inCabinetSlice != null) && (inCabinetSlice.food != null) && (inCabinetSlice.food.foodStatus == FOODStatus.SLICED) && (food == null))  
            // get sliceFood from CabinetSlice
            {
                food = inCabinetSlice.food;
                Transform parent = this.GetComponent<Transform>().GetChild(1);
                food.transform.parent = parent;
                food.transform.position = parent.position;
                food.transform.rotation = parent.rotation;
                inCabinetSlice.food = null;
            }
            else if ((inCabinetPot != null) && (inCabinetPot.food == null)
                      && (food != null) && (food.foodStatus == FOODStatus.SLICED))
            // put food to inCabinetPot, then cook food
            {
                inCabinetPot.food = food;
                Transform parent = inCabinetPot.transform.GetChild(1);
                inCabinetPot.food.transform.parent = parent;
                inCabinetPot.food.transform.position = parent.position; 
                inCabinetPot.food.transform.rotation = parent.rotation;
                inCabinetPot.Cooking();
                food = null;
            }
            else if ((inCabinetPot != null) && (inCabinetPot.food != null) && (food == null))
            // get cooked Food from inCabinetPot
            {
                food = inCabinetPot.food;
                Transform parent = this.GetComponent<Transform>().GetChild(3);
                food.transform.parent = parent;
                food.transform.position = parent.position;
                food.transform.rotation = parent.rotation;
                inCabinetPot.food = null;
            }
            else if ((inCabinetArrow != null) && (inCabinetArrow.food == null) 
                    && (food != null) && (food.foodType == FOODType.DISH01))
            // put dish to CabinetArrow
            {
                inCabinetArrow.food = food;
                Transform parent = inCabinetArrow.transform.GetChild(2);
                inCabinetArrow.food.transform.parent = parent;
                inCabinetArrow.food.transform.position = parent.position;
                inCabinetArrow.food.transform.rotation = parent.rotation;
                inCabinetArrow.GetComponent<Animation>().Play("cabinetArrowTrans");
                food = null;
            }
            else if (inDeliveryTile != null) 
            {
                // player is inside delivery tile trigger
                if (food != null && inDeliveryTile.food == null)
                {
                    // place food on delivery tile
 //                   inDeliveryTile.food = food;
                    inDeliveryTile.food.transform.parent = inCabinetWork.transform.GetChild(1);
                    inDeliveryTile.food.transform.position = inCabinetWork.transform.GetChild(1).transform.position;
                    inDeliveryTile.food.transform.rotation = inCabinetWork.transform.GetChild(1).transform.rotation;
                    food = null;
                }
                else if (food == null && inDeliveryTile.food != null)
                {
                    // pickup food from delivery tile
 //                   food = inDeliveryTile.food;
                    Transform parent = this.GetComponent<Transform>().GetChild(1);
                    food.transform.parent = parent;
                    food.transform.position = parent.position;
                    food.transform.rotation = parent.rotation;
//                    inDeliveryTile.food = null;
                }
            }
        }
    }

}
