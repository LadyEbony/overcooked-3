﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

  // Player Speed
  public float playerSpeed = 4f;

  [Header("Network")]
  public Vector3 basePosition;
  public Vector3 nextPosition;
  public float baseTime;
  public float nextTime;

  public Quaternion nextRotation;

  private Rigidbody rb;

  private void Awake() {
    rb = GetComponent<Rigidbody>();
  }

  public void LocalStart(){
    gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
  }

  public void RemoteStart(){
    gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
    rb.position = nextPosition;
  }

  public void LocalUpdate(){

    /*
    // Get vertical and horizontal input
    float verticalInput = Input.GetAxis("Vertical");
    float horizontalInput = Input.GetAxis("Horizontal");
    // create movement vectors based on input and camera postition
    Vector3 verticalVector = verticalInput * Camera.main.transform.forward;
    Vector3 horizontalVector = horizontalInput * Camera.main.transform.right;
    Vector3 combinedVector = (verticalVector + horizontalVector).normalized;
    Vector3 movementVelocity = new Vector3(combinedVector.x, 0, combinedVector.z);
    */

    // Jose: Reworked your code to be a bit more consistent
    var ct = Camera.main.transform;

    var fwd = ct.forward;
    var right = ct.right;
    fwd.y = 0;
    right.y = 0;

    fwd = fwd.normalized;
    right = right.normalized;

    var hor = Input.GetAxisRaw("Horizontal");
    var ver = Input.GetAxisRaw("Vertical");

    var delta = hor * right;
    delta += ver * fwd;

    if (delta != Vector3.zero) delta = delta.normalized;

    // Face player towards movement velocity (not fully working)
    // Jose: You can't give LookRotation a zero Vector
    if (delta != Vector3.zero){
      transform.rotation = Quaternion.LookRotation(delta);
    }

    // move player
    rb.velocity = delta * playerSpeed;
  }

  public void RemoteUpdate(){
    // lerp to next position
    var lp = (Time.time - baseTime) / nextTime;
    var pos = Vector3.Lerp(basePosition, nextPosition, lp);

    rb.velocity = (pos - rb.position) / Time.fixedDeltaTime;

    transform.rotation = nextRotation;
  }

}
