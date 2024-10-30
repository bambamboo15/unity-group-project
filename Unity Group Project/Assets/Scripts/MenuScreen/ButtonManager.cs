using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {
    public void MapSelect() {
        SceneManager.LoadScene("Assets/Scenes/MapSelect.unity");
    }
    
    public void ClassicMode() {
        SceneManager.LoadScene("Assets/Scenes/Maps/Beginner Snake.unity");
    }
}