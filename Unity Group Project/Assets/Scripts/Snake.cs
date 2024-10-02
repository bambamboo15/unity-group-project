using UnityEngine;
using UnityEngine.Tilemaps;

public class Snake : MonoBehaviour {
    // The player the snake should be directed to. It can virtually 
    // be any GameObject because only the position is tracked.
    public GameObject player;

    // The grid for layout purposes.
    public Grid grid;

    // Private variables 
    private Tilemap tilemap;

    // Initialize the internal data structure based on prepared tiles 
    void Start() {
        tilemap = GetComponent<Tilemap>();

        int count = 0;

        TileBase[] tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        for (int i = 0; i != tiles.Length; ++i) {
            TileBase tile = tiles[i];
            ++count;
        }

        Debug.Log(count);
    }
}