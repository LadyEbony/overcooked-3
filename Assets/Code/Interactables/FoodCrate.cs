using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCrate : MonoBehaviour, IInteractable {

  private new Renderer renderer;

  public Material defaultMaterial;
  public Material selectedMaterial;

  void Awake(){
    renderer = GetComponent<Renderer>();
  }

  public bool IsInteractable(PlayerEntity player){
    return !player.grab.held;
  }

  public void Activate(PlayerEntity player){
    var item = ItemEntity.CreateEntity() as ItemEntity;
    UnitEntityManager.Local.Register(item);
    item.Activate(player);
  }

  public void OnSelect(PlayerEntity player) {
    renderer.material = selectedMaterial;
  }

  public void OnDeselect(PlayerEntity player) {
    renderer.material = defaultMaterial;
  }
}
