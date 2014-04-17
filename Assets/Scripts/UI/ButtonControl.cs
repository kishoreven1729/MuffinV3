﻿using UnityEngine;
using System.Collections;

public class ButtonControl : MonoBehaviour
{

    public enum ButtonType
    {
        Play,
        Credits,
        Pause,
        Resume,
		Share
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
                GUIManager.guiInstance.ShowGamePausedPanel();
//                type = ButtonType.Resume;
                break;
            case ButtonType.Resume:
                GameDirector.gameInstance.ResumeGame();
                GUIManager.guiInstance.ShowInGamePanel();
//                type = ButtonType.Pause;
                break;
			case ButtonType.Share:
				Debug.Log("Facebook Share Clicked");
				ScoringDirector.scoringInstance.PostOnFacebook();
				type = ButtonType.Share;
				break;
        }
    }

}
