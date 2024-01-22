using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]Level[] sceneLevels;
    [SerializeField] Level currentLevel;
    int nextLvlIndex = 0;

    bool restarting;

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        nextLevel();
    }
    private void Update()
    {
        if (restarting) return;

        if(currentLevel == null)
        {
            return;
        }

        if (currentLevel.levelCompleted()) {
            nextLevel();
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (currentLevel is Cutscene)
            {
                Cutscene currentScene = (Cutscene)currentLevel;
                currentScene.skip();
            }
            else
            {
                nextLevel();
            }
        }
#if UNITY_EDITOR
        processEditorCommands();

#endif
    }
    void processEditorCommands() {

        if (Input.GetKeyDown(KeyCode.P))
        {
            restartLevel();
        }

    }
    public void nextLevel() {
        if (currentLevel != null) {
            currentLevel.onLevelEnd();
        }
        if (nextLvlIndex < sceneLevels.Length)
        {
            currentLevel = sceneLevels[nextLvlIndex];
            currentLevel.startLevel();
            nextLvlIndex++;
        }
        else
        {
            //play la urmatoarea scena
            currentLevel = null;

            if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    public void restartLevel() {
        if (restarting) return;
        
        restarting = true;
        HUDManager.instance.playLevelFailAnim();

        StartCoroutine(restartLvlEnum());
    }
    IEnumerator restartLvlEnum()
    {
        yield return new WaitForSeconds(1.9f);

        currentLevel.restartLevel();
        PlayerManager.instance.health = 100;
        PlayerManager.instance.takeDamage(0);


        yield return new WaitForSeconds(0.1f);
        restarting = false;
    }
    
}
