using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {
    public void MapSelect() {
        SceneManager.LoadScene("Assets/Scenes/MapSelect.unity");
    }
    
    public void ClassicMode() {
        SceneManager.LoadScene("Assets/Scenes/Maps/Demo.unity");
    }

    public void LoadDemo() {
        SceneManager.LoadScene("Assets/Scenes/Maps/Demo.unity");
    }
}