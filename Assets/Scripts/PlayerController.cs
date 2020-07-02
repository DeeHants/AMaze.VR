using System;
using UnityEngine;

[RequireComponent (typeof (CharacterController))]
public class PlayerController : MonoBehaviour {
    [Tooltip ("Movement speed (in units/s)")]
    public float moveSpeed = 3;
    [Tooltip ("Rotational speed (in degrees/s)")]
    public float rotateSpeed = 50;

    [Tooltip ("Use the camera as the forward direction")]
    public bool cameraForward = true;

    private CharacterController characterController = null;

    private float fallSpeed = 0;

    private Transform mainObject = null;
    private Transform offsetObject = null;
    private Transform cameraObject = null;

    void Start () {
        // Get the character controller
        this.characterController = this.GetComponent<CharacterController> ();

        // Get the object that defines our forward direction
        this.mainObject = this.transform;
        this.offsetObject = mainObject.Find ("Camera offset");
        this.cameraObject = this.offsetObject.Find ("Main Camera");

        if (this.cameraForward) { this.mainObject = this.cameraObject; }
    }

    void Update () {
        // Reset camera position to match the player object if the Reset input is pressed
        if (Input.GetButtonDown ("Reset")) {
            this.ResetOffset ();
        }

        // Get input data from keyboard or controller
        float forwardSpeed = Input.GetAxis ("Vertical");
        float sidewaysSpeed = Input.GetAxis ("Horizontal");
        float rotationalSpeed = Input.GetAxis ("Horizontal2");

        // Move forward and sideways, making sure we stay on the ground
        Vector3 moveDirection = Vector3.zero;
        if (this.characterController.isGrounded) {
            moveDirection += this.mainObject.forward * forwardSpeed;
            moveDirection += this.mainObject.right * sidewaysSpeed;
            moveDirection *= this.moveSpeed * Time.deltaTime;
            // No flying!
            this.fallSpeed = 0;
            moveDirection.y = 0;
        } else {
            // Obey the laws of gravity!
            this.fallSpeed += Physics.gravity.y * Time.deltaTime;
            moveDirection.y = this.fallSpeed * Time.deltaTime;
        }
        this.characterController.Move (moveDirection);

        // Rotate
        Vector3 rotateDirection = new Vector3 (0, rotationalSpeed, 0);
        rotateDirection *= this.rotateSpeed * Time.deltaTime;
        this.transform.Rotate (rotateDirection, Space.Self);
    }

    /// <summary>
    /// Resets the camera position to the center of the player object  
    /// </summary>
    void ResetOffset () {
        // Get the camera's current position
        Vector3 offsetPosition = this.cameraObject.localPosition;

        // Create the required offset values
        offsetPosition = new Vector3 (
            -offsetPosition.x,
            0, // Camera should stay at head height
            -offsetPosition.z
        );

        // Change the offset
        this.offsetObject.localPosition = offsetPosition;
    }
}