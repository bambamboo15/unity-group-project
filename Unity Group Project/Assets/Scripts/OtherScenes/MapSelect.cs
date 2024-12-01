using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MapSelect : MonoBehaviour {
    [SerializeField] private Transform[] maps;
    [SerializeField] private int selectedIndex;
    [SerializeField] private Color textUnselected;
    [SerializeField] private Color textSelected;
    [SerializeField] private Material borderSelected;
    [SerializeField] private Material borderUnselected;
    [SerializeField] private Image transition;
    [SerializeField] private SFXPlayer sfxPlayer;
    [SerializeField] private AudioClip toggleAudio;
    [SerializeField] private AudioClip selectAudio;
    private float selectionDelay = 0.0f;
    private bool l = false;

    void Update() {
        Transform selected = maps[selectedIndex];
        Transform borderT = selected.GetChild(0);
        Transform container = selected.GetChild(2);
        Transform nameT = container.GetChild(0);
        Transform descT = container.GetChild(1);
        TMP_Text name = nameT.GetComponent<TMP_Text>();
        TMP_Text desc = descT.GetComponent<TMP_Text>();
        Image border = borderT.GetComponent<Image>();
        name.overrideColorTags = true;
        desc.overrideColorTags = true;
        name.color = textSelected;
        desc.color = textSelected;
        border.material = borderSelected;
        Color color = transition.color;
        color.a = 1.0f - Mathf.Min(1.0f - Mathf.Pow(1.0f - Time.timeSinceLevelLoad, 5.0f), 1.0f);
        transition.color = color;
        if (Time.timeSinceLevelLoad > 1.0f && Input.GetKeyDown("escape") && !l) {
            selectionDelay = 0.5f;
            l = true;
        }
    
        if (selectionDelay < 0.0f && l)
            SceneManager.LoadScene("MenuScreen");
        else 
            selectionDelay -= Time.deltaTime;
    }
}