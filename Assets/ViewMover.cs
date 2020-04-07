using System;
using UnityEngine;

public class ViewMover : MonoBehaviour {
    public int speed = 1;

    // Update is called once per frame
    void Update () {
        // Get input data from keyboard or controller
        double directionalSpeed = Input.GetAxis ("Vertical");
        double rotationalSpeed = Input.GetAxis ("Horizontal");
        double multiplier = this.speed * Time.deltaTime;

        // Get the current postion and rotation
        Vector3 position = this.transform.position;
        Vector3 rotation = this.transform.rotation.eulerAngles;

        // Rotate based on the horizontal input
        rotation.y += (float) (rotationalSpeed * 25 * multiplier);
        Transform camera = this.transform.Find ("Main Camera");
        double rotationRadian = Math.PI * (camera.transform.rotation.eulerAngles.y) / 180.0;

        // Use trigonometry to adjust the position based ont he current direction
        double xVector = Math.Sin (rotationRadian);
        double zVector = Math.Cos (rotationRadian);
        position.x += (float) (directionalSpeed * xVector * multiplier);
        position.z += (float) (directionalSpeed * zVector * multiplier);

        // Update the object
        this.transform.position = position;
        this.transform.rotation = Quaternion.Euler (rotation);
    }
}