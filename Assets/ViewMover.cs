using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMover : MonoBehaviour {
    public int speed = 0;

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        // Get input data from keyboard or controller
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");

        // Update player position based on input
        Vector3 position = this.transform.position;
        position.x += moveHorizontal * this.speed * Time.deltaTime;
        position.z += moveVertical * this.speed * Time.deltaTime;
        this.transform.position = position;
    }
}