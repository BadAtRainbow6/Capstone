using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : NetworkBehaviour
{
    GameManager gameManager;
    public VisualElement ui;

    public Button nextTurnButton;

    UIDocument doc;

    private void Start()
    {
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
    }

    void OnEnable()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        doc = GetComponent<UIDocument>();
        doc.rootVisualElement.schedule.Execute(() =>
        {
            ui = doc.rootVisualElement;

            nextTurnButton = ui.Q<Button>("EndTurnButton");
            nextTurnButton.clicked += TurnPassClicked ;

        }).ExecuteLater(0);

        doc.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnClientStarted()
    {
        doc.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void TurnPassClicked()
    {
        gameManager.SwapTurn();
    }
}
