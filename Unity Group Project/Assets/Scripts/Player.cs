using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

// The player is one of the core components of the whole game.
// What a "player" means should be self-explanatory.
public class Player : MonoBehaviour {
    // Object variables 
    [SerializeField] private GameObject snakes;
    [SerializeField] private Tilemap background;
    [SerializeField] private Tilemap walls;
    [SerializeField] private Tilemap exit;
    [SerializeField] private Tilemap items;
    [SerializeField] private Gold gold;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Grid grid;

    // How long the player takes to breathe after they move 
    public float delay;

    // How long the player takes to move from square to square 
    public float moveDelay;

    // The player health and stamina
    public int health = 100;
    public int stamina = 100;
    
    // Private variables 
    private bool moving;
    private float delayTimer;
    private float moveDelayTimer;
    private Vector3 dir;
    private Vector3 origPos;
    private Vector3 destPos;

    // Gold collected 
    public int goldCollected = 0;

    // Only runs on game startup 
    void Start() {
        moving = false;
        delayTimer = delay;
        dir = new Vector3(1.0f, 0.0f, 0.0f);
    }

    // Is this cell position blocked by a snake or wall tile?
    public bool isBlocked(Vector3Int pos) {
        for (int i = 0; i != snakes.transform.childCount; ++i)
            if (snakes.transform.GetChild(i).GetComponent<Tilemap>().HasTile(pos))
                return true;
        return walls.HasTile(pos);
    }

    // Ouch 
    private void Ouch() {
        SceneManager.LoadScene("Assets/Scenes/FailScreen.unity");
    }

    // YouWon
    private void YouWon() {
        SceneManager.LoadScene("Assets/Scenes/YouWonScreen.unity");
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

        //> If the player has no health left, commence fail sequence 
        if (health < 1) {
            Ouch();
            return;
        }

        //> If the player is not moving and a snake is on top of it,
        //> the snake attack 
        if (!moving) {
            //> If the player is on an exit square, instantly win 
            Vector3Int pos = grid.WorldToCell(transform.position);
            if (exit.HasTile(pos))
                YouWon();
            
            for (int i = 0; i != snakes.transform.childCount; ++i) {
                Snake snake = snakes.transform.GetChild(i).GetComponent<Snake>();
                Vector3Int snake_pos = snake.Head();

                //> Ouch 
                if (snake_pos == pos) {
                    health -= 55;
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
            Vector3Int nextTilePos = grid.WorldToCell(nextPos);
            
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

                Vector3Int gridPos = grid.WorldToCell(transform.position);

                //> Check if you are stepping on an item 
                if (items.HasTile(gridPos)) {
                    items.SetTile(gridPos, null);
                }

                //> Check if you are stepping on gold 
                if (gold.HasGold(gridPos)) {
                    //> Collect gold at that location 
                    gold.CollectAt(gridPos);
                    ++goldCollected;
                }

                //> Check if you are on a background tile, and if not, ouch 
                if (!background.HasTile(gridPos)) {
                    Ouch();
                    return;
                }
            }
        }
    }
}