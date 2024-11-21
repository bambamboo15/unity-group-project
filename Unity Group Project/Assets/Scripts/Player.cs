using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private Volume volume;
    [SerializeField] private Items itemManager;

    // How long the player takes to breathe after they move 
    public float delay;

    // How long the player takes to move from square to square 
    public float moveDelay;

    // The absolute speed multiplier for the player 
    public float speedMultiplier = 1.0f;

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

    // Player inventory and current inventory item selected 
    public ItemTag[] inventory = new ItemTag[3] {
        new ItemTag(), new ItemTag(), new ItemTag()
    };
    public int inventorySelectedIndex = 0;

    // Various postprocessor settings 
    private Bloom postProcessorBloom;

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

    // When you collect an item, what happens?
    //  (do not deal with deleting item)
    private void CollectItem(Item item) {
        // If the player has an unused inventory slot, put the 
        // collected item tag there 
        int i = 0;
        for (; i != 3; ++i) {
            if (inventory[i].empty) {
                inventory[i] = new ItemTag(item);
                break;
            }
        }

        // If the player has no unused inventory slots, replace 
        // the currently selected item tag with the collected 
        // item tag.
        //
        // [TODO] Place the replaced item tag back onto the map 
        if (i == 3)
            inventory[inventorySelectedIndex] = new ItemTag(item);
    }

    // Use the current item selected, if any 
    private void UseItemSelected() {
        ItemTag tag = inventory[inventorySelectedIndex];
        if (!tag.empty) {
            inventory[inventorySelectedIndex] = new ItemTag();
            UseItem(tag);
        }
    }

    // What happens when you use an item?
    private void UseItem(ItemTag tag) {
        tag.itemInterface.Function();
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
        
        bool E = Input.GetKey("e");
        bool SPACE = Input.GetKeyDown("space");

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

        //> If the player is not moving, but is holding E, and the currently 
        //> selected item slot is nonempty, then use it 
        if (!moving && E)
            UseItemSelected();

        //> If the player presses space, move to the next inventory slot 
        //> and cycle if necessary 
        if (SPACE)
            inventorySelectedIndex =
                (inventorySelectedIndex + 1) % 3;

        //> If the updated delay timer is out, the player is not moving,
        //> and the player pressed a key after the delay timer went off,
        //> then enter the move sequence 
        delayTimer -= Time.deltaTime * speedMultiplier;
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
            moveDelayTimer -= Time.deltaTime * speedMultiplier;
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
                TileBase itemTile = items.GetTile(gridPos);
                if (itemTile != null && itemTile is Item) {
                    CollectItem(itemTile as Item);
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