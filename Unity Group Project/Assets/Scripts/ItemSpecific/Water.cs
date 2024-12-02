using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

// Water helper 
public class Water : MonoBehaviour {
    [SerializeField] private TileBase waterPrefab;
    [SerializeField] private float delay;
    [SerializeField] private int steps;
    [HideInInspector] public Tilemap walls;
    [HideInInspector] public Vector3Int pos;
    public HashSet<Vector3Int> cameFrom = new HashSet<Vector3Int>();
    private Tilemap tilemap;
    private float timer = 0.0f;
    private int m = 0;

    void Start() {
        tilemap = GetComponent<Tilemap>();
        cameFrom.Add(pos);
        AllNeighbors(pos, neighbor =>
            tilemap.SetTile(neighbor, waterPrefab));
    }

    public void AllNeighbors(Vector3Int node, Action<Vector3Int> callback) {
        if (walls.GetTile(node + Vector3Int.left) is null)
            callback(node + Vector3Int.left);
        if (walls.GetTile(node + Vector3Int.right) is null)
            callback(node + Vector3Int.right);
        if (walls.GetTile(node + Vector3Int.up) is null)
            callback(node + Vector3Int.up);
        if (walls.GetTile(node + Vector3Int.down) is null)
            callback(node + Vector3Int.down);
        if (walls.GetTile(node + Vector3Int.left + Vector3Int.up) is null)
            callback(node + Vector3Int.left + Vector3Int.up);
        if (walls.GetTile(node + Vector3Int.right + Vector3Int.up) is null)
            callback(node + Vector3Int.right + Vector3Int.up);
        if (walls.GetTile(node + Vector3Int.left + Vector3Int.down) is null)
            callback(node + Vector3Int.left + Vector3Int.down);
        if (walls.GetTile(node + Vector3Int.right + Vector3Int.down) is null)
            callback(node + Vector3Int.right + Vector3Int.down);
    }

    void Update() {
        timer += Time.deltaTime;
        if (timer > delay) {
            if (m++ == steps) {
                Destroy(gameObject);
                return;
            }
            HashSet<Vector3Int> current = new HashSet<Vector3Int>();
            HashSet<Vector3Int> future = new HashSet<Vector3Int>();
            for (int x = tilemap.origin.x; x != tilemap.origin.x + tilemap.size.x; ++x) {
                for (int y = tilemap.origin.y; y != tilemap.origin.y + tilemap.size.y; ++y) {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    TileBase tile = tilemap.GetTile(position);
                    if (tile is not null)
                        current.Add(position);
                }
            }
            foreach (Vector3Int position in current)
                AllNeighbors(position, neighbor => {
                    if (!cameFrom.Contains(neighbor) && !current.Contains(neighbor))
                        future.Add(neighbor);
                });
            tilemap.ClearAllTiles();
            foreach (Vector3Int position in future)
                tilemap.SetTile(position, waterPrefab);
            cameFrom = current;

            timer = 0.0f;
        }
    }
}