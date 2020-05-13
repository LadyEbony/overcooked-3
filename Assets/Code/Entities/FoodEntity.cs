using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEntity : ItemEntity {

  public int foodID;

  public new static UnitEntity CreateEntity() {
    return CreateEntityHelper(GameInitializer.Instance.foodPrefab);
  }

  public override void StartEntity() {
    base.StartEntity();

    var prefab = ItemContainer.Instance.foods[foodID];
    var obj = Instantiate(prefab, transform);
    description = obj.GetComponent<ItemDescription>();

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
      var result = ItemContainer.Instance.GetRecipeResult(plate, this);
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

}
