using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MapSelectManager : MonoBehaviour {
    [SerializeField] private Transform[] maps;
    [SerializeField] private int selectedIndex;
    [SerializeField] private Color textUnselected;
    [SerializeField] private Color textSelected;
    [SerializeField] private Material borderSelected;
    [SerializeField] private Material borderUnselected;
    [SerializeField] private Image transition;
    [SerializeField] private Image aaa;
    [SerializeField] private Image bbb;
    [SerializeField] private SFXPlayer sfxPlayer;
    [SerializeField] private AudioClip toggleAudio;
    [SerializeField] private AudioClip selectAudio;
    private float selectionDelay = 0.0f;
    private bool l = false;
    private float m = 0.0f;
    private string sceneToLoad = "MenuScreen";

    void Update() {
        if (!l) {
            for (int i = 0; i != maps.Length; ++i) {
                Transform map = maps[i];
                Transform borderT = map.GetChild(0);
                Transform pictureT = map.GetChild(1).GetChild(0);
                Transform container = map.GetChild(2);
                Transform nameT = container.GetChild(0);
                Transform descT = container.GetChild(1);
                TMP_Text name = nameT.GetComponent<TMP_Text>();
                TMP_Text desc = descT.GetComponent<TMP_Text>();
                Image border = borderT.GetComponent<Image>();
                Image picture = pictureT.GetComponent<Image>();
                name.overrideColorTags = true;
                desc.overrideColorTags = true;

                if (i == selectedIndex) {
                    name.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    desc.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    border.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    picture.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                    map.parent.localPosition = -map.localPosition +
                        new Vector3(0.0f, 20.0f * m * Mathf.Sin(Time.time * 10.0f), 0.0f);
                } else {
                    name.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    desc.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    border.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    picture.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                }
            }

            if (Input.GetKeyDown("up")) {
                --selectedIndex;
                if (selectedIndex == -1) selectedIndex = maps.Length - 1;
                m = 0.2f;
                aaa.color = new Color(1.0f, 0.85f, 0.0f, 1.0f);
                sfxPlayer.Play(toggleAudio);
            } else if (Input.GetKeyDown("down")) {
                ++selectedIndex;
                if (selectedIndex == maps.Length) selectedIndex = 0;
                m = 0.2f;
                bbb.color = new Color(1.0f, 0.85f, 0.0f, 1.0f);
                sfxPlayer.Play(toggleAudio);
            } else if (Time.timeSinceLevelLoad > 1.0f && (Input.GetKeyDown("enter") || Input.GetKeyDown("return"))) {
                sceneToLoad = maps[selectedIndex].GetComponent<MapSelectOption>().sceneToLoad;
                selectionDelay = 0.5f;
                l = true;
            }

            aaa.color *= 1.0f - 5.0f * Time.deltaTime;
            aaa.color += 5.0f * Time.deltaTime * Color.white;
            bbb.color *= 1.0f - 5.0f * Time.deltaTime;
            bbb.color += 5.0f * Time.deltaTime * Color.white;

            m -= Time.deltaTime;
            if (m < 0.0f) m = 0.0f;

            Color color = transition.color;
            color.a = 1.0f - Mathf.Min(1.0f - Mathf.Pow(1.0f - Time.timeSinceLevelLoad, 5.0f), 1.0f);
            transition.color = color;

            if (Time.timeSinceLevelLoad > 1.0f && Input.GetKeyDown("escape"))
                l = true;
        } else {
            if (selectionDelay < 0.0f)
                SceneManager.LoadScene(sceneToLoad);
            else 
                selectionDelay -= Time.deltaTime;
        }
    }
}