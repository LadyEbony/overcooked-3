using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhysicsEntityManager : UnitManager<PhysicsEntity> {

  [Header("Lock Step")]
  public int tick_number;
  private float timer;

  private void Start() {
    tick_number = 0;
    timer = 0f;
  }

  void Update(){
    if (!NetworkManager.expectedState) return;

    timer += Time.deltaTime;
    while(timer >= Time.fixedDeltaTime){
      timer -= Time.fixedDeltaTime;

      var items = entities.Values.ToArray();
      foreach(var e in items){
        e.UpdateEntity();
      }

      if (NetworkManager.inRoom){
        // master sends response to everyone
        if (NetworkManager.isMaster){
          UpdateNow();
        } 
        // local client sends input to only master
        else if (isMine){
          var options = ExitGames.Client.Photon.LoadBalancing.RaiseEventOptions.Default;
          options.Receivers = ExitGames.Client.Photon.LoadBalancing.ReceiverGroup.MasterClient;
          UpdateNow(options: options);
        }
      }
    }
  }

  public override void Serialize(Hashtable h) {
    base.Serialize(h);

    foreach(var e in entities.Values){
      var temp = new Hashtable();
      if (NetworkManager.isMaster) e.SerializeMaster(temp);
      else e.SerializeClient(temp);

      h.Add(e.entityID, temp);
    }
  }

  public override void Deserialize(Hashtable h) {
    base.Deserialize(h);
    
    //Debug.LogFormat("{0}: {1}", authorityID, isMine);

    // a message from master client (but it's a response from us)
    if (isMine){
      foreach(var e in h){
        if (e.Key is int){
          var id = (int)e.Key;
          var hashtable = (Hashtable)e.Value;
          PhysicsEntity item;
          if (entities.TryGetValue(id, out item) && hashtable.Count > 0){
            item.DeserializeClient(hashtable);
          }
        }
      }
    } 
    // an input message
    else if (NetworkManager.isMaster) {
      DeserializeHelper(h, (item, hash) => item.DeserializeMaster(hash));
    } 
    // a message from master client
    else {
      DeserializeHelper(h, (item, hash) => item.DeserializeClient(hash));
    }
  }

}
