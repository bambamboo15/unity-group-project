using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SnakeHeadTile : TileBase {
    public Sprite snakeHead;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = snakeHead;
    }
}