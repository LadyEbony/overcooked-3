﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlateEntity : ItemEntity {

  public HashSet<int> ingredients;
  public bool ready;

  public new static UnitEntity CreateEntity() {
    return CreateEntityHelper(GameInitializer.Instance.platePrefab);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    ingredients = new HashSet<int>();
  }
     
  public override void StartEntity() {
    base.StartEntity();

    description = GetComponent<ItemDescription>();

    //original:
    //render = GetComponent<Renderer>();

    //Chong: I have adjust this part due to the indicating selection issue
    renderers = new Renderer[1];
    renderers[0]  = GetComponentInChildren<Renderer>();
  }

  [EntityBase.NetEvent('m')]
  public void Combine(int foodID){
    if(ingredients.Add(foodID)){
      var copy = Instantiate(ItemContainer.Instance.foods[foodID]);
      Destroy(copy.GetComponent<Collider>());
      Destroy(copy.GetComponent<ItemDescription>());
      copy.GetComponent<Food>().cooked();
      var c = ingredients.Count;
      copy.transform.localScale *= 0.75f;
      copy.transform.parent = transform;
      copy.transform.localPosition = new Vector3(Mathf.Cos(c * Mathf.PI * 0.5f) * 0.125f, 0.1f, Mathf.Sin(c * Mathf.PI * 0.5f) * 0.125f);
    }
  }

}
