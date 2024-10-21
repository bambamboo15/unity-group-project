using UnityEngine;

// The player collects gold. Collect enough and 
// you win. However, the game progressively gets harder 
// and harder when you collect more gold, at least for 
// classic mode.
public class Gold : Action {
    // Has the gold been collected?
    public bool collected = false;

    // Private variables 
    private SpriteRenderer sr;

    // Gold initialization script 
    public override void Start() {
        sr = GetComponent<SpriteRenderer>();
        base.Start();
    }

    // When the gold is collected by the player 
    public override void Triggered() {
        if (!collected) {
            collected = true;
            sr.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            ++player.gold;
        }
    }
}