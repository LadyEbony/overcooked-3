using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : Cabient {

  void Update() {
    // plate accepts
    if (item && item.isMine) {
      item.RaiseEvent('s', true);
    }
  }

}
