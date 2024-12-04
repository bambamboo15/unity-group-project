using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class InvisibleWallTile : TileBase {
    public Sprite editorSprite;
    public Sprite runtimeSprite;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
#if UNITY_EDITOR 
        if (Application.isPlaying) {
            tileData.sprite = runtimeSprite;
        } else {
            tileData.sprite = editorSprite;
        }
#else 
        tileData.sprite = runtimeSprite;
#endif 
    }
}