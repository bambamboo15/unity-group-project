using UnityEngine;
using UnityEngine.Tilemaps;

// The player collects gold. Collect enough and 
// you win. However, the game progressively gets harder 
// and harder when you collect more gold, at least for 
// classic mode.
public class Gold : MonoBehaviour {
    // The Tilemap component 
    private Tilemap tilemap;

    // Total number of gold 
    public int goldAmount;

    void Start() {
        tilemap = GetComponent<Tilemap>();
        ComputeGoldAmount();
    }

    // Recompute gold amount, an expensive operation!
    public void ComputeGoldAmount() {
        for (int x = tilemap.origin.x; x != tilemap.origin.x + tilemap.size.x; ++x) {
            for (int y = tilemap.origin.y; y != tilemap.origin.y + tilemap.size.y; ++y) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                goldAmount += tilemap.HasTile(pos) ? 1 : 0;
            }
        }
    }
}