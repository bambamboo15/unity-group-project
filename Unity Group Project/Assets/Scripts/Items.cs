using UnityEngine;
using UnityEngine.Tilemaps;

// Item tag 
public class ItemTag {
    public Sprite sprite;
    public string name;
    public bool empty;
    public IItem itemInterface;

    public ItemTag() {
        empty = true;
    }

    public ItemTag(Item item) {
        sprite = item.sprite;
        name = item.itemName;
        empty = false;
        itemInterface = item as IItem;
    }
};

// Keeps track of all items and manages their initialization 
public class Items : MonoBehaviour {
    // The tilemap 
    private Tilemap tilemap;

    // Item assets 
    [SerializeField] private Item speedup;
    [SerializeField] private Item cookie;

    // Item probabilities 
    [SerializeField] private float speedupProb;
    [SerializeField] private float cookieProb;

    // Initialize all items 
    void Start() {
        tilemap = GetComponent<Tilemap>();

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

    void Update() {
        transform.position = new Vector3(0.0f, Mathf.Sin(Time.time * 4.0f) * 0.075f * tilemap.layoutGrid.cellSize.y, 0.0f);
    }
}