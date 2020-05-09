using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour {

  public static ItemContainer Instance { get; private set; }

  public GameObject[] prefabs;

  private void Awake() {
    Instance = this;
  }

}
