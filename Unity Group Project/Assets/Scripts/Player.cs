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
    [SerializeField] private GameObject snakes;
    [SerializeField] private Tilemap background;
    [SerializeField] private Tilemap walls;
    [SerializeField] private Tilemap exit;
    [SerializeField] private Tilemap items;
    [SerializeField] private Transform goldDoorFolder;
    [SerializeField] private Gold gold;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Grid grid;
    [SerializeField] private Volume volume;
    [SerializeField] private Items itemManager;
    [SerializeField] private SFXPlayer sfxPlayer;
    [SerializeField] private AudioClip healthLossAudio;
    [SerializeField] private AudioClip itemCollectionAudio;
    [SerializeField] private AudioClip inventoryToggleAudio;
    [SerializeField] private ParticleSystem collectionEffect;

    // How long the player takes to breathe after they move 
    public float delay;

    // How long the player takes to move from square to square 
    public float moveDelay;

    // The player health and stamina 
    public float health = 100.0f;
    public float stamina = 100.0f;

    // Stamina per second when regaining or sprinting 
    // and stamina speed multiplier 
    public float staminaRateUp;
    public float staminaRateDown;
    public float staminaSpeedMultiplier;

    // Settings for postprocessor bloom effect 
    public float bloomWalking;
    public float bloomSprinting;
    public float bloomAdjustRate;

    // Health-related configurations 
    public float playerHealthLossDelay;
    public float snakeBiteDamage;
    public float nonBackgroundDamage;

    // Camera smoothing 
    public float playerCameraAdjustRate;
    
    // Private variables 
    private float playerHealthLossTimer;
    private bool healthLossFrame;
    private bool moving;
    private float delayTimer;
    private float moveDelayTimer;
    private Vector3 dir;
    private Vector3 origPos;
    private Vector3 destPos;
    private bool sprinting;
    private GoldDoor[] goldDoors;

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
        playerHealthLossTimer = playerHealthLossDelay;

        goldDoors = new GoldDoor[goldDoorFolder.childCount];
        for (int i = 0; i != goldDoors.Length; ++i) {
            goldDoors[i] = goldDoorFolder.GetChild(i).GetComponent<GoldDoor>();
        }
    }

    // Is this cell position blocked? A cell position can be blocked by:
    //    - a snake tile 
    //    - a wall tile 
    //    - an unopened golden door 
    public bool isBlocked(Vector3Int pos) {
        for (int i = 0; i != snakes.transform.childCount; ++i)
            if (snakes.transform.GetChild(i).GetComponent<Tilemap>().HasTile(pos))
                return true;
        for (int i = 0; i != goldDoors.Length; ++i) {
            GoldDoor goldDoor = goldDoors[i];
            if (!goldDoor.Opened() && goldDoor.tilemap.HasTile(pos))
                return true;
        }
        return walls.HasTile(pos);
    }

    // When you collect an item, what happens?
    //  (do not deal with deleting item)
    private void CollectItem(Item item, Vector3Int gridPos) {
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
        if (i == 3)
            inventory[inventorySelectedIndex] = new ItemTag(item);
        
        // Play item sound 
        sfxPlayer.Play(itemCollectionAudio);

        // Make item collection effect 
        Vector3 pos = grid.CellToWorld(gridPos) + grid.cellSize / 2.0f;
        ParticleSystem.MainModule settings = collectionEffect.main;
        settings.startColor = new ParticleSystem.MinMaxGradient(item.collectColor);
        Instantiate(collectionEffect, pos, Quaternion.identity);
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
    //
    // This will call a Use{ItemName} function that the player 
    // has to implement. Such functions must be public, and 
    // must assume that the position of an item is located on 
    // the tile that the player is on.
    private void UseItem(ItemTag tag) {
        tag.itemInterface.Function(this);
    }

    // Use a speedup item 
    public void UseSpeedup() {
        stamina = 150.0f;
    }

    // Use a cookie item 
    public void UseCookie() {}

    // Ouch 
    private void Ouch() {
        SceneManager.LoadScene("Assets/Scenes/FailScreen.unity");
    }

    // YouWon
    private void YouWon() {
        SceneManager.LoadScene("Assets/Scenes/YouWonScreen.unity");
    }

    // Lose health if health loss frame 
    private void LoseHealthIfPossible(float lost) {
        if (healthLossFrame) {
            sfxPlayer.Play(healthLossAudio);
            health -= lost;
        }
    }

    // Runs every frame 
    void Update() {
        //> Player input key mapping 
        bool W = Input.GetKey("w");
        bool A = Input.GetKey("a");
        bool S = Input.GetKey("s");
        bool D = Input.GetKey("d");
        bool ANY = W || A || S || D;
        
        bool E = Input.GetKeyDown("e");
        bool SPACE = Input.GetKeyDown("space");

        bool SHIFT = Input.GetKey("left shift") || Input.GetKey("right shift");

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

        //> If the stamina key [SHIFT] is being pressed...
        if (SHIFT) {
            //> If there is stamina left, use it up and sprint, but if 
            //> there is not, do not sprint.
            if (stamina > 0.0f) {
                stamina -= Time.deltaTime * staminaRateDown;
                if (stamina < 0.0f) {
                    stamina = 0.0f;
                }
                sprinting = true;
            } else {
                sprinting = false;
            }
        } else {
            //> If the player is not trying to move, and the player does not have 
            //> full stamina, gradually increase stamina to full by the 
            //> stamina growth rate. Do not sprint.
            if (!ANY && stamina < 100.0f) {
                stamina += Time.deltaTime * staminaRateUp;
                if (stamina > 100.0f) {
                    stamina = 100.0f;
                }
            }
            sprinting = false;
        }

        //> Is the current frame a health loss frame?
        healthLossFrame = false;
        playerHealthLossTimer -= Time.deltaTime;
        if (playerHealthLossTimer < 0.0f) {
            playerHealthLossTimer = playerHealthLossDelay;
            healthLossFrame = true;
        }

        //> If the player has no health left, commence fail sequence 
        if (health <= 0.0f) {
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
                Tilemap snakeTM = snakes.transform.GetChild(i).GetComponent<Tilemap>();

                //> Snake bite, ouch :(
                if (snakeTM.HasTile(pos)) {
                    LoseHealthIfPossible(snakeBiteDamage);
                }
            }
        }

        //> If the player is not moving, but is holding E, and the currently 
        //> selected item slot is nonempty, then use it 
        if (!moving && E)
            UseItemSelected();

        //> If the player presses space, move to the next inventory slot 
        //> and cycle if necessary; play audio as well 
        if (SPACE) {
            inventorySelectedIndex = (inventorySelectedIndex + 1) % 3;
            sfxPlayer.Play(inventoryToggleAudio);
        }

        //> If the updated delay timer is out, the player is not moving,
        //> and the player pressed a key after the delay timer went off,
        //> then enter the move sequence. Adjust for sprinting.
        delayTimer -= Time.deltaTime * (sprinting ? staminaSpeedMultiplier : 1.0f);
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

        //> Get current player position as a tile position 
        Vector3Int gridPos = grid.WorldToCell(transform.position);

        //> If the player is moving, let the move delay timer count down,
        //> and lerp the player position. Adjust for sprinting.
        if (moving) {
            moveDelayTimer -= Time.deltaTime * (sprinting ? staminaSpeedMultiplier : 1.0f);
            float dt = 1.0f - (moveDelayTimer / moveDelay);
            transform.position = Vector3.Lerp(origPos, destPos, dt);

            //> If the move sequence is over, snap the player to the 
            //> destination position, stop moving, and start delaying 
            if (moveDelayTimer < 0.0f) {
                transform.position = destPos;
                moving = false;
                delayTimer = delay;

                //> Check if you are stepping on an item 
                TileBase itemTile = items.GetTile(gridPos);
                if (itemTile != null && itemTile is Item) {
                    CollectItem(itemTile as Item, gridPos);
                    items.SetTile(gridPos, null);
                }

                //> Check if you are stepping on gold 
                if (gold.HasGold(gridPos)) {
                    //> Collect gold at that location 
                    gold.CollectAt(gridPos);
                    ++goldCollected;
                }
            }
        }

        //> Check if you are on a background tile, and if not, ouch 
        if (!background.HasTile(gridPos)) {
            LoseHealthIfPossible(nonBackgroundDamage);
        }

        //> If player is sprinting, amplify postprocessor bloom effect,
        //> but if not, unamplify it. Do everything smoothly.
        VolumeProfile profile = volume.sharedProfile;
        volume.profile.TryGet(out postProcessorBloom);
        postProcessorBloom.intensity.value *= 1.0f - bloomAdjustRate * Time.deltaTime;
        postProcessorBloom.intensity.value +=
            bloomAdjustRate * Time.deltaTime *
                (sprinting ? bloomSprinting : bloomWalking);
        
        //> Adjust player camera position. Do everything smoothly.
        float oldZ = playerCamera.transform.position.z;
        playerCamera.transform.position *= 1.0f - playerCameraAdjustRate * Time.deltaTime;
        playerCamera.transform.position += playerCameraAdjustRate * Time.deltaTime * transform.position;
        playerCamera.transform.position = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y, oldZ);
    }
}