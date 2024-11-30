using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public class Snake : MonoBehaviour {
    // The player the snake should be directed to.
    public Player player;

    // The grid for layout purposes 
    public Grid grid;

    // The snake tiles 
    public SnakeHeadTile snakeHeadTile;
    public SnakeBodyTile snakeBodyTile;

    // Other required tilemaps 
    public Tilemap walls;

    // When the player collects a gold, how much faster 
    // does the snake get exponentially? That is, how 
    // much does the delay multiply everytime that 
    // happens?
    public float goldMultiplier = 0.95f;

    // Sound effect player 
    public SFXPlayer sfxPlayer;

    // Audio cues that the snake will make 
    public AudioClip moveAudio;
    public AudioClip cookieAudio;

    // Other configurations 
    public float moveInterval;
    public int restartLength;
    public float restartMultiplier;

    // Snake body data structure. Essentially what this is are all 
    // the tiles of the snake. The last element of this "body" list 
    // is the head of the snake. The length of this list is the snake 
    // length.
    private LinkedList<Vector3Int> body = new LinkedList<Vector3Int>();

    // Private variables 
    private GridLayout gridLayout;
    private Tilemap tilemap;
    private float moveIntervalTimer;
    private bool stuck = false;

    // Initialize the internal data structure based on prepared tiles.
    void Start() {
        tilemap = GetComponent<Tilemap>();
        gridLayout = grid.GetComponent<GridLayout>();
        PreemptivelyFindSnakeBodyAppend();
    }

    // What the snake does every frame 
    void Update() {
        moveIntervalTimer -= Time.deltaTime;
        if (moveIntervalTimer <= 0.0f) {
            moveIntervalTimer = moveInterval;
            Move(CalculateBestDirection());
        }
    }

    // Apply gold collection to the snake 
    public void ApplyGoldCollection() {
        moveInterval *= goldMultiplier;
    }

    // Calculate the "best direction" of the snake. It is what the snake 
    // thinks is the optimal direction to take towards the player. Of course,
    // it will not be optimal, but we do not want the game to be impossible.
    //
    // If the snake is somehow "trapped", it will return a (0, 0, 0)
    public Vector3Int CalculateBestDirection() {
        Vector3Int head = Head(), output = Vector3Int.zero,
                   playerPos = gridLayout.WorldToCell(player.transform.position);
        float minDistance = float.PositiveInfinity;

        if (!isBlocked(head + Vector3Int.up)) {
            float distance = Vector3Int.Distance(head + Vector3Int.up, playerPos);
            if (distance < minDistance) {
                minDistance = distance;
                output = Vector3Int.up;
            }
        }

        if (!isBlocked(head + Vector3Int.down)) {
            float distance = Vector3Int.Distance(head + Vector3Int.down, playerPos);
            if (distance < minDistance) {
                minDistance = distance;
                output = Vector3Int.down;
            }
        }

        if (!isBlocked(head + Vector3Int.left)) {
            float distance = Vector3Int.Distance(head + Vector3Int.left, playerPos);
            if (distance < minDistance) {
                minDistance = distance;
                output = Vector3Int.left;
            }
        }

        if (!isBlocked(head + Vector3Int.right)) {
            float distance = Vector3Int.Distance(head + Vector3Int.right, playerPos);
            if (distance < minDistance) {
                minDistance = distance;
                output = Vector3Int.right;
            }
        }

        return output;
    }

    // Is the square disallowed from the snake? The only possibilities are:
    //    - a square such that the player cannot go to 
    public bool isBlocked(Vector3Int pos) {
        return player.isBlocked(pos);
    }

    // Move the snake in a certain direction!
    //
    // If, somehow, the direction equals the zero vector,
    // the snake will go to its restart length, however,
    // it will get slower.
    public void Move(Vector3Int dir) {
        if (dir.Equals(Vector3Int.zero)) {
            for (int length = Length(); length > restartLength; --length) {
                Vector3Int tail = Tail();
                tilemap.SetTile(tail, null);
                body.RemoveFirst();
            }
            if (!stuck) {
                moveInterval *= restartMultiplier;
                stuck = true;
            }
        } else {
            Vector3Int head = Head(), tail = Tail();

            tilemap.SetTile(head, snakeBodyTile);
            tilemap.SetTile(head + dir, snakeHeadTile);
            tilemap.SetTile(tail, null);

            body.AddLast(head + dir);
            body.RemoveFirst();
            stuck = false;
            
            sfxPlayer.Play(moveAudio, head);
        }
    }

    // Get the tail of the snake 
    public Vector3Int Tail() {
        return body.First.Value;
    }
    
    // Get the head of the snake 
    public Vector3Int Head() {
        return body.Last.Value;
    }

    // Get the length of the snake 
    public int Length() {
        return body.Count;
    }

    // Finds out where the snake head tile is right after tilemap 
    // is obtained. An expensive operation, complexity O(area)
    private Vector3Int PreemptivelyFindSnakeHead() {
        for (int x = tilemap.origin.x; x != tilemap.origin.x + tilemap.size.x; ++x) {
            for (int y = tilemap.origin.y; y != tilemap.origin.y + tilemap.size.y; ++y) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(pos);
                
                if (tile is SnakeHeadTile)
                    return pos;
            }
        }

        throw new InvalidOperationException("Snake head not found!");
    }
    
    // This function gets the entire snake body from prepared tiles.
    // This sets the original length counter accordingly.
    //
    // Now, a problem is how to get snake data from us painting the 
    // snake. Remember, we do not assign an order to any snake tile.
    // We take care of this problem by "backtracking" from the snake 
    // head tile, with the condition that snake tiles never form a 
    // junction.
    // 
    // Please do not incur undefined behavior by using snake tiles 
    // in a wrong way! The exceptions here are only for unreachable 
    // code path warning silencing, and precautions are taken not 
    // to crash Unity when undefined behavior happens.
    private void PreemptivelyFindSnakeBodyAppend() {
        Vector3Int pos = PreemptivelyFindSnakeHead();
        body.AddLast(pos);

        Vector3Int last = pos;
        int safety_counter = 0;
        while (safety_counter != 100) {
            Vector3Int up = pos + Vector3Int.up;
            if (last != up && tilemap.HasTile(up)) {
                body.AddFirst(up);
                last = pos;
                pos = up;
                continue;
            }

            Vector3Int down = pos + Vector3Int.down;
            if (last != down && tilemap.HasTile(down)) {
                body.AddFirst(down);
                last = pos;
                pos = down;
                continue;
            }

            Vector3Int left = pos + Vector3Int.left;
            if (last != left && tilemap.HasTile(left)) {
                body.AddFirst(left);
                last = pos;
                pos = left;
                continue;
            }

            Vector3Int right = pos + Vector3Int.right;
            if (last != right && tilemap.HasTile(right)) {
                body.AddFirst(right);
                last = pos;
                pos = right;
                continue;
            }
            return;
        }

        throw new InvalidOperationException("Undefined behavior detected!");
    }
}