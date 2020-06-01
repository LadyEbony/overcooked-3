using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryTile : Cabient {

    public AudioSource source;
    public AudioClip success;


    private bool FoodIsCurrentOrder(PlateEntity plate) {
    // TODO: Implement Food Check 
    var isrecipe = ItemContainer.Instance.GetCompletedRecipe(plate);
    return isrecipe;
  }

  private void DestroyFoodAndUpdateScore(PlateEntity plate) {
    plate.Destroy();
    // TODO: Implement score update
  }

  // Update is called once per frame
  void Update() {
    var p = plate;

    // plate accepts
    if (p && NetworkManager.isMaster) {
      if (FoodIsCurrentOrder(p)) {
        DestroyFoodAndUpdateScore(p);
        source.PlayOneShot(success);
      }
    }
  }

}
