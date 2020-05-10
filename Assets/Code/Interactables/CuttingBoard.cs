using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : Cabient, IInteractableAlt {

  public int IsInteractableAlt(PlayerEntity player) {
    return PlayerHoldingNothingOnFullCabient(player) ? 1 : int.MaxValue;
  }

  public void ActivateAlt(PlayerEntity player) {
    if (PlayerHoldingNothingOnFullCabient(player) && item.description.cut > 0) {
      item.RaiseEvent('c', true);
    }
  }

}
