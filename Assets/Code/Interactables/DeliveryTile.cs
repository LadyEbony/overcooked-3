using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryTile : Cabient {

  

  private bool FoodIsCurrentOrder(PlateEntity plate) {
    // TODO: Implement Food Check 
    var isrecipe = ItemContainer.Instance.GetCompletedRecipe(plate);
    if (isrecipe == -1) return false;

    return OrderManager.Instance.IsAnOrder(isrecipe);
  }

  private void DestroyFoodAndUpdateScore(PlateEntity plate) {
    OrderManager.Instance.RemoveOrder(ItemContainer.Instance.GetCompletedRecipe(plate));
    plate.RaiseEvent('s', true);
    // TODO: Implement score update
  }

  // Update is called once per frame
  void Update() {
    var p = plate;

    // plate accepts
    if (p && NetworkManager.isMaster) {
      if (FoodIsCurrentOrder(p)) {
        DestroyFoodAndUpdateScore(p);
      }
    }
  }

}
