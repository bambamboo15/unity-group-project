using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {
    public void NewGame() {
        SceneManager.LoadScene("Assets/Scenes/Maps/Beginner Snake.unity");
    }
}