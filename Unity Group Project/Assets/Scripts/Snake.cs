using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public class Snake : MonoBehaviour {
    // The player the snake should be directed to.
    [SerializeField] private Player player;

    // The grid for layout purposes 
    [SerializeField] private Grid grid;

    // The snake tiles 
    [SerializeField] private SnakeHeadTile snakeHeadTile;
    [SerializeField] private SnakeBodyTile snakeBodyTile;

    // Other required tilemaps 
    [SerializeField] private Tilemap walls;

    // All of the cookies 
    [SerializeField] private Transform cookies;

    // When the player collects a gold, how much faster 
    // does the snake get exponentially? That is, how 
    // much does the delay multiply everytime that 
    // happens?
    public float goldMultiplier;

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

    // What the snake thinks is the optimal cell position to go to. It always 
    // knows where the player is located, but if there is a cookie somewhere,
    // it cannot resist it :)
    public Vector3Int OptimalCellPosition() {
        if (cookies.childCount != 0) {
            return gridLayout.WorldToCell(cookies.GetChild(0).position);
        } else {
            return gridLayout.WorldToCell(player.transform.position);
        }
    }

    // From the snake head position, handle what happens when the snake is on 
    // a cookie.
    public void HandleCookie() {
        if (cookies.childCount != 0) {
            Transform cookie = cookies.GetChild(0);
            if (Head() == gridLayout.WorldToCell(cookie.position)) {
                Destroy(cookie.gameObject);
                sfxPlayer.Play(cookieAudio, Head());
            }
        }
    }

    // For A*, calculates the distance given two nodes 
    public int AStarDistance(Vector3Int node0, Vector3Int node1) {
        int dx = node0.x - node1.x,
            dy = node0.y - node1.y;
        if (dx < 0) dx = -dx;
        if (dy < 0) dy = -dy;
        return dx + dy;
    }

    // For A*, calculates the g-score given root node 
    public int AStarGScore(Vector3Int node, Vector3Int rootNode) {
        return AStarDistance(node, rootNode);
    }

    // For A*, calculates the h-score given goal node 
    public int AStarHScore(Vector3Int node, Vector3Int goalNode) {
        return AStarDistance(node, goalNode);
    }

    // For A*, calculates the f-score given root and goal nodes 
    public int AStarFScore(Vector3Int node, Vector3Int rootNode, Vector3Int goalNode) {
        return AStarGScore(node, rootNode) + AStarHScore(node, goalNode);
    }

    // All accessible neighbors of a node, through a callback 
    public void AllNeighbors(Vector3Int node, Action<Vector3Int> callback) {
        if (!isBlocked(node + Vector3Int.left))
            callback(node + Vector3Int.left);
        if (!isBlocked(node + Vector3Int.right))
            callback(node + Vector3Int.right);
        if (!isBlocked(node + Vector3Int.up))
            callback(node + Vector3Int.up);
        if (!isBlocked(node + Vector3Int.down))
            callback(node + Vector3Int.down);
    }

    // Calculate the "best direction" of the snake. It is what the snake 
    // thinks is the optimal direction to take towards the player. Of course,
    // it will not be optimal, but we do not want the game to be impossible.
    //
    // The A* pathfinding algorithm is now used. The reason why this is called 
    // "nonfailure" is because it does not decide what to do when it fails.
    //
    // If the snake is somehow "trapped", it will return a (0, 0, 0)
    public Vector3Int CalculateBestDirectionNonFailure() {
        //> Initialization (from Wikipedia)
        Vector3Int root = Head(), goal = OptimalCellPosition();
        HashSet<Vector3Int> openSet = new HashSet<Vector3Int>();
        openSet.Add(root);
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, int> gScore = new Dictionary<Vector3Int, int>(),
                                    fScore = new Dictionary<Vector3Int, int>();
        gScore[root] = 0;
        fScore[root] = AStarHScore(root, goal);

        //> Nonempty loop 
        while (openSet.Count != 0) {
            //> Node with lowest f-score 
            Vector3Int current = Vector3Int.zero;
            foreach (Vector3Int candidate in openSet) {
                current = candidate;
                break;
            }
            int currentFScore = fScore[current];
            foreach (Vector3Int candidate in openSet) {
                int candidateFScore = fScore[candidate];
                if (candidateFScore < currentFScore) {
                    currentFScore = candidateFScore;
                    current = candidate;
                }
            }

            //> We have reached the goal :D
            //>
            //> Reconstruct the path from end to start.
            //> If there is a glitch, an infinite loop 
            //> may occur, crashing the game.
            //>
            //> Just before full path reconstruction,
            //> we obtain the position just after the 
            //> head of the snake, subtract it by the 
            //> head of the snake, and that outputs 
            //> optimal direction.
            if (current == goal) {
                while (true) {
                    //> If, for some unfortunate reason, the algorithm 
                    //> glitched somewhere (it does in certain cases),
                    //> fail :/
                    if (!cameFrom.ContainsKey(current))
                        return Vector3Int.zero;
                    Vector3Int next = cameFrom[current];
                    if (next == root)
                        return current - root;
                    current = next;
                }
            }

            //> Remove that current node and start the subloop 
            openSet.Remove(current);
            AllNeighbors(current, neighbor => {
                //> Tenative g-score 
                int tenativeGScore = gScore[current] + 1; /* distance(current, neighbor) == 1 */
                if (!gScore.ContainsKey(neighbor) || tenativeGScore < gScore[neighbor]) {
                    //> "This path to neighbor is better than any 
                    //> previous one. Record it!"
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tenativeGScore;
                    fScore[neighbor] = tenativeGScore + AStarHScore(neighbor, goal);
                    openSet.Add(neighbor); /* contains check not required */
                }
            });
        }

        //> Goal never reached :(
        return Vector3Int.zero;
    }

    // Calculate the best direction for the snake. Handle failure cases as well.
    public Vector3Int CalculateBestDirection() {
        Vector3Int output = CalculateBestDirectionNonFailure();
        if (output == Vector3.zero)
            AllNeighbors(Head(), neighbor => output = neighbor);
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
            
            sfxPlayer.Play(moveAudio, Head());

            HandleCookie();
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