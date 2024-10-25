using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class RestartBounds : MonoBehaviour {
    public bool restartBounds;
    private Tilemap tm;

    void Start() {
        tm = GetComponent<Tilemap>();
    }

    void Update() {
        if (restartBounds) {
            restartBounds = false;
            tm.CompressBounds();
        }
    }
}