using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuScreenManager : MonoBehaviour {
    [SerializeField] private Image transition;
    [SerializeField] private MenuScreenOption[] options;
    [SerializeField] private Color textSelected;
    [SerializeField] private Color textUnselected;
    [SerializeField] private SFXPlayer sfxPlayer;
    [SerializeField] private AudioClip toggleAudio;
    [SerializeField] private AudioClip selectAudio;
    [SerializeField] private float selectionDelay;
    private string sceneToLoad;
    private bool selectionApplied;
    private bool sceneAlreadyLoaded;
    private float progress;
    private int index;

    void Start() {
        transition.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        progress = 0.0f;
        index = 0;
        selectionApplied = false;
        sceneAlreadyLoaded = false;
    }
    
    void Update() {
        if (!selectionApplied) {
            bool UP = Input.GetKeyDown("up");
            bool DOWN = Input.GetKeyDown("down");
            bool ENTER = Input.GetKeyDown("enter");
            bool RETURN = Input.GetKeyDown("return");

            MenuScreenOption option = options[index];
            TMP_Text text = option.GetComponent<TMP_Text>();
            text.overrideColorTags = true;
            text.color = textSelected;

            if (ENTER || RETURN) {
                if (option.optionActive) {
                    sceneToLoad = option.sceneToLoad;
                    selectionApplied = true;
                    sfxPlayer.Play(selectAudio);
                }
            } else if (UP) {
                index = (index - 1 + options.Length) % options.Length;
                text.color = textUnselected;
                sfxPlayer.Play(toggleAudio);
            } else if (DOWN) {
                index = (index + 1) % options.Length;
                text.color = textUnselected;
                sfxPlayer.Play(toggleAudio);
            }
            
            Color color = transition.color;
            color.a = 1.0f - Mathf.Min(1.0f - Mathf.Pow(1.0f - (progress += Time.deltaTime), 5.0f), 1.0f);
            transition.color = color;
        } else {
            if (selectionDelay > 0.0f) {
                selectionDelay -= Time.deltaTime;
            } else {
                if (!sceneAlreadyLoaded) {
                    SceneManager.LoadScene(sceneToLoad);
                } else {
                    sceneAlreadyLoaded = true;
                }
            }
        }
    }
}