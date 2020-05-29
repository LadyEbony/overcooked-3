using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UnitEntityManager : UnitManager<UnitEntity> {

  public static UnitEntityManager Local;
  
  public override void Serialize(Hashtable h) {
    base.Serialize(h);

    foreach(var e in entities.Values){
      var temp = new Hashtable();
      // we only send the full entity data when their auto timers are triggered
      // otherwise send empty
      e.Serialize(temp);
      h.Add(e.entityID, temp);
    }
  }

  public override void Deserialize(Hashtable h) {
    base.Deserialize(h);

    DeserializeHelper(h, (item, hash) => item.Deserialize(hash));
  }

}
