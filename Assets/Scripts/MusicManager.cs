using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] AudioClip[] levelTracks;
    AudioSource audioPlr;
    private void Awake()
    {
        audioPlr = GetComponent<AudioSource>();
        instance = this;
    }
    public void playTrack(int nr, float volume) {
        if (nr >= levelTracks.Length) {
            return;
        }
        audioPlr.Stop();
        audioPlr.clip = levelTracks[nr];
        audioPlr.volume = volume;
        audioPlr.time = 0;
        audioPlr.Play();
        
    }
}
