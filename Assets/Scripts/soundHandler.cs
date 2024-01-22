using UnityEngine;

public class soundHandler : MonoBehaviour
{
    [SerializeField]AudioSource player;
    [SerializeField] AudioClip[] soundsToPlay;
    int soundIndex;

    public void playNextSound() {
        if (soundIndex < soundsToPlay.Length) {
            player.PlayOneShot(soundsToPlay[soundIndex]);
            soundIndex++;
        } 
    }
}
