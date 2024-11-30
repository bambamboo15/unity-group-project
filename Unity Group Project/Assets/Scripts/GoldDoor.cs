using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

// When you collect enough gold, this door opens 
public class GoldDoor : MonoBehaviour {
    // Gold required to open the door 
    public int goldRequired;

    // Player for gold collected and text color 
    [SerializeField] private Player player;

    // Grid layout and tilemap for text positioning (partially lazy)
    [SerializeField] private GridLayout gridLayout;
    public Tilemap tilemap;

    // Private variables 
    private TMP_Text text;
    private bool opened;

    void Start() {
        text = transform.GetChild(0).GetComponent<TMP_Text>();
        text.overrideColorTags = true;
        tilemap = GetComponent<Tilemap>();
        opened = false;

        Vector3Int total = Vector3Int.zero;
        int count = 0;

        for (int x = tilemap.origin.x; x != tilemap.origin.x + tilemap.size.x; ++x) {
            for (int y = tilemap.origin.y; y != tilemap.origin.y + tilemap.size.y; ++y) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(pos);
                
                if (tile is not null) {
                    total += pos;
                    ++count;
                }
            }
        }

        Vector3 average = gridLayout.CellToWorld(total) / count;
        average += gridLayout.cellSize / 2.0f;
        average.z = text.transform.position.z;
        text.transform.position = average;
    }

    public int GoldMore() {
        return goldRequired - player.goldCollected;
    }

    public bool Opened() {
        return opened;
    }

    void Update() {
        if (!opened) {
            text.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - Mathf.Pow(Vector3.Distance(player.transform.position, text.transform.position) / 8.0f, 2.0f));
            text.text = GoldMore() + " more";

            if (GoldMore() <= 0) {
                text.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                tilemap.ClearAllTiles();
                opened = true;
            }
        }
    }
}