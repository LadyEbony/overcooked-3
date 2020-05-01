using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsEntity : Unit {

  [Header("Rigidbody")]
  public new Rigidbody rigidbody;
  public Transform renderTransform;
  public float speed = 4f;

  public PhysicsEntityManager.ClientState[] clientStateBuffer;

  // Add this to all EntityUnits
  public static PhysicsEntity CreateEntity(){
    return CreateEntityHelper(GameInitializer.Instance.playerPrefab);

    // Example: replace null with your prefab
    // return SetEntityHelper(null);
  }

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

    // 8 frame interperlation
    errorpos *= (7f / 8f);
    errorrot = Quaternion.Slerp(errorrot, Quaternion.identity, 1f / 8f);

    renderTransform.position = rigidbody.position + errorpos;
    renderTransform.rotation = rigidbody.rotation * errorrot;
  }

  public PhysicsEntityManager.EntityState GetEntityState(){
    PhysicsEntityManager.EntityState state;
    state.id = entityID;
    state.position = rigidbody.position;
    state.rotation = rigidbody.rotation;
    state.velocity = rigidbody.velocity;
    state.angularVelocity = rigidbody.angularVelocity;

    return state;
  }

  public void SaveClientState(int buffertick, byte input){
    clientStateBuffer[buffertick].input = input;
    clientStateBuffer[buffertick].position = rigidbody.position;
    clientStateBuffer[buffertick].rotation = rigidbody.rotation;
  }

  public void ApplyInput(byte input){
    var left = (input & 1) > 0;
    var right = (input & 2) > 0;
    var up = (input & 4) > 0;
    var down = (input & 8) > 0;
    var jump = (input & 16) > 0;

    var velocity = rigidbody.velocity;
    if (left && !right) velocity.x = -speed;
    if (right && !left) velocity.x = speed;
    if (up && !down) velocity.z = speed;
    if (down && !up) velocity.z = -speed;
    if (jump) velocity.y = speed;
    rigidbody.velocity = velocity;
  }

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
