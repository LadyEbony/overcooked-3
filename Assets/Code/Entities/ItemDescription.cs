using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDescription : MonoBehaviour {

  [Header("Renderer")]
  public Material defaultMaterial;
  public Material selectedMaterial;

  [Header("Description")]
  public int cutAmount = -1;
  public int cookAmount = -1;

}
