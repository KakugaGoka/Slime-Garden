using UnityEngine;
using UnityEngine.UI;

public delegate void ClickAction();

public enum ClickActions {
    NewGame,
    LoadGame,
    SaveAndQuitToMenu,
    QuitGame,
    ResumeGame,
    QuitToMenu,
    SelectShopItem
}

public class ButtonController : MonoBehaviour
{
    public ClickActions actionType;

    private ClickAction clickAction;
    private Button m_Button;
    void Start()
    {
        switch (actionType) {
            case ClickActions.QuitGame:
                clickAction = SaveLoadManager.QuitFromMenu; break;
            case ClickActions.LoadGame:
                clickAction = SaveLoadManager.LoadGame; break;
            case ClickActions.SaveAndQuitToMenu:
                clickAction = SaveLoadManager.SaveAndQuit; break;
            case ClickActions.ResumeGame:
                clickAction = GameFlowManager.Pause; break;
            case ClickActions.NewGame:
                clickAction = SaveLoadManager.NewGame; break;
            case ClickActions.QuitToMenu:
                clickAction = SaveLoadManager.QuitFromGame; break;
        }

        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(delegate () { clickAction(); }) ;
    }
}
