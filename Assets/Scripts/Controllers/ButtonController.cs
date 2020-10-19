using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void ClickAction();

public enum ClickActions {
    LoadGame,
    SaveAndQuitToMenu,
    QuitGame,
    ResumeGame
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
        }

        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(delegate () { clickAction(); }) ;
    }
}
