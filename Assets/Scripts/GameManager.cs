using UnityEngine;

/// <summary>
/// Game state values
/// </summary>
internal enum GameState {
    Startup,
    Showing,
    ShowingComplete,
    Running,
    Hiding,
    HidingComplete,
}

/// <summary>
/// Manages the game state and restarts.
/// </summary>
public class GameManager : MonoBehaviour {
    [Tooltip ("Speed at which the walls hide or show at the end/start of the game")]
    public float shrinkSpeed = 0.2f;

    [Header ("Objects")]
    [Tooltip ("Player object")]
    public GameObject playerObject = null;

    internal GameState state = GameState.Startup;

    private MazeController maze = null;

    void Start () {
        this.maze = this.GetComponent<MazeController> ();

        // Create the initial maze
        this.NewGame ();
    }

    void Update () {
        this.CheckGameState ();
        this.CheckMazeHeight ();
    }

    /// <summary>
    /// Starts a new game.
    /// </summary>
    public void NewGame () {
        // If the game is running, hide
        if (this.state != GameState.Startup) {
            this.state = GameState.Hiding;
        }
        this.CheckGameState ();
    }

    /// <summary>
    /// Handles the game state machine.
    /// </summary>
    void CheckGameState () {
        if (this.state == GameState.Startup || this.state == GameState.HidingComplete) {
            // Done hiding (or starting up), create a new game

            if (this.state == GameState.Startup) {
                // Hide it immediately
                Vector3 scale = this.maze.container.transform.localScale;
                scale.y = 0;
                this.maze.container.transform.localScale = scale;
            } else if (this.state == GameState.HidingComplete) {
                // Clear up the old maze
                maze.RemoveWalls ();

                // Swap the start and finish
                Vector2Int temp = maze.start;
                maze.start = maze.finish;
                maze.finish = temp;
            }

            // Create the new maze
            this.maze.CreateMaze ();

            if (this.state == GameState.Startup) {
                // Move the player to the start position
                if (playerObject != null) {
                    Vector3 startPosition = maze.GetCellPosition (maze.start);
                    playerObject.transform.position = startPosition;
                }
            }

            // Show maze again
            this.state = GameState.Showing;
        }

        if (this.state == GameState.ShowingComplete) {
            // Done showing so enter the running state
            this.state = GameState.Running;
        }
    }

    /// <summary>
    /// Checks the game state, and whether it needs to be hidden or shown.
    /// </summary>
    void CheckMazeHeight () {
        Transform container = maze.container;
        Vector3 scale = container.localScale;
        if (this.state == GameState.Hiding) {
            // Decrease the height of the maze object
            scale.y -= this.shrinkSpeed * Time.deltaTime;
            if (scale.y <= 0) {
                scale.y = 0;
                this.state = GameState.HidingComplete;
            }
        }
        if (this.state == GameState.Showing) {
            // Increase the height of the maze object
            scale.y += this.shrinkSpeed * Time.deltaTime;
            if (scale.y >= 1) {
                scale.y = 1;
                this.state = GameState.ShowingComplete;
            }
        }
        if (container.localScale.y != scale.y) { container.localScale = scale; }
    }
}