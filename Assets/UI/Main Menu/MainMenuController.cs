using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public VisualElement ui;

    public Button startButton;
    public Button quitButton;

    void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        doc.rootVisualElement.schedule.Execute(() =>
        {
            ui = doc.rootVisualElement;

            startButton = ui.Q<Button>("StartButton");
            startButton.clicked += StartClicked;

            quitButton = ui.Q<Button>("QuitButton");
            quitButton.clicked += QuitClicked;

        }).ExecuteLater(0);
    }

    public void StartClicked()
    {
        SceneManager.LoadScene("Test");
    }

    public void QuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}