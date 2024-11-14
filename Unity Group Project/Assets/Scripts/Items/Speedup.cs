using UnityEngine;
using UnityEngine.Tilemaps;

// Gives the player a speedup 
[CreateAssetMenu]
public class Speedup : TileBase {
    public Sprite sprite;
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = sprite;
    }
}