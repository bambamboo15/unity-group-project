using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour {
    // this was at 9pm in the night so this code is not 
    // going to be that good 
    // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA 
    [SerializeField] private TMP_Text a;
    [SerializeField] private TMP_Text b;
    [SerializeField] private SFXPlayer x;
    [SerializeField] private AudioClip m;
    private float d = 0.075f;
    private float p = 0.0f;
    private bool l = false;

    void FixedUpdate() {
        if (l)
            d /= 1.05f;
    }

    void Update() {
        a.overrideColorTags = true;
        b.overrideColorTags = true;
        Color q = new Color(1.0f, 1.0f, 1.0f, d);
        a.color = q;
        b.color = q;
        p += Time.deltaTime * 40.0f;
        if (p > 100.0f && !l) {
            d = 1.0f;
            l = true;
            x.Play(m);
        }
        if (p > 200.0f)
            SceneManager.LoadScene("MenuScreen");
        b.text = Mathf.Min(100, Mathf.RoundToInt(p)) + "% done loaded";
    }
}