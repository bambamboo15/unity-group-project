using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private GridLayout gridLayout;
    private AudioSource audioSource;

    public void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void AdjustVolume(float volume) {
        audioSource.volume = volume;
    }

    public void PlayPermament(AudioClip audioClip) {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void Play(AudioClip audioClip, float volume = 1.0f) {
        audioSource.PlayOneShot(audioClip, volume);
    }

    public void Play(AudioClip audioClip, Vector3 pos, float volume = 1.0f) {
        Play(audioClip, volume / (Vector3.Distance(player.transform.position, pos) * 0.5f + 1.2f));
    }

    public void Play(AudioClip audioClip, Vector3Int pos, float volume = 1.0f) {
        Play(audioClip, gridLayout.CellToWorld(pos), volume);
    }
}