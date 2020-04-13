using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Tooltip ("Player object")]
    public GameObject playerObject = null;

    private MazeController maze = null;

    void Start () {
        this.maze = this.GetComponent<MazeController> ();

        // Create the initial maze
        this.NewGame ();
    }

    void NewGame () {
        // Create the new maze
        this.maze.CreateMaze ();

        // Move the player to the start position
        if (playerObject != null) {
            Vector3 startPosition = maze.GetCellPosition (maze.start);
            playerObject.transform.position = startPosition;
        }
    }
}