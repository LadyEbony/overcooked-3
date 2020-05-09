using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : Cabient, IInteractable {

  public override void ActivateAlt(PlayerEntity player) {
    var playerheld = player.grab.held;
    if (!playerheld && item) {
      item.RaiseEvent('c', true);
    }
  }

}
