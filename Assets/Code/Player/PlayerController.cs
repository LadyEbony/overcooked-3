using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Speed
    public float playerSpeed = 5.0f;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Get vertical and horizontal input
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        // create movement vectors based on input and camera postition
        Vector3 verticalVector = verticalInput * Camera.main.transform.forward;
        Vector3 horizontalVector = horizontalInput * Camera.main.transform.right;
        Vector3 combinedVector = verticalVector + horizontalVector;
        Vector3 movementVelocity = new Vector3(combinedVector.normalized.x, 0, combinedVector.normalized.z);
        // Face player towards movement velocity (not fully working)
        Quaternion rot = Quaternion.LookRotation(movementVelocity);
        if (rot.y != 0)
            transform.rotation = rot;
        // move player
        rb.velocity = movementVelocity * playerSpeed;
    }
}
