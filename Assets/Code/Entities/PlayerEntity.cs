using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerEntity : PhysicsEntity {

  public float velocity = 4f;
  public float accerlation = 16f;

  private PlayerController controller;

  public override void AwakeEntity() {
    base.AwakeEntity();

    controller = GetComponent<PlayerController>();
  }

  public override void StartEntity() {
    base.StartEntity();

    if (!isMine){
      //controller.RemoteStart();
    }
  }

  public override void UpdateEntity() {
    InterperlateErrorState(4);
  }

  public override PhysicsEntityManager.EntityState GetEntityState() {
    PhysicsEntityManager.EntityState state;
    state.id = entityID;
    state.mask = 1;
    state.position = rigidbody.position;
    state.rotation = rigidbody.rotation;
    state.velocity = rigidbody.velocity;
    state.angularVelocity = rigidbody.angularVelocity;

    return state;
  }

  public override void SaveClientState(int buffertick, byte input) {
    clientStateBuffer[buffertick].input = input;
    clientStateBuffer[buffertick].position = rigidbody.position;
    clientStateBuffer[buffertick].rotation = rigidbody.rotation;
  }

  public override void ApplyInput(byte input) {
    // get player input
    var left = (input & 1) > 0;
    var right = (input & 2) > 0;
    var up = (input & 4) > 0;
    var down = (input & 8) > 0;
    
    // get application of input
    var hor = 0f;
    var ver = 0f;
    if (left && !right) hor = -1f;
    if (right && !left) hor = 1f;
    if (up && !down) ver = 1f;
    if (down && !up) ver = -1f;

    var prev = rigidbody.velocity;
    var steering = new Vector3(hor, 0f, ver).normalized * velocity;
    var cur = Vector3.MoveTowards(rigidbody.velocity, steering, accerlation * Time.fixedDeltaTime);
    
    cur.y = prev.y;
    rigidbody.velocity = cur;

  }

}
