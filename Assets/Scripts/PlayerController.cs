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

    void Start () {
        // Get the character controller
        this.characterController = this.GetComponent<CharacterController> ();
    }

    void Update () {
        // Get input data from keyboard or controller
        float forwardSpeed = Input.GetAxis ("Vertical");
        float sidewaysSpeed = Input.GetAxis ("Horizontal");
        float rotationalSpeed = Input.GetAxis ("Horizontal2");

        // Get the object that defines our forward direction
        Transform mainObject = this.transform;
        if (this.cameraForward) { mainObject = mainObject.Find ("Main Camera"); }

        // Move forward and sideways, making sure we stay on the ground
        Vector3 moveDirection = mainObject.forward * forwardSpeed;
        moveDirection += mainObject.right * sidewaysSpeed;
        moveDirection *= this.moveSpeed * Time.deltaTime;
        moveDirection.y = 0; // No flying!
        this.characterController.Move (moveDirection);

        // Rotate
        Vector3 rotateDirection = new Vector3 (0, rotationalSpeed, 0);
        rotateDirection *= this.rotateSpeed * Time.deltaTime;
        this.transform.Rotate (rotateDirection, Space.Self);
    }
}