using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : Cabient, IInteractableAlt {

  [Header("Cutting Board Renderers")]
  public GameObject selectionAltRenderer;

  public int IsInteractableAlt(PlayerEntity player) {
    return PlayerHoldingNothingOnFullCabient(player) ? 1 : int.MaxValue;
  }

  public void ActivateAlt(PlayerEntity player) {
    if (PlayerHoldingNothingOnFullCabient(player) && item.description.cut > 0) {
      item.RaiseEvent('c', true);
    }
  }

  public void OnSelectAlt(PlayerEntity player) {
    selectionAltRenderer.SetActive(true);
  }

  public void OnDeselectAlt(PlayerEntity player) {
    selectionAltRenderer.SetActive(false);
  }
}
