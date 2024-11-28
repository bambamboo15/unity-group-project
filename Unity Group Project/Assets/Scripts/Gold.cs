using UnityEngine;
using UnityEngine.Tilemaps;

// The player collects gold. Collect enough and 
// you win. However, the game progressively gets harder 
// and harder when you collect more gold, at least for 
// classic mode.
public class Gold : MonoBehaviour {
    // Gold particle effect prefab 
    [SerializeField] private ParticleSystem collectionEffect;

    // Grid 
    [SerializeField] private Grid grid;

    // Snake folder 
    [SerializeField] private Transform snakes;

    // The Tilemap component 
    [SerializeField] private Tilemap tilemap;

    // Sound effect player 
    [SerializeField] private SFXPlayer sfxPlayer;

    // The audio clip for gold collection 
    [SerializeField] private AudioClip goldAudio;

    // Total number of gold 
    public int goldAmount;

    void Start() {
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

    // Does gold tile exist there?
    public bool HasGold(Vector3Int pos) {
        return tilemap.HasTile(pos);
    }

    // Collect gold at location given that there is a tile there.
    public void CollectAt(Vector3Int pos) {
        tilemap.SetTile(pos, null);
        Vector3 worldPos = grid.CellToWorld(pos) + new Vector3(grid.cellSize.x * 0.5f, grid.cellSize.y * 0.5f, 0.0f);
        Instantiate(collectionEffect, worldPos, Quaternion.identity);

        sfxPlayer.Play(goldAudio);

        for (int i = 0; i != snakes.childCount; ++i) {
            Snake snake = snakes.GetChild(i).GetComponent<Snake>();
            snake.ApplyGoldCollection();
        }
    }
}