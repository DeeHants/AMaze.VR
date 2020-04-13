using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Tooltip ("Player object")]
    public GameObject playerObject = null;

    void Start () {
        if (playerObject != null) {
            // We need a reference to the maze generator
            MazeController maze = this.GetComponent<MazeController> ();

            // Get the start position, and move the player object to that location
            Vector3 startPosition = maze.GetCellPosition (maze.start);
            playerObject.transform.position = startPosition;
        }
    }
}