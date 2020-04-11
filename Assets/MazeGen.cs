using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGen : MonoBehaviour {
    public GameObject wallObject = null;
    public Vector2Int size = new Vector2Int (10, 10);
    public int width = 1;

    public GameObject startObject = null;
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
        System.Random rnd = new System.Random ();

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

        // Start and finish are opposite sides, 3 up/down from the corner
        Vector2Int start = new Vector2Int (0, 2);
        Vector2Int finish = new Vector2Int (this.size.x - 1, this.size.y - 3);

        // Move the start and finish markers
        this.startObject.transform.position = this.GetCellPosition (start);
        this.finishObject.transform.position = this.GetCellPosition (finish);

        // Generate the map!
        // Start at the finish (!) and randomly move, working backwards to the start
        // When we hit a cell that has no unvisited neighbours, backtrack until we find one that does
        // https://en.wikipedia.org/wiki/Maze_generation_algorithm#Depth-first_search

        // Create a stack for the path
        Stack<Vector2Int> path = new Stack<Vector2Int> ();
        path.Push (finish);
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
        // Each cell only adds it's top right walls, unless it's on the bottom row, or left column
        for (int x = 0; x < this.size.x; x++) {
            for (int z = 0; z < this.size.y; z++) {
                MazeWall walls = cells[x, z].walls;
                Vector3 position = this.GetCellPosition (new Vector2Int (x, z));

                // Create the wall sections
                if ((walls & MazeWall.Up) == MazeWall.Up) { // Top wall
                    Vector3 wallPosition = position + new Vector3 (0, 0, this.width / 2f);
                    Quaternion rotation = Quaternion.Euler (0, 90, 0);
                    GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, this.transform);
                }
                if ((walls & MazeWall.Down) == MazeWall.Down && z == 0) { // Bottom wall (only when on the bottom row)
                    Vector3 wallPosition = position - new Vector3 (0, 0, this.width / 2f);
                    Quaternion rotation = Quaternion.Euler (0, 90, 0);
                    GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, this.transform);
                }
                if ((walls & MazeWall.Right) == MazeWall.Right) { // Right wall
                    Vector3 wallPosition = position + new Vector3 (this.width / 2f, 0, 0);
                    Quaternion rotation = Quaternion.Euler (0, 0, 0);
                    GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, this.transform);
                }
                if ((walls & MazeWall.Left) == MazeWall.Left && x == 0) { // Left wall (only when on the left column)
                    Vector3 wallPosition = position - new Vector3 (this.width / 2f, 0, 0);
                    Quaternion rotation = Quaternion.Euler (0, 0, 0);
                    GameObject wallSection = Instantiate (this.wallObject, wallPosition, rotation, this.transform);
                }
            }
        }
    }

    private Vector3 GetCellPosition (MazeCell cell) {
        return this.GetCellPosition (cell.coords);
    }

    private Vector3 GetCellPosition (Vector2Int cellCoords) {
        Vector3 position = new Vector3 (cellCoords.x, 0, cellCoords.y); // Start off with the cell coordinates
        position += new Vector3 (-(this.size.x / 2), 0, -(this.size.y / 2)); // Offset by half the dimensions of the grid
        position += new Vector3 (0.5f, 0, 0.5f); // Center of the cell
        if (this.width != 1) { position *= this.width; } // Scale for the corridor width
        position += this.transform.position; // Offset by the position of the "maze" game object
        return position;
    }
}