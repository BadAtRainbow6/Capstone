using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : NetworkBehaviour
{
    GameManager gameManager;
    public VisualElement ui;

    public Button nextTurnButton;
    public Button ability1Button;
    public Button ability2Button;

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
            nextTurnButton.clicked += TurnPassClicked;

            ability1Button = ui.Q<Button>("Ability1Button");
            ability1Button.clicked += Ability1Clicked;

            ability2Button = ui.Q<Button>("Ability2Button");
            ability2Button.clicked += Ability2Clicked;

        }).ExecuteLater(0);

        doc.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnClientStarted()
    {
        doc.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void TurnPassClicked()
    {
        gameManager.SwapTurnRpc();
    }

    public void Ability1Clicked()
    {
        gameManager.SetSelectedAbility(0);
    }

    public void Ability2Clicked()
    {
        gameManager.SetSelectedAbility(1);
    }
}
