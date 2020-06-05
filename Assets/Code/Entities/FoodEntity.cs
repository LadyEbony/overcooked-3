using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEntity : ItemEntity {

  public int foodID;

  public int cutCurrent;
  public int cookCurrent;

  public int cutMax => description.cutAmount;
  public int cookMax => description.cookAmount;
  public bool isProcessingDone => cutCurrent <= 0 && cookCurrent <= 0 && cookCurrent != -2;

  public new static UnitEntity CreateEntity() {
    return CreateEntityHelper(GameInitializer.Instance.foodPrefab);
  }

  public override void StartEntity() {
    base.StartEntity();

    var prefab = ItemContainer.Instance.ingredients[foodID].prefab;
    var obj = Instantiate(prefab, transform);
    description = obj.GetComponent<ItemDescription>();

    cutCurrent = description.cutAmount;
    cookCurrent = description.cookAmount;

        //original:
        //renderer = obj.GetComponent<Renderer>();

        //Chong: find the # of children then set up a Renderer type array.
        //After that, put all the renderers under the object into the array.
        int children = obj.transform.childCount;
        renderers = new Renderer[children];
        for (int i = 0; i < children; i++)
        {
            renderers[i] = obj.transform.GetChild(i).transform.GetComponentInChildren<Renderer>();
        }
    }
  
  public override int IsInteractable(PlayerEntity player) {
    var held = player.held;
    if (held == null){
      return 2;
    } 
    var plate = held as PlateEntity;
    if (plate != null) {
      // food has the most priority
      var result = isProcessingDone && ItemContainer.Instance.GetRecipeResult(plate, this);
      if (result) return 1;
    }
    return int.MaxValue;
  }

  public override void Activate(PlayerEntity player) {
    var held = player.held;
    if (held == null){
      RaiseEvent('p', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID);
    }

    var plate = held as PlateEntity;
    if (plate != null) {
      // food has the most priority
      var result = ItemContainer.Instance.GetRecipeResult(plate, this);
      plate.RaiseEvent('m', true, foodID);
      RaiseEvent('s', true);
    }
  }

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('f', (byte)foodID);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    foodID = (int)(byte)h['f'];
  }

  public float cutPercentage => (float)(cutMax - cutCurrent) / cutMax;
  public float cookPercentage => (float)(cookMax - cookCurrent) / cookMax;

  [EntityBase.NetEvent('c')]
  public void Cut(){
    cutCurrent = Mathf.Max(0, cutCurrent - 1);

    // get cabient that this item belongs to
    var cab = DoubleDictionary<Cabient, ItemEntity>.Get(this);
    if (cab){
      // You have access to the cabient class
      // Perhaps tell the cabient to play a cutting animation
      //Debug.LogWarning("Insert code to have cabient play knife cutting animation");
      this.description.GetComponent<Food>().slicing();
    }
  }

  [EntityBase.NetEvent('k')]
  public void Cook(){
    if (cookCurrent == 0)
    {
        cookCurrent = -2;
        this.description.GetComponent<Food>().burnt();  //Tsq
    }
    else if (cookCurrent > 0) cookCurrent -= 1;

    // get cabient that this item belongs to
    var cab = DoubleDictionary<Cabient, ItemEntity>.Get(this);
    if (cab){
      // You have access to the cabient class
      // Perhaps tell the cabient to play a cutting animation
      //Debug.LogWarning("Insert code to have cooking animation IF ANYTHING");
      if (cookCurrent == 0)
      {
         this.description.GetComponent<Food>().cooked();
      }
    }
  }

}
