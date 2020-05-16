﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stove : Cabient {

  public TextMeshPro textMesh;

  [Header("Cook timers")]
  public float cookTime = 1f;
  private float nextCookTime;
  private FoodEntity cookingFood;

  private void Update() {
    var f = food;
    UpdateTextMesh(textMesh, f && f.cookCurrent >= 0 ? f.cookPercentage : -1);

    if (f && f.isMine && f.cookCurrent >= 0){
      // scuffed way to check if new item
      if (f != cookingFood){
        cookingFood = f;
        nextCookTime = Time.time + cookTime;
      }

      if (Time.time >= nextCookTime){
        f.RaiseEvent('k', true);
        nextCookTime = Time.time + cookTime;
      }

    } else {
      cookingFood = null;
    }
  }



}
