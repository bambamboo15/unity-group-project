using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialManager : MonoBehaviour {
    [SerializeField] private Image transition;
    private float selectionDelay = 0.0f;
    private bool l = false;

    void Update() {
        if (!l) {
            Color color = transition.color;
            color.a = 1.0f - Mathf.Min(1.0f - Mathf.Pow(1.0f - Time.timeSinceLevelLoad, 5.0f), 1.0f);
            transition.color = color;

            if (Time.timeSinceLevelLoad > 1.0f && Input.GetKeyDown("escape"))
                l = true;
        } else {
            if (selectionDelay < 0.0f)
                SceneManager.LoadScene("MenuScreen");
            else 
                selectionDelay -= Time.deltaTime;
        }
    }
}