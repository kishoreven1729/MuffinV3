using UnityEngine;
using System.Collections;

public class ButtonControl : MonoBehaviour
{

    public enum ButtonType
    {
        Play,
        Credits,
        Pause,
        Resume
    }

    public ButtonType type;

    void OnClick()
    {
        print("hi");
        switch (type)
        {
            case ButtonType.Play:
                GameDirector.gameInstance.ResetGame();
                break;
            case ButtonType.Credits:
                break;
            case ButtonType.Pause:
                GameDirector.gameInstance.PauseGame();
                type = ButtonType.Resume;
                break;
            case ButtonType.Resume:
                GameDirector.gameInstance.ResumeGame();
                type = ButtonType.Pause;
                break;
        }
    }

}
