using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UnitEntityManager : UnitManager<UnitEntity> {

  public static UnitEntityManager Local;

  void Update(){
    if (!NetworkManager.expectedState) return;

    var items = entities.Values.ToArray();
    foreach(var e in items){
      e.UpdateEntity();
    }

    if (NetworkManager.inRoom && isMine){
      UpdateNow();
    }
  }
  
  public override void Serialize(Hashtable h) {
    base.Serialize(h);

    foreach(var e in entities.Values){
      var temp = new Hashtable();
      // we only send the full entity data when their auto timers are triggered
      // otherwise send empty
      if (e.UpdateReady){
        e.Serialize(temp);
      }
      h.Add(e.entityID, temp);
    }
  }

  public override void Deserialize(Hashtable h) {
    base.Deserialize(h);

    DeserializeHelper(h, (item, hash) => item.Deserialize(hash));
  }

  // RaiseEvent calls a function on every client
  // including this local client
  // it's recognized by the 'g' key
  public void Pickup(PlayerEntity player, ItemEntity item){
    RaiseEvent('g', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID, item.authorityID, item.entityID, (byte)0);
  }

  public void Drop(PlayerEntity player, ItemEntity item){
    RaiseEvent('g', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID, item.authorityID, item.entityID, (byte)1);
  }

  public void Throw(PlayerEntity player, ItemEntity item){
    RaiseEvent('g', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID, item.authorityID, item.entityID, (byte)2);
  }

  [NetEvent('g')]
  public void __Pickup(float serverTime, int playerActorID, int playerEntityID, int itemActorID, int itemEntityID, byte evt){
    var item = GameInitializer.Instance.Entity<ItemEntity>(itemActorID, itemEntityID);
    Debug.Log(item);
    if (item == null) return;

    var player = GameInitializer.Instance.Entity<PlayerEntity>(playerActorID, playerEntityID);
    Debug.Log(player);
    if (player == null) return;

    if (evt == 0)
      item.Pickup(serverTime, player);
    else if (evt == 1)
      item.Drop(serverTime, player);
    else if (evt == 2)
      item.Throw(serverTime, player);
  }

}
