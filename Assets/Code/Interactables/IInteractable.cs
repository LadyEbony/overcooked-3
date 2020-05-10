using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {
  void OnSelect(PlayerEntity player);
  void OnDeselect(PlayerEntity player);
}

public interface IInteractableBase : IInteractable {
  int IsInteractable(PlayerEntity player);
  void Activate(PlayerEntity player);
}

public interface IInteractableAlt : IInteractable {
  int IsInteractableAlt(PlayerEntity player);
  void ActivateAlt(PlayerEntity player);
}