using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {
 
  bool IsInteractable(PlayerEntity player);
  void Activate(PlayerEntity player);
  void ActivateAlt(PlayerEntity player);

  void OnSelect(PlayerEntity player);
  void OnDeselect(PlayerEntity player);

}
