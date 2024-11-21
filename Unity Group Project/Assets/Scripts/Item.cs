using UnityEngine;
using UnityEngine.Tilemaps;

// The interface all items have to follow 
public interface IItem {
    public void Function();
}

// Represents an arbitrary item 
public abstract class Item : TileBase {
    public Sprite sprite;
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = sprite;
    }
}