
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Animator menuAnimator;
    [SerializeField] GameObject creditsScreen;
    [SerializeField] GameObject controlsScreen;
    [SerializeField] GameObject modsScreen;
    AudioSource audioPlayer;

    //Button[] allButtons;

    bool busy;
    private void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();

        creditsScreen.SetActive(false);
        controlsScreen.SetActive(false);
        modsScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void startGame() {

        if (busy) return;

        busy = true;
        hideUi();

        loadFirstLevel();

    }

    void hideUi() {
        menuAnimator.Play("menuHide");
    }
    IEnumerator LoadYourAsyncScene()
    {
        yield return new WaitForSeconds(0.8f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    void loadFirstLevel() {
        StartCoroutine(LoadYourAsyncScene());
    }


    public void hoverButton(Button hoveredButton) {
        audioPlayer.Play();

        hoveredButton.GetComponentInChildren<TMPro.TMP_Text>().color = hoveredButton.colors.highlightedColor;


    }
    public void exitHover(Button hoveredButton) {
        hoveredButton.GetComponentInChildren<TMPro.TMP_Text>().color = hoveredButton.colors.normalColor;
    }

    public void toggleCredits() {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
        modsScreen.SetActive(false);
        controlsScreen.SetActive(false);
    }

    public void toggleControls() {
        creditsScreen.SetActive(false);
        modsScreen.SetActive(false);
        controlsScreen.SetActive(!controlsScreen.activeSelf);
    }

    public void toggleMods() {

        creditsScreen.SetActive(false);
        controlsScreen.SetActive(false);

        modsScreen.SetActive(!modsScreen.activeSelf);
    }

    public void quitGame() {
        Application.Quit();
    }
}
