using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CuttingBoard : Cabient, IInteractableAlt {

  [Header("Cutting Board Renderers")]
  public GameObject selectionAltRenderer;
  public TextMeshPro textMesh;

  void Update(){
    var f = food;
    UpdateTextMesh(textMesh, f && f.cutCurrent >= 0 ? f.cutPercentage : -1);
  }

  public int IsInteractableAlt(PlayerEntity player) {
    var f = food;
    return PlayerHoldingNothingOnFullCabient(player) && f && f.cutCurrent > 0 ? 1 : int.MaxValue;
  }

  public void ActivateAlt(PlayerEntity player) {
    var f = food;
    //f.description.GetComponent<Food>().slicing();
    if (PlayerHoldingNothingOnFullCabient(player) && f && f.cutCurrent > 0) {
        f.RaiseEvent('c', true);
    }
  }

  public void OnSelectAlt(PlayerEntity player) {
    selectionAltRenderer.SetActive(true);
  }

  public void OnDeselectAlt(PlayerEntity player) {
    selectionAltRenderer.SetActive(false);
  }
}
