using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class ItemEntity : UnitEntity, IInteractable {

  private ItemDescription description;
  private Rigidbody rb;
  private new Renderer renderer;

  public PlayerEntity owner;
  public float lastServerTime = float.MinValue;
  public int itemIndex;

  public Material defaultMaterial => description.defaultMaterial;
  public Material selectedMaterial => description.selectedMaterial;

  public new static UnitEntity CreateEntity(){
    return CreateEntityHelper(GameInitializer.Instance.aiPrefab);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    rb = GetComponent<Rigidbody>();
  }

  public override void StartEntity() {
    base.StartEntity();

    var prefab = ItemContainer.Instance.prefabs[itemIndex];
    var obj = Instantiate(prefab, transform);
    description = obj.GetComponent<ItemDescription>();

    renderer = obj.GetComponent<Renderer>();
  }

  private void LateUpdate() {
    if (owner){
      transform.position = owner.hand.position;
      transform.rotation = owner.hand.rotation;
    }
  }

  public bool IsInteractable(PlayerEntity player){
    return !player.grab.held;
  }

  public void Activate(PlayerEntity player){
    RaiseEvent('p', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID);
  }

  public void OnSelect(PlayerEntity player) {
    renderer.material = selectedMaterial;
  }

  public void OnDeselect(PlayerEntity player) {
    renderer.material = defaultMaterial;
  }

  [EntityBase.NetEvent('p')]
  public void Pickup(float serverTime, int playerAID, int playerEID){
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

    lastServerTime = serverTime;
  }

  [EntityBase.NetEvent('d')]
  public void Drop(float serverTime, int playerAID, int playerEID){
    // only recognize the latest
    if (serverTime < lastServerTime) return;
    var player = GameInitializer.Instance.Entity<PlayerEntity>(playerAID, playerEID);
    if (player == null) return;

    rb.isKinematic = false;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    
    owner = null;
    player.grab.held = null;

    lastServerTime = serverTime;
  }

  [EntityBase.NetEvent('t')]
  public void Throw(float serverTime, int playerAID, int playerEID){
    // only recognize the latest
    if (serverTime < lastServerTime) return;
    var player = GameInitializer.Instance.Entity<PlayerEntity>(playerAID, playerEID);
    if (player == null) return;

    rb.isKinematic = false;
    rb.velocity = transform.forward * 10f;   // i find setting the velocity to work a lot better than force
    rb.angularVelocity = Vector3.zero;
    
    owner = null;
    player.grab.held = null;

    lastServerTime = serverTime;
  }

}
