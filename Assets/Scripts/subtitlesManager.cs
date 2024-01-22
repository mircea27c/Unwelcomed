using UnityEngine;
using UnityEngine.UI;

public class subtitlesManager : MonoBehaviour
{
    [SerializeField] Text subtitles;

    [System.Serializable]
    struct subtitle {
        public enum displayType { show, add, remove};
        public displayType display;
        public string text;

    }
    [SerializeField] subtitle[] allSubtitles;

    int subtitleIndex = 0;

    private void Awake()
    {
        clearText();
    }
    public void showText(string text) {
        subtitles.text = text;
    }
    public void clearText() {
        subtitles.text = "";
    }
    public void addText(string text) {
        subtitles.text += "\n" + text;
    }
    public void nextSubtitle() {
        if (subtitleIndex >= allSubtitles.Length) return;
        switch (allSubtitles[subtitleIndex].display)
        {
            case subtitle.displayType.show:
                showText(allSubtitles[subtitleIndex].text);
                break;
            case subtitle.displayType.add:
                addText(allSubtitles[subtitleIndex].text);
                break;
            case subtitle.displayType.remove:
                clearText();
                break;
        }
        subtitleIndex++;
    }
}
