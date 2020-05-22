using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TMPro;

public abstract class ItemEntity : UnitEntity, IInteractableBase {

  protected Rigidbody rb;
  //protected new Renderer renderer;
  protected TextMeshPro debug;

  public ItemDescription description;
  public float lastServerTime = float.MinValue;

  public Material defaultMaterial => description.defaultMaterial;
  public Material selectedMaterial => description.selectedMaterial;
  public PlayerEntity owner => DoubleDictionary<PlayerEntity, ItemEntity>.Get(this);

  //Chong: I use "Render[]" instead of "Renderer" to solve indicating selection issue
  protected Renderer[] renderers;


  public override void AwakeEntity() {
    base.AwakeEntity();

    rb = GetComponent<Rigidbody>();
    debug = GetComponentInChildren<TextMeshPro>();
  }

  private void LateUpdate() {
    var owner = this.owner;
    if (owner) {
      transform.position = owner.hand.position;
      transform.rotation = owner.hand.rotation;
    }
  }

  public virtual int IsInteractable(PlayerEntity player) {
    var held = player.held;
    return held == null ? 2 : int.MaxValue;
  }

  public virtual void Activate(PlayerEntity player) {
    var held = player.held;
    if (held == null){
      RaiseEvent('p', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID);
    }
  }

  //Chong: set all the renderers under the object to selectedMaterial
  public void OnSelect(PlayerEntity player) {
    foreach(Renderer renderer in renderers)
        {
            renderer.material = selectedMaterial;
        }
    }

  public void OnDeselect(PlayerEntity player) {
        foreach (Renderer renderer in renderers)
        {

            renderer.material = defaultMaterial;
        }
  }

  [EntityBase.NetEvent('p')]
  public void Pickup(float serverTime, int playerAID, int playerEID) {
    // only recognize the latest
    if (serverTime < lastServerTime) return;
    var player = GameInitializer.Instance.Entity<PlayerEntity>(playerAID, playerEID);
    if (player == null) return;

    DoubleDictionary<PlayerEntity, ItemEntity>.Remove(this);

    rb.isKinematic = true;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    DoubleDictionary<PlayerEntity, ItemEntity>.Set(player, this);
    DoubleDictionary<Cabient, ItemEntity>.Remove(this);

    lastServerTime = serverTime;
  }

  [EntityBase.NetEvent('d')]
  public void Drop(float serverTime) {
    // only recognize the latest
    if (serverTime < lastServerTime) return;

    rb.isKinematic = false;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    DoubleDictionary<PlayerEntity, ItemEntity>.Remove(this);
    DoubleDictionary<Cabient, ItemEntity>.Remove(this);

    lastServerTime = serverTime;
  }

  [EntityBase.NetEvent('t')]
  public void Throw(float serverTime) {
    // only recognize the latest
    if (serverTime < lastServerTime) return;

    rb.isKinematic = false;
    rb.velocity = transform.forward * 10f;   // i find setting the velocity to work a lot better than force
    rb.angularVelocity = Vector3.zero;

    DoubleDictionary<PlayerEntity, ItemEntity>.Remove(this);
    DoubleDictionary<Cabient, ItemEntity>.Remove(this);

    lastServerTime = serverTime;
  }

  [EntityBase.NetEvent('l')]
  public void Place(float serverTime, int cabientID) {
    var cab = Cabient.cabients[cabientID];

    // only recognize the latest
    if (serverTime < lastServerTime){
      // a late message, still place near station
      rb.isKinematic = false;
      rb.position = cab.placeTransform.position;
      rb.rotation = Quaternion.identity;
      rb.velocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;

      transform.position = cab.placeTransform.position;
      transform.rotation = Quaternion.identity;

      return;
    }
    
    // pop any item on the cabient
    var item = DoubleDictionary<Cabient, ItemEntity>.Get(cab);
    if (item){
      var itemrb = item.rb;
      itemrb.isKinematic = false;
      itemrb.velocity = Vector3.zero;
      itemrb.angularVelocity = Vector3.zero;
    }
    DoubleDictionary<Cabient, ItemEntity>.Remove(cab);

    rb.isKinematic = true;
    rb.position = cab.placeTransform.position;
    rb.rotation = Quaternion.identity;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    transform.position = cab.placeTransform.position;
    transform.rotation = Quaternion.identity;

    DoubleDictionary<PlayerEntity, ItemEntity>.Remove(this);
    DoubleDictionary<Cabient, ItemEntity>.Set(cab, this);

    lastServerTime = serverTime;
  }

  [EntityBase.NetEvent('s')]
  public void Destroy(){
    DestroyEntity();

    if (isMine){
      UnitEntityManager.Local.Deregister(this);
      Destroy(gameObject);
    } else {
      gameObject.SetActive(false);
    }
  }

  public override void DestroyEntity() {
    base.DestroyEntity();

    DoubleDictionary<PlayerEntity, ItemEntity>.Remove(this);
    DoubleDictionary<Cabient, ItemEntity>.Remove(this);
  }

}
