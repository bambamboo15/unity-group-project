using UnityEngine;
using UnityEngine.Tilemaps;

// Item tag 
public class ItemTag {
    public Sprite sprite;
    public bool empty;
    public IItem itemInterface;

    public ItemTag() {
        empty = true;
    }

    public ItemTag(Item item) {
        sprite = item.sprite;
        empty = false;
        itemInterface = item as IItem;
    }
};

// Keeps track of all items and manages their initialization 
public class Items : MonoBehaviour {
    // The tilemap 
    [SerializeField] private Tilemap tilemap;

    // Item assets 
    [SerializeField] private Item speedup;
    [SerializeField] private Item cookie;

    // Item probabilities 
    [SerializeField] private float speedupProb;
    [SerializeField] private float cookieProb;

    // Initialize all items 
    void Start() {
        for (int x = tilemap.origin.x; x != tilemap.origin.x + tilemap.size.x; ++x) {
            for (int y = tilemap.origin.y; y != tilemap.origin.y + tilemap.size.y; ++y) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(pos);
                
                if (tile is RandomItem) {
                    float number = Random.value;
                    float accumulator = 0.0f;
                    
                    if (number < (accumulator += speedupProb)) {
                        tilemap.SetTile(pos, speedup);
                    } else if (number < (accumulator += cookieProb)) {
                        tilemap.SetTile(pos, cookie);
                    } else {
                        tilemap.SetTile(pos, null);
                    }
                }
            }
        }
    }
}