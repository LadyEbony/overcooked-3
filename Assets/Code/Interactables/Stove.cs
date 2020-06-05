using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stove : Cabient {

  [Header("Effects")]
  public GameObject progressBarMain;
  public SpriteRenderer progressBarRenderer;
  public GameObject cooking;  // the cooking particle

  [Header("Cook timers")]
  public float cookTime = 1f;
  public float burnTime = 3f;
  private float nextCookTime;
  private FoodEntity cookingFood;

  [Header("Sound effects")]
  public AudioSource source;
  public AudioClip boiling;

  private void Update() {
    var f = food;
    if (f == null || f.cookCurrent == -1){
      progressBarMain.SetActive(false);
    } else {
      progressBarMain.SetActive(true);

      if (f.cookCurrent == -2){
        progressBarRenderer.transform.localScale = Vector3.one;
        progressBarRenderer.color = Color.red;
      } else {
        progressBarRenderer.transform.localScale = new Vector3(f.cookPercentage, 1f, 1f);
        progressBarRenderer.color = f.cookCurrent == 0 ? Color.green : Color.yellow;
      }
      
    }

    if (f && f.isMine && f.cookCurrent >= 0){
      cooking.SetActive(true);
      if (!source.isPlaying)
      {
        source.clip = boiling;
        source.Play();
      }

      // scuffed way to check if new item
      if (f != cookingFood){
        cookingFood = f;
        nextCookTime = Time.time + (f.cookCurrent > 0 ? cookTime : burnTime);
      }

      if (Time.time >= nextCookTime){
        f.RaiseEvent('k', true);
        nextCookTime = Time.time + (f.cookCurrent > 0 ? cookTime : burnTime);
      }

    } else {
       source.Stop();
       cooking.SetActive(false);
       cookingFood = null;
    }
  }



}
