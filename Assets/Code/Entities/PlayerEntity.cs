using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerEntity : UnitEntity {

  public Color[] playerColors = new Color[]{
    Color.black,
    Color.blue,
    Color.cyan,
    Color.gray,
    Color.green,
    Color.magenta,
    Color.red,
    Color.white,
    Color.yellow
  };

  public Transform hand;

  public PlayerController controller;
  public GrabandDrop grab;

  public ItemEntity held => DoubleDictionary<PlayerEntity, ItemEntity>.Get(this);

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

    var renderers = GetComponentsInChildren<Renderer>();
    foreach(var r in renderers){
      var materials = r.materials;
      foreach(var m in materials){
        m.color = playerColors[authorityID % playerColors.Length];
      }
    }

  }

  public override void UpdateEntity() {
    base.UpdateEntity();

    if (isMine) {
      controller.LocalUpdate();
      grab.LocalUpdate();
    }
  }

  private void FixedUpdate() {
    if (!isMine){
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
    controller.nextTime = 0.1f * 1.25f;

    var rot = (Quaternion)h['r'];
    controller.nextRotation = rot;
  }

}
