using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {
    public void MapSelect() {
        SceneManager.LoadScene("Assets/Scenes/Map Select.unity");
    }
    
    public void ClassicMode() {
        SceneManager.LoadScene("Assets/Scenes/Maps/Beginner Snake.unity");
    }
}