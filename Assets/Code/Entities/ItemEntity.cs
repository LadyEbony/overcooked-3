using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TMPro;

public class ItemEntity : UnitEntity, IInteractable {

  private Rigidbody rb;
  private new Renderer renderer;
  private TextMeshPro debug;

  public PlayerEntity owner;
  public Cabient cabient;
  public ItemDescription description;
  public float lastServerTime = float.MinValue;
  public int itemIndex;

  public Material defaultMaterial => description.defaultMaterial;
  public Material selectedMaterial => description.selectedMaterial;

  public new static UnitEntity CreateEntity() {
    return CreateEntityHelper(GameInitializer.Instance.aiPrefab);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    rb = GetComponent<Rigidbody>();
    debug = GetComponentInChildren<TextMeshPro>();
  }

  public override void StartEntity() {
    base.StartEntity();

    var prefab = ItemContainer.Instance.prefabs[itemIndex];
    var obj = Instantiate(prefab, transform);
    description = obj.GetComponent<ItemDescription>();

    renderer = obj.GetComponent<Renderer>();
  }

  private void LateUpdate() {
    if (owner) {
      transform.position = owner.hand.position;
      transform.rotation = owner.hand.rotation;
    }

    if (debug){
      debug.text = description.cut >= 0 ? description.cut.ToString() : string.Empty;
    }
  }

  public bool IsInteractable(PlayerEntity player) {
    return !player.grab.held && !cabient;
  }

  public void Activate(PlayerEntity player) {
    RaiseEvent('p', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID);
  }

  public void ActivateAlt(PlayerEntity player) { }

  public void OnSelect(PlayerEntity player) {
    renderer.material = selectedMaterial;
  }

  public void OnDeselect(PlayerEntity player) {
    renderer.material = defaultMaterial;
  }

  [EntityBase.NetEvent('p')]
  public void Pickup(float serverTime, int playerAID, int playerEID) {
    // only recognize the latest
    if (serverTime < lastServerTime) return;
    var player = GameInitializer.Instance.Entity<PlayerEntity>(playerAID, playerEID);
    if (player == null) return;

    if (owner) owner.grab.held = null;

    rb.isKinematic = true;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    owner = player;
    player.grab.held = this;

    if (cabient){
      if (cabient.item == this){
        cabient.item = null;
      }
      cabient = null;
    }

    lastServerTime = serverTime;
  }

  [EntityBase.NetEvent('d')]
  public void Drop(float serverTime, int playerAID, int playerEID) {
    // only recognize the latest
    if (serverTime < lastServerTime) return;
    var player = GameInitializer.Instance.Entity<PlayerEntity>(playerAID, playerEID);
    if (player == null) return;

    rb.isKinematic = false;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    owner = null;
    player.grab.held = null;

    if (cabient){
      if (cabient.item == this){
        cabient.item = null;
      }
      cabient = null;
    }

    lastServerTime = serverTime;
  }

  [EntityBase.NetEvent('t')]
  public void Throw(float serverTime, int playerAID, int playerEID) {
    // only recognize the latest
    if (serverTime < lastServerTime) return;
    var player = GameInitializer.Instance.Entity<PlayerEntity>(playerAID, playerEID);
    if (player == null) return;

    rb.isKinematic = false;
    rb.velocity = transform.forward * 10f;   // i find setting the velocity to work a lot better than force
    rb.angularVelocity = Vector3.zero;

    owner = null;
    player.grab.held = null;

    if (cabient){
      if (cabient.item == this){
        cabient.item = null;
      }
      cabient = null;
    }

    lastServerTime = serverTime;
  }

  [EntityBase.NetEvent('l')]
  public void Place(float serverTime, int playerAID, int playerEID, int cabientID) {
    // only recognize the latest
    if (serverTime < lastServerTime) return;
    var player = GameInitializer.Instance.Entity<PlayerEntity>(playerAID, playerEID);
    if (player == null) return;
    var cab = Cabient.cabients[cabientID];

    rb.isKinematic = true;
    rb.position = cab.placeTransform.position;
    rb.rotation = Quaternion.identity;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    owner = null;
    player.grab.held = null;

    cabient = cab;
    cabient.item = this;

    lastServerTime = serverTime;
  }

  [EntityBase.NetEvent('c')]
  public void Cut(){
    description.cut = Mathf.Max(0, description.cut - 1);
  }

}
