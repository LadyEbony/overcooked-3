using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : UnitEntity {

  private Rigidbody rb;

  public PlayerEntity owner;
  public float lastServerTime;

  public new static UnitEntity CreateEntity(){
    return CreateEntityHelper(GameInitializer.Instance.aiPrefab);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    rb = GetComponent<Rigidbody>();
  }

  private void LateUpdate() {
    if (owner){
      transform.position = owner.hand.position;
      transform.rotation = owner.hand.rotation;
    }
  }

  public void Pickup(float serverTime, PlayerEntity player){
    // only recognize the latest
    if (serverTime < lastServerTime) return;

    if (owner) owner.grab.held = null;

    rb.isKinematic = true;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    
    owner = player;
    player.grab.held = this;

    lastServerTime = serverTime;
  }

  public void Drop(float serverTime, PlayerEntity player){
    // only recognize the latest
    if (serverTime < lastServerTime) return;

    rb.isKinematic = false;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    
    owner = null;
    player.grab.held = null;

    lastServerTime = serverTime;
  }

  public void Throw(float serverTime, PlayerEntity player){
    // only recognize the latest
    if (serverTime < lastServerTime) return;

    rb.isKinematic = false;
    rb.velocity = transform.forward * 5f;   // i find setting the velocity to work a lot better than force
    rb.angularVelocity = Vector3.zero;
    
    owner = null;
    player.grab.held = null;

    lastServerTime = serverTime;
  }

}
