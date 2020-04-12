using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
internal enum MazeWall {
    Up = 0x1,
    Down = 0x2,
    Right = 0x4,
    Left = 0x8,
    All = 0xF,
    None = 0x0,
}

internal class MazeCell {
    public Vector2Int coords = new Vector2Int ();
    public MazeWall walls = MazeWall.All;
    public bool visited = false;
}

internal static class MazeGenerator {

    internal static MazeCell[, ] GenerateMaze (Vector2Int size, Vector2Int start, Vector2Int finish) {
        return GenerateMaze (0, size, start, finish);
    }

    internal static MazeCell[, ] GenerateMaze (int randomSeed, Vector2Int size, Vector2Int start, Vector2Int finish) {
        System.Random rnd = randomSeed != 0 ? new System.Random (randomSeed) : new System.Random ();

        // Create the empty maze grid
        MazeCell[, ] cells = new MazeCell[size.x, size.y];
        for (int x = 0; x < size.x; x++) {
            for (int z = 0; z < size.y; z++) {
                MazeCell cell = new MazeCell ();
                cell.coords = new Vector2Int (x, z);
                cell.walls = MazeWall.All;
                cell.visited = false;
                cells[x, z] = cell;
            }
        }

        // Generate the maze!
        // Start at the finish (!) and randomly move, working backwards to the start
        // When we hit a cell that has no unvisited neighbours, backtrack until we find one that does
        // https://en.wikipedia.org/wiki/Maze_generation_algorithm#Depth-first_search

        // Create a stack for the path
        Stack<Vector2Int> path = new Stack<Vector2Int> ();
        path.Push (finish);
        int attempts = size.x * size.y * 10; // Allow 10x the number of cells 
        while (path.Count > 0 && attempts++ > 0) {
            // Current coords/cell
            Vector2Int currentCoords = path.Peek ();
            MazeCell currentCell = cells[currentCoords.x, currentCoords.y];
            MazeWall currentWall = MazeWall.None;

            // Mark this cell as visited
            currentCell.visited = true;

            // If no unvisited neighbours, remove from stack, and try the previous one again
            int remainingNeighbours = 4;
            if (currentCoords.y >= size.y - 1 || cells[currentCoords.x, currentCoords.y + 1].visited) { remainingNeighbours--; }
            if (currentCoords.y <= 0 || cells[currentCoords.x, currentCoords.y - 1].visited) { remainingNeighbours--; }
            if (currentCoords.x >= size.x - 1 || cells[currentCoords.x + 1, currentCoords.y].visited) { remainingNeighbours--; }
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
                    if (currentCoords.y < size.y - 1) {
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
                    if (currentCoords.x < size.x - 1) {
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

        return cells;
    }
}