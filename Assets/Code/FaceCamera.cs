using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Texel

[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour {
	void LateUpdate () {
		var activeCam = Camera.main;

		if (activeCam) { // Ignore if the camera is null (No camera assigned)
      var camT = activeCam.transform;
      transform.LookAt(transform.position + camT.rotation * Vector3.forward, camT.rotation * Vector3.up);
		}
	}
}
