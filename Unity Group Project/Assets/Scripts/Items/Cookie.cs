using UnityEngine;
using UnityEngine.Tilemaps;

// Gives the player a cookie to distract the snake 
[CreateAssetMenu]
public class CookieItem : Item, IItem {
    public void Function(Player player) { player.UseCookie(); }
}