using UnityEngine;

// When the player steps on a tile derived from Action,
// its overridden Triggered method gets called.
//
// Please note that the Triggered method will only be 
// called right after the player moves. Between player 
// movements it will not be called.
public class Action : MonoBehaviour {
    // The player that will be used for action tile bookkeeping 
    public Player player;

    // Add the current tile to player action tiles as a reference 
    void Start() {
        player.actionTiles.Add(this);
    }

    // The overrideable method called Triggered 
    public virtual void Triggered() {}
}