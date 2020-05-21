using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryTile : Cabient
{
    private bool FoodIsCurrentOrder(FoodEntity food)
    {
        // TODO: Implement Food Check 
        return true;
    }

    private void DestroyFoodAndUpdateScore(FoodEntity food)
    {
        food.Destroy();
        // TODO: Implement score update
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var f = food;

        if (f && f.isMine)
        {
            if (FoodIsCurrentOrder(f))
            {
                DestroyFoodAndUpdateScore(f);
            }
        }
    }

}
