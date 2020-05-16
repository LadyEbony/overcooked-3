using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCrate : MonoBehaviour, IInteractableBase {

  public int foodID;
  public GameObject selectedRenderer;

  public int IsInteractable(PlayerEntity player){
    return !player.grab.held ? 2 : int.MaxValue;
  }

  public void Activate(PlayerEntity player){

    if (foodID == -1){
      var item = PlateEntity.CreateEntity() as PlateEntity;
      UnitEntityManager.Local.Register(item);
      item.Activate(player);
    } else {
      var item = FoodEntity.CreateEntity() as FoodEntity;
      item.foodID = foodID;
      UnitEntityManager.Local.Register(item);
      item.Activate(player);
    }

  }

  public void OnSelect(PlayerEntity player) {
    selectedRenderer.SetActive(true);
  }

  public void OnDeselect(PlayerEntity player) {
    selectedRenderer.SetActive(false);
  }

}
