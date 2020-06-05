using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct Recipe{
  public string name;
  public int[] ingredients;
  public Sprite sprite;
  public int index;
}

[System.Serializable]
public struct Ingredient{
  public GameObject prefab;
  public Sprite sprite;
  public int index;
}

public class ItemContainer : MonoBehaviour {

  public static ItemContainer Instance { get; private set; }

  [Header("Ingredients")]
  public Ingredient[] ingredients;

  [Header("Recipes")]
  public Recipe[] recipes; 

  private void Awake() {
    Instance = this;
  }

  public Recipe GetRandomRecipe{
    get {
      return recipes[Random.Range(0, recipes.Length)];
    }
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
      if (r.ingredients.Contains(food.foodID))
        return true;
    }

    return false;

  }

  public int GetCompletedRecipe(PlateEntity plate){
    foreach(var r in recipes){
      var rhash = new HashSet<int>(r.ingredients);
      if (rhash.SetEquals(plate.ingredients)) return r.index;
    }
    return -1;
  }

}
