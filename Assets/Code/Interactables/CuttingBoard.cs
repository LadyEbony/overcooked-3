using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CuttingBoard : Cabient, IInteractableAlt {

  [Header("Cutting Board Renderers")]
  public GameObject selectionAltRenderer;
  public GameObject progressBarMain;
  public SpriteRenderer progressBarRenderer;

  void Update(){
    var f = food;
    if (f == null || f.cutCurrent == -1){
      progressBarMain.SetActive(false);
    } else {
      progressBarMain.SetActive(true);
      progressBarRenderer.transform.localScale = new Vector3(f.cutPercentage, 1f, 1f);
      progressBarRenderer.color = f.cutCurrent == 0 ? Color.green : Color.yellow;
    }
    
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
