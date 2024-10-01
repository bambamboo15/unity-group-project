using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class RestartBounds : MonoBehaviour {
    [Rename("Restart Bounds")] public bool restart_bounds;
    private Tilemap tm;

    void Start() {
        tm = GetComponent<Tilemap>();
    }

    void Update() {
        if (restart_bounds) {
            restart_bounds = false;
            tm.CompressBounds();
        }
    }
}
