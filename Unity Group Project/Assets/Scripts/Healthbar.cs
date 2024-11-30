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

    // The transition healthbar 
    [SerializeField] private RectTransform transition;

    // The health text 
    [SerializeField] private TMP_Text healthText;

    // Transition smoothing (roughly reciprocal)
    [SerializeField] private float reciprocalSmoothing;

    // Transition value 
    private float transitionValue;

    private void Apply(RectTransform rectTransform, float value) {
        rectTransform.sizeDelta = new Vector2(
            reference.sizeDelta.x * value,
            reference.sizeDelta.y 
        );
        rectTransform.anchoredPosition = new Vector3(
            reference.anchoredPosition.x - reference.sizeDelta.x * 0.5f + rectTransform.sizeDelta.x * 0.5f,
            reference.anchoredPosition.y 
        );
    }
    // Dynamically change everything 
    void Update() {
        healthText.text = Mathf.FloorToInt(player.health).ToString();
        transitionValue *= 1.0f - reciprocalSmoothing * Time.deltaTime;
        transitionValue += reciprocalSmoothing * Time.deltaTime * (player.health / 100.0f);
        Apply(health, player.health / 100.0f);
        Apply(transition, transitionValue);
    }
}