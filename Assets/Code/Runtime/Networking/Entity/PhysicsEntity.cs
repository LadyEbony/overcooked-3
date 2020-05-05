using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(Rigidbody))]
public abstract class PhysicsEntity : Unit {

  [Header("Rigidbody")]
  public new Rigidbody rigidbody;
  public Transform renderTransform;

  public PhysicsEntityManager.ClientState[] clientStateBuffer;

  public static PhysicsEntity CreateEntityHelper(GameObject prefab){
    var obj = Instantiate(prefab);
    var comp = obj.GetComponent<PhysicsEntity>();
    comp.AwakeEntity();
    return comp;
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    clientStateBuffer = new PhysicsEntityManager.ClientState[PhysicsEntityManager.BUFFER_SIZE];
  }

  public override void StartEntity() {
    base.StartEntity();

    rigidbody = GetComponent<Rigidbody>();
    renderTransform = transform.GetChild(0);
  }

  public override void UpdateEntity() {
    base.UpdateEntity();

    InterperlateErrorState(8);
  }

  protected void InterperlateErrorState(int frames){
    errorpos *= (frames - 1) / (float)frames;
    errorrot = Quaternion.Slerp(errorrot, Quaternion.identity, 1f / frames);

    renderTransform.position = rigidbody.position + errorpos;
    renderTransform.rotation = rigidbody.rotation * errorrot;
  }

  /// <summary>
  /// Get entity state data for server to send out
  /// </summary>
  /// <returns></returns>
  public abstract PhysicsEntityManager.EntityState GetEntityState();

  /// <summary>
  /// Save data into client predication buffer
  /// </summary>
  /// <param name="buffertick"></param>
  /// <param name="input"></param>
  public abstract void SaveClientState(int buffertick, byte input);

  /// <summary>
  /// Apply input to entity
  /// </summary>
  /// <param name="input"></param>
  public abstract void ApplyInput(byte input);

  private Vector3 prevpos, errorpos;
  private Quaternion prevrot, errorrot;

  private PhysicsEntityManager.EntityState prevEntityState;

  public void CapturePrevState(){
    prevpos = rigidbody.position + errorpos;
    prevrot = rigidbody.rotation * errorrot;
  }

  public void ComparePrevState(){
    // force fix if too far
    if ((prevpos - rigidbody.position).sqrMagnitude >= 4f){
      errorpos = Vector3.zero;
      errorrot = Quaternion.identity;
    } 
    // interperloate to correct position
    else {
      errorpos = prevpos - rigidbody.position;
      errorrot = Quaternion.Inverse(rigidbody.rotation) * prevrot;
    }
  }

  public void StorePrevEntityState(PhysicsEntityManager.EntityState state){
    prevEntityState = state;
  }

  public float GetError(int buffertick){
    var client = clientStateBuffer[buffertick];
    return (prevEntityState.position - client.position).sqrMagnitude + 
      (1f - Quaternion.Dot(prevEntityState.rotation, client.rotation));
  }

  public void RestorePrevEntityState(){
    rigidbody.position = prevEntityState.position;
    rigidbody.rotation = prevEntityState.rotation;
    rigidbody.velocity = prevEntityState.velocity;
    rigidbody.angularVelocity = prevEntityState.angularVelocity;
  }

}
