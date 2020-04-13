using UnityEngine;

/// <summary>
/// Handles the finish trigger and starts a new game
/// </summary>
public class FinishHandler : MonoBehaviour {
    [Header ("Objects")]
    [Tooltip ("Maze object")]
    public GameObject mazeObject = null;
    [Tooltip ("Player object")]
    public GameObject playerObject = null;

    // Handles the trigger, and calls the game manager to start a new game
    void OnTriggerEnter (Collider other) {
        // Check it it was triggered by the player object
        if (playerObject != null && other.gameObject == playerObject) {
            if (mazeObject != null) {
                // Call the game manager
                GameManager game = mazeObject.GetComponent<GameManager> ();
                game.NewGame();
            }
        }
    }
}