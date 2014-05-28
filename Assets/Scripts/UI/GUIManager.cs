#region References
using UnityEngine;
using System.Collections;
#endregion

public class GUIManager : MonoBehaviour 
{
	#region Private Variables
	private bool _loadGUI;
    private UIRoot _root;
	#endregion

	#region Public Variables
	public static GUIManager guiInstance;

    /// <summary>
    /// The start panel.
    /// </summary>
	public GameObject startPanel;
    /// <summary>
    /// The in game panel.
    /// </summary>
	public GameObject inGamePanel;
	public GameObject gameOverPanel;
    public GameObject gamePausedPanel;
    public GameObject leaderboardPanel;
    public UILabel scoreLabel;
    public UISprite trap;
    public UISprite powerUp;
    public UILabel powerUpLabel;
    public GameObject scoreItemPrefab;

    /// <summary>
    /// Sets the height of the user interface.
    /// </summary>
    /// <value>The height of the user interface.</value>
    public int UIHeight
    {
        set
        {
            _root.manualHeight = value;
        }
    }

	#endregion

	#region Constructor
	void Awake()
	{
		guiInstance = this;
	}

	void Start()
	{
		_loadGUI = false;
        _root = GetComponent<UIRoot>();
	}
	#endregion
	
	#region Loop
    /// <summary>
    /// Update this instance.
    /// </summary>
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
        gamePausedPanel.SetActive(false);
	}

	public void ShowInGamePanel()
	{
		startPanel.SetActive(false);
		inGamePanel.SetActive(true);
		gameOverPanel.SetActive(false);
        gamePausedPanel.SetActive(false);
	}

	public void ShowGameOverPanel()
	{
        StartCoroutine(_ShowGameOverPanel());
	}

    IEnumerator _ShowGameOverPanel()
    {
        yield return new WaitForEndOfFrame();
        startPanel.SetActive(false);
        inGamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        gamePausedPanel.SetActive(false);
    }

    public void ShowGamePausedPanel()
    {
        startPanel.SetActive(false);
        inGamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gamePausedPanel.SetActive(true);
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

    public void SetPowerUpLabel(string s)
    {
        StartCoroutine(_SetPowerUpLabel(s));
    }

    IEnumerator _SetPowerUpLabel(string s)
    {
        powerUpLabel.text = s;
        yield return new WaitForSeconds(1f);
        powerUpLabel.text = "";
    }

    /// <summary>
    /// Creates the high score items.
    /// </summary>
    void CreateHighScoreItems()
    {
        foreach( string n in FacebookManager.facebookInstance.friendsHighScore.Keys)
        {
            GameObject go = Instantiate(scoreItemPrefab, leaderboardPanel.transform.position, Quaternion.identity) as GameObject;
            go.GetComponent<ScoreItem>().SetScore(n, FacebookManager.facebookInstance.friendsHighScore[n]);
            go.transform.parent = leaderboardPanel.transform;
        }
    }
    /// <summary>
    /// Shows the leaderboard panel.
    /// </summary>
    public void ShowLeaderboardPanel()
    {
        leaderboardPanel.SetActive(true);
        CreateHighScoreItems();
    }
    /// <summary>
    /// Hides the leaderboard panel.
    /// </summary>
    public void HideLeaderboardPanel()
    {
        int count = leaderboardPanel.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Destroy(leaderboardPanel.transform.GetChild(i).gameObject);
        }

        leaderboardPanel.SetActive(false);
    }


	#endregion
}
