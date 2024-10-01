using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour {
    // Public object variables 
    public Tilemap background;
    public Tilemap walls;
    public Grid grid;

    // How long the player takes to breathe after they move 
    public float delay;

    // How long the player takes to move from square to square 
    [Rename("Move Delay")] public float move_delay;

    // Private variables 
    private bool moving;
    private float delay_timer;
    private float move_delay_timer;
    private Vector3 dir;
    private Vector3 orig_pos;
    private Vector3 dest_pos;
    private GridLayout grid_layout;

    // Only runs on game startup 
    void Start() {
        moving = false;
        delay_timer = delay;
        dir = new Vector3(1.0f, 0.0f, 0.0f);
        grid_layout = grid.GetComponent<GridLayout>();
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

        //> If the updated delay timer is out, the player is not moving,
        //> and the player pressed a key after the delay timer went off,
        //> then enter the move sequence 
        delay_timer -= Time.deltaTime;
        if (!moving && delay_timer < 0.0f && ANY) {
            //> Now, we first need to check if there is a wall tile to where 
            //> we are going.
            Vector3 next_pos = transform.position +
                Vector3.Scale(dir, new Vector3(1.0f, 1.0f, 0.0f) + grid.cellGap);
            Vector3Int next_tile_pos = grid_layout.WorldToCell(next_pos);
            
            if (!walls.HasTile(next_tile_pos)) {
                move_delay_timer = move_delay;
                moving = true;
                orig_pos = transform.position;
                dest_pos = orig_pos + Vector3.Scale(dir, new Vector3(1.0f, 1.0f, 0.0f) + grid.cellGap);
            }
        }

        //> If the player is moving, let the move delay timer count down,
        //> and lerp the player position 
        if (moving) {
            move_delay_timer -= Time.deltaTime;
            float dt = 1.0f - (move_delay_timer / move_delay);
            transform.position = Vector3.Lerp(orig_pos, dest_pos, dt);

            //> If the move sequence is over, snap the player to the 
            //> destination position, stop moving, and start delaying 
            if (move_delay_timer < 0.0f) {
                transform.position = dest_pos;
                moving = false;
                delay_timer = delay;
            }
        }
    }
}