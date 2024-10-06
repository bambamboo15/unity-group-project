using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public class Snake : MonoBehaviour {
    // The player the snake should be directed to. It can virtually 
    // be any GameObject because only the position is tracked.
    public GameObject player;

    // The grid for layout purposes.
    public Grid grid;

    // The snake tiles.
    public SnakeHeadTile snakeHeadTile;
    public SnakeBodyTile snakeBodyTile;

    public bool _move;
    void Update() {
        if (_move) {
            _move = false;
            Move(Vector3Int.right);
        }
    }

    // Snake body data structure. Essentially what this is are all 
    // the tiles of the snake. The last element of this "body" list 
    // is the head of the snake. The length of this list is the snake 
    // length.
    private LinkedList<Vector3Int> body = new LinkedList<Vector3Int>();

    // Private variables 
    private Tilemap tilemap;

    // Initialize the internal data structure based on prepared tiles.
    void Start() {
        tilemap = GetComponent<Tilemap>();
        PreemptivelyFindSnakeBodyAppend();
    }

    // Move the snake in a certain direction!
    public void Move(Vector3Int dir) {
        Vector3Int head = Head(), tail = Tail();

        tilemap.SetTile(head, snakeBodyTile);
        tilemap.SetTile(head + dir, snakeHeadTile);
        tilemap.SetTile(tail, null);

        body.AddLast(head + dir);
        body.RemoveFirst();
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
            }
            return;
        }

        throw new InvalidOperationException("Undefined behavior detected!");
    }
}