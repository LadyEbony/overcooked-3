using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerEntity : UnitEntity {

  public Transform hand;

  public PlayerController controller;
  public GrabandDrop grab;

  public new static UnitEntity CreateEntity(){
    return CreateEntityHelper(GameInitializer.Instance.playerPrefab);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    controller = GetComponent<PlayerController>();
    grab = GetComponent<GrabandDrop>();
  }

  public override void StartEntity() {
    base.StartEntity();

    if (isMine) {
      controller.LocalStart();
    } else {
      controller.RemoteStart();
    }
  }

  public override void UpdateEntity() {
    base.UpdateEntity();

    if (isMine) {
      controller.LocalUpdate();
      grab.LocalUpdate();
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

    var pos = (Vector3)h['p'];
    controller.basePosition = transform.position;
    controller.nextPosition = pos;
    controller.baseTime = Time.time;
    controller.nextTime = updateTimer * 1.25f;

    var rot = (Quaternion)h['r'];
    controller.nextRotation = rot;
  }

}
