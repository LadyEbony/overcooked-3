using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerEntity : EntityUnit {

  private PlayerController controller;

  public new static EntityUnit CreateEntity(){
    return CreateEntityHelper(GameInitializer.Instance.playerPrefab);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    controller = GetComponent<PlayerController>();
  }

  public override void StartEntity() {
    base.StartEntity();

    if (!isMine){
      controller.RemoteStart();
    }
  }

  private void FixedUpdate() {
    if (isMine){
      controller.LocalUpdate();
    } else {
      controller.RemoteUpdate();
    }
  }

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('p', transform.position);
    h.Add('r', transform.rotation);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;
    if (h.TryGetValue('p', out val)){
      controller.nextPosition = (Vector3)val;
      controller.baseTime = Time.time;
      controller.nextTime = updateTimer * 1.5f;   // grace period
    }

    if (h.TryGetValue('r', out val)){
      transform.rotation = (Quaternion)val;
    }
  }

}
