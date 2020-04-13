using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour {
    [Header ("Maze geometry")]
    [Tooltip ("Maze dimensions (in cells)")]
    public Vector2Int size = new Vector2Int (10, 10);
    [Tooltip ("Width of the corridor (in units)")]
    public int width = 1;
    [Tooltip ("Cell position of the start marker")]
    // Start and finish default to opposite sides, 3 up/down from the corner
    public Vector2Int start = new Vector2Int (0, 2);
    [Tooltip ("Cell position of the start marker")]
    public Vector2Int finish = new Vector2Int (9, 7);
    [Tooltip ("Map seed value")]
    public int randomSeed = 0;

    [Header ("Objects")]
    [Tooltip ("Prefab/asset used for the wall (Must have a Z size that matches the corridor width)")]
    public GameObject wallObject = null;
    [Tooltip ("Container of all the wall sections")]
    public GameObject wallContainer = null;

    [Tooltip ("Object for the start marker")]
    public GameObject startObject = null;
    [Tooltip ("Object for the finish marker")]
    public GameObject finishObject = null;

    private List<GameObject> wallSections = new List<GameObject> ();

    public void CreateMaze () {
        // Move the start and finish markers
        this.startObject.transform.position = this.GetCellPosition (this.start);
        this.finishObject.transform.position = this.GetCellPosition (this.finish);

        // Generate the maze!
        MazeCell[, ] cells = MazeGenerator.GenerateMaze (
            this.randomSeed,
            this.size,
            this.start,
            this.finish
        );

        // Add all the walls
        for (int x = 0; x < this.size.x; x++) {
            for (int z = 0; z < this.size.y; z++) {
                MazeCell cell = cells[x, z];
                MazeWall walls = cell.walls;

                // Each cell only adds it's top right walls, unless it's on the bottom row, or left column
                if (x > 0) { walls &= ~MazeWall.Left; }
                if (z > 0) { walls &= ~MazeWall.Down; }

                this.AddCellWalls (cell.coords, walls);
            }
        }
    }

    /// <summary>
    /// Gets the container of the wall sections.
    /// </summary>
    /// <value>A Transform object.</value>
    public Transform container {
        get{
            return this.wallContainer ? this.wallContainer.transform : this.transform;
        }
    }

    private Vector3 GetCellPosition (MazeCell cell) {
        return this.GetCellPosition (cell.coords);
    }

    internal Vector3 GetCellPosition (Vector2Int cellCoords) {
        Vector3 position = new Vector3 (cellCoords.x, 0, cellCoords.y); // Start off with the cell coordinates
        position += new Vector3 (-(this.size.x / 2), 0, -(this.size.y / 2)); // Offset by half the dimensions of the grid
        position += new Vector3 (0.5f, 0, 0.5f); // Center of the cell
        if (this.width != 1) { position *= this.width; } // Scale for the corridor width
        position += this.container.position; // Offset by the position of the "maze" game object or wall container
        return position;
    }

    /// <summary>
    /// Adds the cell walls to a given MazeCell.
    /// </summary>
    /// <param name="cell">The MazeCell to add walls to.</param>
    private void AddCellWalls (MazeCell cell) {
        this.AddCellWalls (cell.coords, cell.walls);
    }

    /// <summary>
    /// Adds the cell walls at a given cell coordinates.
    /// </summary>
    /// <param name="coords">The cell coordinates.</param>
    /// <param name="walls">The list of walls to add.</param>
    private void AddCellWalls (Vector2Int coords, MazeWall walls) {
        Vector3 cellPosition = this.GetCellPosition (new Vector2Int (coords.x, coords.y));
        Transform container = this.container;

        // Create the wall sections
        if ((walls & MazeWall.Down) == MazeWall.Down) { // Bottom wall
            Vector3 wallPosition = cellPosition - new Vector3 (0, 0, this.width / 2f);
            Quaternion rotation = Quaternion.Euler (0, 90, 0);
            GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, container);
            wallSection.name = string.Format ("Cell {0}, wall {1}", coords, "down");
            this.wallSections.Add (wallSection);
        }
        if ((walls & MazeWall.Left) == MazeWall.Left) { // Left wall
            Vector3 wallPosition = cellPosition - new Vector3 (this.width / 2f, 0, 0);
            Quaternion rotation = Quaternion.Euler (0, 0, 0);
            GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, container);
            wallSection.name = string.Format ("Cell {0}, wall {1}", coords, "left");
            this.wallSections.Add (wallSection);
        }
        if ((walls & MazeWall.Right) == MazeWall.Right) { // Right wall
            Vector3 wallPosition = cellPosition + new Vector3 (this.width / 2f, 0, 0);
            Quaternion rotation = Quaternion.Euler (0, 0, 0);
            GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, container);
            wallSection.name = string.Format ("Cell {0}, wall {1}", coords, "right");
            this.wallSections.Add (wallSection);
        }
        if ((walls & MazeWall.Up) == MazeWall.Up) { // Top wall
            Vector3 wallPosition = cellPosition + new Vector3 (0, 0, this.width / 2f);
            Quaternion rotation = Quaternion.Euler (0, 90, 0);
            GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, container);
            wallSection.name = string.Format ("Cell {0}, wall {1}", coords, "up");
            this.wallSections.Add (wallSection);
        }
    }

    /// <summary>
    /// Remove all wall sections, ready to start a new game
    /// </summary>
    public void RemoveWalls () {
        foreach (GameObject wallSection in this.wallSections) {
            Destroy (wallSection);
        }
        this.wallSections.Clear ();
    }
}