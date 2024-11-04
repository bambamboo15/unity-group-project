using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using TMPro;

// Let us remind the player of how much gold they have.
public class GoldCounter : MonoBehaviour {
    // The player that we need to remind (and take how 
    // much gold they have)
    public Player player;

    // The gold 
    public Gold gold;

    // Our text component 
    private TMP_Text textComponent;

    // Previous amount of gold collected 
    private int prevGoldCollected;

    // Initialize our text component 
    void Start() {
        textComponent = GetComponent<TMP_Text>();
        prevGoldCollected = player.goldCollected;
    }

    // Constantly update our text 
    void Update() {
        if (prevGoldCollected != player.goldCollected) {
            prevGoldCollected = player.goldCollected;
            Color color = textComponent.color;
            color.a = 0.5f;
            textComponent.color = color;
        }
        if (textComponent.color.a < 1.0f) {
            Color color = textComponent.color;
            color.a += Time.deltaTime * 2.0f;
            textComponent.color = color;
        }
        textComponent.text = String.Format("{0}/{1}", player.goldCollected, gold.goldAmount);
    }
}