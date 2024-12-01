using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private GridLayout gridLayout;
    private AudioSource audioSource;

    public void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip audioClip, float volume = 1.0f) {
        audioSource.PlayOneShot(audioClip, volume);
    }

    public void Play(AudioClip audioClip, Vector3 pos, float volume = 1.0f) {
        Play(audioClip, volume / (Vector3.Distance(player.transform.position, pos) * 0.5f + 1.0f));
    }

    public void Play(AudioClip audioClip, Vector3Int pos, float volume = 1.0f) {
        Play(audioClip, gridLayout.CellToWorld(pos), volume);
    }
}