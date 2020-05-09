using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabient : MonoBehaviour, IInteractable {
  
  private new Renderer renderer;

  [Header("IDs")]
  public int id;
  public static Dictionary<int, Cabient> cabients;

  [Header("Renderers")]
  public Material defaultMaterial;
  public Material selectedMaterial;
  public Transform placeTransform;

  [Header("Item")]
  public ItemEntity item;

  static Cabient(){
    cabients = new Dictionary<int, Cabient>();
  }

  public virtual void Awake(){
    cabients.Add(id, this);
    renderer = GetComponent<Renderer>();
  }

  public void Activate(PlayerEntity player) {
    var playerheld = player.grab.held;
    if (playerheld && !item){
      playerheld.RaiseEvent('l', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID, id);
    } else if (!playerheld && item) {
      item.RaiseEvent('p', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID);
    }
  }

  public virtual void ActivateAlt(PlayerEntity player) { }

  public bool IsInteractable(PlayerEntity player) {
    var playerheld = player.grab.held;
    return (playerheld && !item) || (!playerheld && item);
  }

  public void OnSelect(PlayerEntity player) {
    renderer.material = selectedMaterial;
  }

  public void OnDeselect(PlayerEntity player) {
    renderer.material = defaultMaterial;
  }

}
