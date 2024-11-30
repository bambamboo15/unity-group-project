using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Staminometer : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private RectTransform reference;
    [SerializeField] private RectTransform stamina;
    private Image staminaImage;

    void Start() {
        staminaImage = stamina.transform.GetComponent<Image>();
    }

    void Update() {
        float adjust = (player.stamina / 50.0f) - 2.0f;
        if (adjust < 0.0f) adjust = 0.0f;
        if (adjust > 1.0f) adjust = 1.0f;
        staminaImage.color = new Color(adjust, 1.0f, adjust, 1.0f);
        stamina.sizeDelta = new Vector2(
            reference.sizeDelta.x * (player.stamina / 100.0f),
            reference.sizeDelta.y 
        );
        stamina.anchoredPosition = new Vector3(
            reference.anchoredPosition.x - reference.sizeDelta.x * 0.5f + stamina.sizeDelta.x * 0.5f,
            reference.anchoredPosition.y 
        );
    }
}