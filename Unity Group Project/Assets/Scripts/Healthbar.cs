using UnityEngine;
using TMPro;

public class Healthbar : MonoBehaviour {
    // The player 
    [SerializeField] private Player player;

    // The reference healthbar 
    [SerializeField] private Transform reference;

    // The health healthbar 
    [SerializeField] private Transform health;

    // The health text 
    [SerializeField] private TMP_Text healthText;

    // Dynamically change everything 
    void Update() {
        healthText.text = player.health.ToString();
        health.localScale = new Vector3(
            reference.localScale.x * (player.health / 100.0f),
            reference.localScale.y,
            reference.localScale.z 
        );
        health.position = new Vector3(
            reference.position.x - reference.localScale.x * 0.5f + health.localScale.x * 0.5f,
            reference.position.y,
            reference.position.z 
        );
    }
}