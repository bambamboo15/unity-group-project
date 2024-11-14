using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SnakeBodyTile : TileBase {
    public Sprite snakeBody;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = snakeBody;
    }
}