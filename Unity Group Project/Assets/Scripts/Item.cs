using UnityEngine;
using UnityEngine.Tilemaps;

// The interface all items have to follow 
public interface IItem {
    public void Function(Player player);
}

// Represents an arbitrary item 
public abstract class Item : TileBase {
    public Color collectColor;
    public Sprite sprite;
    public string itemName;
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = sprite;
    }
}