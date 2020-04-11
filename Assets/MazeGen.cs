using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGen : MonoBehaviour {
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

    [Tooltip ("Object for the start marker")]
    public GameObject startObject = null;
    [Tooltip ("Object for the finish marker")]
    public GameObject finishObject = null;

    [Flags]
    private enum MazeWall {
        Up = 0x1,
        Down = 0x2,
        Right = 0x4,
        Left = 0x8,
        All = 0xF,
        None = 0x0,
    }
    private class MazeCell {
        public Vector2Int coords = new Vector2Int ();
        public MazeWall walls = MazeWall.All;
        public bool visited = false;
    }

    void Start () {
        System.Random rnd = this.randomSeed != 0 ? new System.Random (this.randomSeed) : new System.Random ();

        // Create the empty maze grid
        MazeCell[, ] cells = new MazeCell[this.size.x, this.size.y];
        for (int x = 0; x < this.size.x; x++) {
            for (int z = 0; z < this.size.y; z++) {
                MazeCell cell = new MazeCell ();
                cell.coords = new Vector2Int (x, z);
                cell.walls = MazeWall.All;
                cell.visited = false;
                cells[x, z] = cell;
            }
        }

        // Move the start and finish markers
        this.startObject.transform.position = this.GetCellPosition (this.start);
        this.finishObject.transform.position = this.GetCellPosition (this.finish);

        // Generate the map!
        // Start at the finish (!) and randomly move, working backwards to the start
        // When we hit a cell that has no unvisited neighbours, backtrack until we find one that does
        // https://en.wikipedia.org/wiki/Maze_generation_algorithm#Depth-first_search

        // Create a stack for the path
        Stack<Vector2Int> path = new Stack<Vector2Int> ();
        path.Push (this.finish);
        int attempts = this.size.x * this.size.y * 10; // Allow 10x the number of cells 
        while (path.Count > 0 && attempts++ > 0) {
            // Current coords/cell
            Vector2Int currentCoords = path.Peek ();
            MazeCell currentCell = cells[currentCoords.x, currentCoords.y];
            MazeWall currentWall = MazeWall.None;

            // Mark this cell as visited
            currentCell.visited = true;

            // If no unvisited neighbours, remove from stack, and try the previous one again
            int remainingNeighbours = 4;
            if (currentCoords.y >= this.size.y - 1 || cells[currentCoords.x, currentCoords.y + 1].visited) { remainingNeighbours--; }
            if (currentCoords.y <= 0 || cells[currentCoords.x, currentCoords.y - 1].visited) { remainingNeighbours--; }
            if (currentCoords.x >= this.size.x - 1 || cells[currentCoords.x + 1, currentCoords.y].visited) { remainingNeighbours--; }
            if (currentCoords.x <= 0 || cells[currentCoords.x - 1, currentCoords.y].visited) { remainingNeighbours--; }
            if (remainingNeighbours == 0) {
                // Backtrack
                path.Pop ();
                continue;
            }

            // Randomly pick a neighbour
            Vector2Int neighbourCoords = new Vector2Int ();
            MazeCell neighbourCell = null;
            MazeWall neighbourWall = MazeWall.None;
            switch ((int) rnd.Next (0, 4)) {
                case 0: // Up
                    if (currentCoords.y < this.size.y - 1) {
                        neighbourCoords = new Vector2Int (currentCoords.x, currentCoords.y + 1);
                        neighbourCell = cells[neighbourCoords.x, neighbourCoords.y];
                    }
                    currentWall = MazeWall.Up;
                    neighbourWall = MazeWall.Down;
                    break;
                case 1: // Down
                    if (currentCoords.y > 0) {
                        neighbourCoords = new Vector2Int (currentCoords.x, currentCoords.y - 1);
                        neighbourCell = cells[neighbourCoords.x, neighbourCoords.y];
                    }
                    currentWall = MazeWall.Down;
                    neighbourWall = MazeWall.Up;
                    break;
                case 2: // Right
                    if (currentCoords.x < this.size.x - 1) {
                        neighbourCoords = new Vector2Int (currentCoords.x + 1, currentCoords.y);
                        neighbourCell = cells[neighbourCoords.x, neighbourCoords.y];
                    }
                    currentWall = MazeWall.Right;
                    neighbourWall = MazeWall.Left;
                    break;
                case 3: // Left
                    if (currentCoords.x > 0) {
                        neighbourCoords = new Vector2Int (currentCoords.x - 1, currentCoords.y);
                        neighbourCell = cells[neighbourCoords.x, neighbourCoords.y];
                    }
                    currentWall = MazeWall.Left;
                    neighbourWall = MazeWall.Right;
                    break;
            }

            // If there is no neighbour, or it's already visited, try again
            if (neighbourCell == null || neighbourCell.visited) { continue; }

            // Remove the adjoining walls
            currentCell.walls &= ~currentWall;
            neighbourCell.walls &= ~neighbourWall;

            // Add neighbour to the stack
            path.Push (neighbourCoords);
        }

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

    private Vector3 GetCellPosition (MazeCell cell) {
        return this.GetCellPosition (cell.coords);
    }

    internal Vector3 GetCellPosition (Vector2Int cellCoords) {
        Vector3 position = new Vector3 (cellCoords.x, 0, cellCoords.y); // Start off with the cell coordinates
        position += new Vector3 (-(this.size.x / 2), 0, -(this.size.y / 2)); // Offset by half the dimensions of the grid
        position += new Vector3 (0.5f, 0, 0.5f); // Center of the cell
        if (this.width != 1) { position *= this.width; } // Scale for the corridor width
        position += this.transform.position; // Offset by the position of the "maze" game object
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
    /// Adds the cell walls at a civen cell coordinates.
    /// </summary>
    /// <param name="coords">The cell coordinates.</param>
    /// <param name="walls">The list of walls to add.</param>
    private void AddCellWalls (Vector2Int coords, MazeWall walls) {
        Vector3 cellPosition = this.GetCellPosition (new Vector2Int (coords.x, coords.y));

        // Create the wall sections
        if ((walls & MazeWall.Down) == MazeWall.Down) { // Bottom wall
            Vector3 wallPosition = cellPosition - new Vector3 (0, 0, this.width / 2f);
            Quaternion rotation = Quaternion.Euler (0, 90, 0);
            GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, this.transform);
            wallSection.name = string.Format ("Cell {0}, wall {1}", coords, "down");
        }
        if ((walls & MazeWall.Left) == MazeWall.Left) { // Left wall
            Vector3 wallPosition = cellPosition - new Vector3 (this.width / 2f, 0, 0);
            Quaternion rotation = Quaternion.Euler (0, 0, 0);
            GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, this.transform);
            wallSection.name = string.Format ("Cell {0}, wall {1}", coords, "left");
        }
        if ((walls & MazeWall.Right) == MazeWall.Right) { // Right wall
            Vector3 wallPosition = cellPosition + new Vector3 (this.width / 2f, 0, 0);
            Quaternion rotation = Quaternion.Euler (0, 0, 0);
            GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, this.transform);
            wallSection.name = string.Format ("Cell {0}, wall {1}", coords, "right");
        }
        if ((walls & MazeWall.Up) == MazeWall.Up) { // Top wall
            Vector3 wallPosition = cellPosition + new Vector3 (0, 0, this.width / 2f);
            Quaternion rotation = Quaternion.Euler (0, 90, 0);
            GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, this.transform);
            wallSection.name = string.Format ("Cell {0}, wall {1}", coords, "up");
        }
    }
}