#region References
using UnityEngine;
using System.Collections;
#endregion

public class GUIManager : MonoBehaviour 
{
	#region Private Variables
	private bool _loadGUI;
	#endregion

	#region Public Variables
	public static GUIManager guiInstance;

	public GameObject startPanel;
	public GameObject inGamePanel;
	public GameObject gameOverPanel;
    public UILabel scoreLabel;
    public UISprite trap;
    public UISprite powerUp;

	#endregion

	#region Constructor
	void Awake()
	{
		guiInstance = this;
	}

	void Start()
	{
		_loadGUI = false;
	}
	#endregion
	
	#region Loop
	void Update()
	{
        if (_loadGUI)
        {
            UpdateScore();
            UpdateTrapNum();
            UpdatePowerUp();
        }
	}

//	void OnGUI()
//	{
//		if(_loadGUI == true)
//		{
//			if(GameDirector.gameInstance.characterLoaded == true)
//			{
//				GUI.Label(new Rect(10, 10, 150, 50), "Powerup: " + PowerupManager.powerupManagerInstance.availablePowerupType.ToString());
//
//				GUI.Label(new Rect(10, 60, 150, 50), "Available Traps: " + TrapManager.trapManagerInstance.availableTrapCount);
//
//				GUI.Label(new Rect(10, 110, 150, 50), "Score: " + ScoringDirector.scoringInstance.gameScore);
//			}
//		}
//	}

	#endregion

	#region Methods
	public void InitializeGUI()
	{
		_loadGUI = true;
	}

	public void ShowStartPanel()
	{
		startPanel.SetActive(true);
		inGamePanel.SetActive(false);
		gameOverPanel.SetActive(false);
	}

	public void ShowInGamePanel()
	{
		startPanel.SetActive(false);
		inGamePanel.SetActive(true);
		gameOverPanel.SetActive(false);
	}

	public void ShowGameOverPanel()
	{
		startPanel.SetActive(false);
		inGamePanel.SetActive(false);
		gameOverPanel.SetActive(true);
	}

    void UpdateScore()
    {
        scoreLabel.text = ScoringDirector.scoringInstance.gameScore.ToString();
    }

    void UpdateTrapNum()
    {
        trap.spriteName = "trap" + TrapManager.trapManagerInstance.availableTrapCount.ToString();
    }

    void UpdatePowerUp()
    {
        powerUp.spriteName = PowerupManager.powerupManagerInstance.availablePowerupType.ToString();
    }

	#endregion
}
