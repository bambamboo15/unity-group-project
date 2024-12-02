using UnityEngine;
using UnityEngine.Tilemaps;

// Gives the player a speedup 
[CreateAssetMenu]
public class WaterBalloonItem : Item, IItem {
    public void Function(Player player) { player.UseWaterBalloon(); }
}