using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainer : MonoBehaviour {

  public static ItemContainer Instance { get; private set; }

  [Header("Foods")]
  public GameObject[] foods;

  [System.Serializable]
  public struct Recipe{
    public int[] ingredients;
  }

  [Header("Recipes")]
  public Recipe[] recipes; 

  private void Awake() {
    Instance = this;
  }

  // Should use an enum
  // -1 == deny
  // 0 == done
  // 1 == progress
  public bool GetRecipeResult(PlateEntity plate, FoodEntity food){
    // immediatly deny if the plate already as the ingredient
    if (plate.ingredients.Contains(food.foodID)) return false;
    
    foreach(var r in recipes){
      // if the recipe has the ingredient
      return r.ingredients.Contains(food.foodID);
    }

    return false;

  }

  public bool GetCompletedRecipe(PlateEntity plate){
    foreach(var r in recipes){
      var rhash = new HashSet<int>(r.ingredients);
      if (rhash.SetEquals(plate.ingredients)) return true;
    }
    return false;
  }

}
