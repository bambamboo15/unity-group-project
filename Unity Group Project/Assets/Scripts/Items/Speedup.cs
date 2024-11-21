using UnityEngine;
using UnityEngine.Tilemaps;

// Gives the player a speedup 
[CreateAssetMenu]
public class SpeedupItem : Item, IItem {
    public void Function() { Debug.Log("speedup"); }
}