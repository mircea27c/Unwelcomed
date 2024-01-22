using UnityEngine;
using UnityEngine.Playables;

public class BeatEnemyLevel : MonoBehaviour
{
    [SerializeField] PlayableDirector scene;
    bool waitingForInput;

    public void pause() {
        scene.Pause();
        waitingForInput = true;
    }

    private void Update()
    {
        if (!waitingForInput) return;
        if (Input.GetMouseButtonDown(0)) {
            scene.Play();
            waitingForInput = false;
        }
    }
}
