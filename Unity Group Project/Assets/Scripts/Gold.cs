using UnityEngine;

// The player, the food, eats apples. Collect enough and 
// you win. However, the game progressively gets harder 
// and harder when you collect more apples, at least for 
// classic mode.
public class Gold : Action {
    public override void Triggered() {
        ++player.gold;
    }
}