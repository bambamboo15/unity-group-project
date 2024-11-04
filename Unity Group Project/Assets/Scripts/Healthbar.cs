using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Healthbar : MonoBehaviour {
    // The player 
    [SerializeField] private Player player;

    // The reference healthbar 
    [SerializeField] private RectTransform reference;

    // The health healthbar 
    [SerializeField] private RectTransform health;

    // The health text 
    [SerializeField] private TMP_Text healthText;

    // Dynamically change everything 
    void Update() {
        healthText.text = player.health.ToString();
        health.sizeDelta = new Vector2(
            reference.sizeDelta.x * (player.health / 100.0f),
            reference.sizeDelta.y 
        );
        health.anchoredPosition = new Vector3(
            reference.anchoredPosition.x - reference.sizeDelta.x * 0.5f + health.sizeDelta.x * 0.5f,
            reference.anchoredPosition.y 
        );
    }
}