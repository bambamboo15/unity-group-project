using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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

    // Our rect transform 
    private RectTransform rectTransform;
    private Vector2 originalTransform;

    // Previous amount of gold collected 
    private int prevGoldCollected;

    // End sequence variables 
    private bool endSequence = false;
    private float endSequenceTimer = 0.0f;

    // Initialize our text component 
    void Start() {
        textComponent = GetComponent<TMP_Text>();
        rectTransform = transform.parent.GetComponent<RectTransform>();
        originalTransform = rectTransform.anchoredPosition;
        prevGoldCollected = player.goldCollected;
    }

    // Constantly update our text 
    void Update() {
        if (prevGoldCollected != player.goldCollected) {
            prevGoldCollected = player.goldCollected;
            Color color = textComponent.color;
            color.a = 0.5f;
            textComponent.color = color;
            if (player.goldCollected == 1) {
                endSequenceTimer = Time.time;
                endSequence = true;
            }
        }
        if (textComponent.color.a < 1.0f) {
            Color color = textComponent.color;
            color.a += Time.deltaTime * 2.0f;
            textComponent.color = color;
        }
        if (endSequence) {
            float progress = (Time.time - endSequenceTimer) * 2.0f - 1.0f;
            rectTransform.anchoredPosition = new Vector2(
                originalTransform.x,
                originalTransform.y + 20.0f * (progress * progress - 1.0f)
            );

            Vignette postProcessorVignette;
            VolumeProfile profile = player.volume.sharedProfile;
            player.volume.profile.TryGet(out postProcessorVignette);
            postProcessorVignette.intensity.value *= 1.0f - Time.deltaTime;
            postProcessorVignette.intensity.value += 0.2f * Time.deltaTime;
        }
        textComponent.text = String.Format("{0}/{1}", player.goldCollected, gold.goldAmount);
    }
}