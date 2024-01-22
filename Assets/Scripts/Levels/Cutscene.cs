using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene : Level
{
    [SerializeField] PlayableDirector[] scenes;
    int currentSceneIndex = -1;

    bool finishedPlaying;

    
    private void Awake()
    {
           
    }

    public override void onLevelEnd()
    {
        PlayerManager.instance.setCutsceneMode(false);
        base.onLevelEnd();
        if (HUDManager.instance != null)
        {
            HUDManager.instance.gameObject.SetActive(true);
        }
    }

    public override void startLevel()
    {
        base.startLevel();
        playNextScene();
        PlayerManager.instance?.setCutsceneMode(true);

        if (HUDManager.instance != null)
        {
            HUDManager.instance.gameObject.SetActive(false);
        }
    }
    public override bool levelCompleted()
    {
        return finishedPlaying;
    }

    private void Update()
    {
        if (!active) return;

        if (scenes[currentSceneIndex].time > scenes[currentSceneIndex].duration - 0.04f) {
            playNextScene();
        }
    }

    public void skip() {
        StopAllCoroutines();

        scenes[currentSceneIndex].Stop();

        PlayableDirector dir = scenes[scenes.Length - 1];
        dir.initialTime = dir.duration - 0.1f;
        dir.Play();
    }

    void playNextScene() {
        currentSceneIndex++;
        if (currentSceneIndex > 0) {
            scenes[currentSceneIndex - 1].gameObject.SetActive(false);
            scenes[currentSceneIndex - 1].enabled = false;
        }

        if (currentSceneIndex >= scenes.Length)
        {
            finishedPlaying = true;
            PlayerManager.instance?.setCutsceneMode(false);

            return;
        }

        scenes[currentSceneIndex].gameObject.SetActive(true);
        scenes[currentSceneIndex].enabled = true;
        scenes[currentSceneIndex].Play();

    }
}
