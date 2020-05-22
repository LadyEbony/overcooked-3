using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabient : MonoBehaviour, IInteractableBase {

  [Header("IDs")]
  public int id;
  public static Dictionary<int, Cabient> cabients;

  [Header("Cabient Renderers")]
  public GameObject selectionRenderer;
  public Transform placeTransform;

  public ItemEntity item => DoubleDictionary<Cabient, ItemEntity>.Get(this);
  public FoodEntity food => item as FoodEntity;
  public PlateEntity plate => item as PlateEntity;

  static Cabient(){
    cabients = new Dictionary<int, Cabient>();
  }

  public virtual void Awake(){
    cabients.Add(id, this);
  }

  public void Activate(PlayerEntity player) {
    if (PlayerHoldingItemOnEmptyCabient(player, out var held)){
      held.RaiseEvent('l', true, NetworkManager.ServerTimeFloat, id);
    }
  }

  public int IsInteractable(PlayerEntity player) {
    return PlayerHoldingItemOnEmptyCabient(player) ? 2 : int.MaxValue;
  }

  public void OnSelect(PlayerEntity player) {
    selectionRenderer.SetActive(true);
  }

  public void OnDeselect(PlayerEntity player) {
    selectionRenderer.SetActive(false);
  }

  protected bool PlayerHoldingItemOnEmptyCabient(PlayerEntity player){
    var held = player.held;
    return held && !item;
  }

  protected bool PlayerHoldingItemOnEmptyCabient(PlayerEntity player, out ItemEntity held){
    held = player.held;
    return held && !item;
  }

  protected bool PlayerHoldingNothingOnFullCabient(PlayerEntity player){
    var held = player.held;
    return !held && item;
  }

  protected bool PlayerHoldingNothingOnFullCabient(PlayerEntity player, out ItemEntity held){
    held = player.held;
    return !held && item;
  }

  protected void UpdateTextMesh(TMPro.TextMeshPro textMesh, float value){
    if (value >= 0){
      textMesh.text = string.Format("{0}%", (int)(value * 100));

      if (value < 0.5f){
        textMesh.color = Color.red;
      } else if (value < 1.0f){
        textMesh.color = Color.yellow;
      } else {
        textMesh.color = Color.green;
      }
    } else {
      textMesh.text = string.Empty;
    }  
  }

}
