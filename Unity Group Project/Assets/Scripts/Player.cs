using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    // Public object variables 
    public GameObject snakes;
    public Tilemap walls;
    public Grid grid;

    // How long the player takes to breathe after they move 
    public float delay;

    // How long the player takes to move from square to square 
    public float moveDelay;

    // All action tiles 
    public List<Action> actionTiles = new List<Action>();

    // Private variables 
    private bool moving;
    private float delayTimer;
    private float moveDelayTimer;
    private Vector3 dir;
    private Vector3 origPos;
    private Vector3 destPos;
    private GridLayout gridLayout;

    // Gold collected 
    public int gold;

    // Only runs on game startup 
    void Start() {
        gold = 0;
        moving = false;
        delayTimer = delay;
        dir = new Vector3(1.0f, 0.0f, 0.0f);
        gridLayout = grid.GetComponent<GridLayout>();
    }

    // Is this cell position blocked by a snake or wall tile?
    public bool isBlocked(Vector3Int pos) {
        for (int i = 0; i != snakes.transform.childCount; ++i)
            if (snakes.transform.GetChild(i).GetComponent<Tilemap>().HasTile(pos))
                return true;
        return walls.HasTile(pos);
    }

    // The player has been caught.
    private void Caught() {
        SceneManager.LoadScene("Assets/Scenes/FailScreen.unity");
    }

    // Runs every frame 
    void Update() {
        //> Player input key mapping 
        bool W = Input.GetKey("w");
        bool A = Input.GetKey("a");
        bool S = Input.GetKey("s");
        bool D = Input.GetKey("d");
        bool ANY = W || A || S || D;

        //> Get player input and adjust direction based on that, if the 
        //> player is not moving 
        if (!moving) {
            if (W) {
                dir = new Vector3(0.0f, 1.0f, 0.0f);
            } else if (A) {
                dir = new Vector3(-1.0f, 0.0f, 0.0f);
            } else if (S) {
                dir = new Vector3(0.0f, -1.0f, 0.0f);
            } else if (D) {
                dir = new Vector3(1.0f, 0.0f, 0.0f);
            }
        }

        //> If the player is not moving and a snake is on top of it,
        //> commence fail sequence 
        if (!moving) {
            Vector3Int pos = gridLayout.WorldToCell(transform.position);
            for (int i = 0; i != snakes.transform.childCount; ++i) {
                Snake snake = snakes.transform.GetChild(i).GetComponent<Snake>();
                Vector3Int snake_pos = snake.Head();

                //> Ouch!
                if (snake_pos == pos) {
                    Caught();
                    return;
                }
            }
        }

        //> If the updated delay timer is out, the player is not moving,
        //> and the player pressed a key after the delay timer went off,
        //> then enter the move sequence 
        delayTimer -= Time.deltaTime;
        if (!moving && delayTimer < 0.0f && ANY) {
            //> Get the tile position of where we are going to 
            Vector3 nextPos = transform.position +
                Vector3.Scale(dir, new Vector3(1.0f, 1.0f, 0.0f) + grid.cellGap);
            Vector3Int nextTilePos = gridLayout.WorldToCell(nextPos);
            
            //> If we are not blocked, then do move 
            if (!isBlocked(nextTilePos)) {
                moveDelayTimer = moveDelay;
                moving = true;
                origPos = transform.position;
                destPos = origPos + Vector3.Scale(dir, new Vector3(1.0f, 1.0f, 0.0f) + grid.cellGap);
            }
        }

        //> If the player is moving, let the move delay timer count down,
        //> and lerp the player position 
        if (moving) {
            moveDelayTimer -= Time.deltaTime;
            float dt = 1.0f - (moveDelayTimer / moveDelay);
            transform.position = Vector3.Lerp(origPos, destPos, dt);

            //> If the move sequence is over, snap the player to the 
            //> destination position, stop moving, and start delaying 
            if (moveDelayTimer < 0.0f) {
                transform.position = destPos;
                moving = false;
                delayTimer = delay;

                //> For all action tiles, call their Triggered methods 
                for (int i = 0; i != actionTiles.Count; ++i) {
                    Action tile = actionTiles[i];
                    if (gridLayout.WorldToCell(tile.transform.position) == gridLayout.WorldToCell(transform.position)) {
                        tile.Triggered();
                    }
                }
            }
        }
    }
}