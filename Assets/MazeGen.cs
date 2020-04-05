using System;
using UnityEngine;

public class MazeGen : MonoBehaviour {
    public GameObject wallObject = null;
    public int size = 10;

    // Start is called before the first frame update
    void Start () {
        // Adjust the bounds to start 1 in from the edge
        int bounds = (this.size / 2) - 1;
        System.Random rnd = new System.Random ();

        // Loop over each point
        for (int x = -bounds; x <= bounds; x++) {
            for (int z = -bounds; z <= bounds; z++) {

                // Generate the position and rotation      
                int direction = (int) rnd.Next (0, 3);
                Vector3 position = new Vector3 (x, 0, z);
                Quaternion rotation = new Quaternion (0, 0, 0, 0);

                // Negative to bottom left
                switch (direction) {
                    case 0: // Up
                        position = new Vector3 ((float) (x + 0.5), 0, (float) z);
                        rotation = Quaternion.Euler (0, 90, 0);
                        break;
                    case 1: // Down
                        position = new Vector3 ((float) (x - 0.5), 0, (float) z);
                        rotation = Quaternion.Euler (0, 90, 0);
                        break;
                    case 2: // Right
                        position = new Vector3 ((float) x, 0, (float) (z + 0.5));
                        rotation = Quaternion.Euler (0, 0, 0);
                        break;
                    case 3: // Left
                        position = new Vector3 ((float) x, 0, (float) (z - 0.5));
                        rotation = Quaternion.Euler (0, 0, 0);
                        break;
                }

                // Create the wall section
                GameObject section = Instantiate (this.wallObject, position, rotation, this.transform);
            }
        }
    }
}