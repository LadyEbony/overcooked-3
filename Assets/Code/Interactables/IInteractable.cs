using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable{ }

public interface IInteractableBase : IInteractable {
  void OnSelect(PlayerEntity player);
  void OnDeselect(PlayerEntity player);

  int IsInteractable(PlayerEntity player);
  void Activate(PlayerEntity player);
}

public interface IInteractableAlt : IInteractable {
  void OnSelectAlt(PlayerEntity player);
  void OnDeselectAlt(PlayerEntity player);

  int IsInteractableAlt(PlayerEntity player);
  void ActivateAlt(PlayerEntity player);
}