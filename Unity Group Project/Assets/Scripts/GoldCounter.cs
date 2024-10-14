using UnityEngine;
using TMPro;
using System;

// Let us remind the player of how much gold they have.
public class GoldCounter : MonoBehaviour {
    // The player that we need to remind (and take how 
    // much gold they have)
    public Player player;

    // Our text component 
    private TMP_Text textComponent;

    // Initialize our text component 
    void Start() {
        textComponent = GetComponent<TMP_Text>();
    }

    // Constantly update our text 
    void Update() {
        textComponent.text = String.Format("Gold: {0}/{1}", player.gold, player.maxGold);
    }
}