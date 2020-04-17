using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class Plate : EntityUnit {

  public new static EntityUnit CreateEntity(int id){
    var item = Instantiate(UnitManager.Instance.platePrefab);
    return SetEntityHelper(id, item);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();
    
    gameObject.SetActive(true);
  }

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('p', transform.position);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;
    if (h.TryGetValue('p', out val)){
      transform.position = (Vector3)val;
    }
  }

}
