using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SnakeBodyTile : TileBase {
    [Rename("Snake Body")] public Sprite snakeBody;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = snakeBody;
    }
}