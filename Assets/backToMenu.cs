using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class backToMenu : MonoBehaviour
{
    [SerializeField] GameObject menu;

    private void Start()
    {
        if (menu.activeSelf) {
            toggleMenu();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            toggleMenu();
        }
    }

    public void toggleMenu() {
        if (menu.activeSelf)
        {
            Time.timeScale = 1;
            menu.SetActive(false); 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else {
            Time.timeScale = 0;
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void goToMenu() {
        Button[] btns = menu.transform.GetComponentsInChildren<Button>();
        foreach (Button button in btns)
        {
            button.enabled = false;
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
