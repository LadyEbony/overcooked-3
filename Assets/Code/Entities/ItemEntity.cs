using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class ItemEntity : UnitEntity, IInteractable {

  private Rigidbody rb;
  private new Renderer renderer;

  public Material defaultMaterial;
  public Material selectedMaterial;

  public PlayerEntity owner;
  public float lastServerTime = float.MinValue;

  public new static UnitEntity CreateEntity(){
    return CreateEntityHelper(GameInitializer.Instance.aiPrefab);
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    rb = GetComponent<Rigidbody>();
    renderer = GetComponent<Renderer>();
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
    UnitEntityManager.Local.Pickup(player, this);
  }

  public void OnSelect(PlayerEntity player) {
    renderer.material = selectedMaterial;
  }

  public void OnDeselect(PlayerEntity player) {
    renderer.material = defaultMaterial;
  }

  public void Pickup(float serverTime, PlayerEntity player){
    // only recognize the latest
    Debug.Log("pickup");
    Debug.Log(serverTime);
    if (serverTime < lastServerTime) return;
    Debug.Log("pickup2");

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

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    if (owner){
      h.Add('a', owner.authorityID);
      h.Add('e', owner.entityID);
    }
    
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    if (h.ContainsKey('a')){
      var auth = (int)h['a'];
      var eith = (int)h['e'];
      var own = GameInitializer.Instance.Entity<PlayerEntity>(auth, eith);

      if (own){
        owner = own;
        owner.grab.held = this;;
        return;
      }
    }

    if (owner){
      owner.grab.held = null;
    }
    owner = null;
  }

}
