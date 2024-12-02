using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Negative correlation between how close it gets to the due date 
// and the quality of the code 
public class FailScreenManager : MonoBehaviour {
    [SerializeField] private AudioClip failAudio;
    [SerializeField] private SFXPlayer sfxPlayer;
    [SerializeField] private Image a;
    [SerializeField] private Image e;
    [SerializeField] private TMP_Text b;
    [SerializeField] private TMP_Text c;
    [SerializeField] private TMP_Text d;
    private RectTransform br;
    private RectTransform cr;
    private RectTransform dr;

    void Start() {
        sfxPlayer.Play(failAudio);
        b.overrideColorTags = true;
        c.overrideColorTags = true;
        d.overrideColorTags = true;
        a.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        b.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        c.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        d.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        br = b.GetComponent<RectTransform>();
        cr = c.GetComponent<RectTransform>();
        dr = d.GetComponent<RectTransform>();
    }

    void Update() {
        if (Time.timeSinceLevelLoad < 1.5f) {
            a.color = new Color(1.0f, 0.0f, 0.0f, 0.15f * Mathf.Sin((Mathf.PI / 2.0f) * (Time.timeSinceLevelLoad / 1.5f)));
        }
        
        if (Time.timeSinceLevelLoad > 1.0f && Time.timeSinceLevelLoad < 3.0f) {
            float p = Mathf.Sin((Mathf.PI / 2.0f) * ((Time.timeSinceLevelLoad - 1.0f) / 2.0f));
            p = 1.0f - p;
            p *= p;
            p = 1.0f - p;
            b.color = new Color(1.0f, 0.0f, 0.0f, p * p * p);
            br.anchoredPosition = new Vector3(0.0f, 89.0f - 51.0f * p);
        }
        
        if (Time.timeSinceLevelLoad > 2.0f && Time.timeSinceLevelLoad < 4.0f) {
            float p = Mathf.Sin((Mathf.PI / 2.0f) * ((Time.timeSinceLevelLoad - 2.0f) / 2.0f));
            p = 1.0f - p;
            p *= p;
            p = 1.0f - p;
            c.color = new Color(1.0f, 1.0f, 1.0f, p * p * p);
            cr.anchoredPosition = new Vector3(0.0f, -13.0f - 51.0f * p);
        }

        if (Time.timeSinceLevelLoad > 6.0f) {
            SceneManager.LoadScene("MenuScreen");
        } else if (Time.timeSinceLevelLoad > 5.0f) {
            e.color += new Color(0.0f, 0.0f, 0.0f, Time.deltaTime);
        }
    }
}