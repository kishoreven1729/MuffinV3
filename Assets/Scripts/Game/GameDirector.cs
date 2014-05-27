#region References
using UnityEngine;
using System.Collections;
#endregion

public class GameDirector : MonoBehaviour 
{
	#region Private Variables
	private Vector3				_initialCharacterPosition;
	private Quaternion			_initialCharacterRotation;

	private float				_defaultTimeScale;
	#endregion

	#region Public Variables
	public static GameDirector 	gameInstance;

	public bool					characterLoaded;
	public Transform			character;

	public Transform			characterDropLocation;

	public Transform			gameCamera;

	public Transform			currentPowerup;
	#endregion

	#region Prefab Variables
	public Transform			characterPrefab;

	public Transform			cranberrySpinParticles;
	public Transform			honeyBlastParticles;
	public Transform			chocoParticles;
	#endregion

	#region Constructor
	void Awake()
	{
		gameInstance = this;
	}

	void Start() 
	{
		CameraAnimate.CameraAnimationEnded += new CameraAnimate.CameraAnimationEndedHandler(OnStartAnimationEnded);

		try
		{
			gameCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
		}
		catch(System.Exception ex)
		{
			Debug.Log("GameDirector-Start: \n" + ex.Message);
		}

		characterLoaded = false;

		_initialCharacterPosition = new Vector3(-1.5f, 0.0f, -2.2f);
		_initialCharacterRotation = Quaternion.AngleAxis(-180.0f, Vector3.up);

		_defaultTimeScale = Time.timeScale;

//		ResetGame();
		GUIManager.guiInstance.ShowStartPanel();
	}
	#endregion
	
	#region Loop
	void Update()
	{	
		if(Input.GetKeyDown(KeyCode.J))
		{
			ResetGame();
		}
		if(Input.GetKeyDown(KeyCode.P))
		{
			PauseGame();
		}
		if(Input.GetKeyDown(KeyCode.O))
		{
			ResumeGame();
		}
	}
	#endregion

	#region Methods
	public void SpawnCharacter()
	{
		try
		{
			if(character != null)
			{
				KillCharacter();
			}

			character = Instantiate(characterPrefab, _initialCharacterPosition, _initialCharacterRotation) as Transform;
			character.name = "Character";

			characterDropLocation = character.FindChild("DropLocation") as Transform;

			gameCamera.SendMessage("AnimateToStart", SendMessageOptions.DontRequireReceiver);

			FacebookManager.facebookInstance.FetchScore();
		}
		catch(System.Exception ex)
		{
			Debug.Log("GameDirector-SpawnCharacter: \n" + ex.Message);
		}
	}

	public void KillCharacter()
	{
		try
		{
			Destroy(character.gameObject);
		}
		catch (System.Exception ex)
		{
			Debug.Log("GameDirector-KillCharacter: \n" + ex.Message);
		}
	}

	public void ResetGame()
	{
		SpawnCharacter();

		EnemySpawnManager.enemySpawnManagerInstance.ResetSpawnManager();

		PowerupManager.powerupManagerInstance.ResetPowerups();

		TrapManager.trapManagerInstance.ResetTrapManager();

		ScoringDirector.scoringInstance.ResetScoring();
	}

	public void PauseGame()
	{
        print("GameDirector : Game Paused");

		Time.timeScale = 0.0f;

		character.SendMessage("PauseGame", SendMessageOptions.DontRequireReceiver);

		EnemySpawnManager.enemySpawnManagerInstance.PauseSpawning();
		
		TrapManager.trapManagerInstance.PauseTrapTimers();
		
		ScoringDirector.scoringInstance.PauseScoring();
	}

	public void ResumeGame()
	{
		print("GameDirector : Game Resumed");

		Time.timeScale = _defaultTimeScale;

		character.SendMessage("ResumeGame", SendMessageOptions.DontRequireReceiver);
		
		TrapManager.trapManagerInstance.ResumeTrapTimers();
	}                  
	#endregion

	#region Event Handlers
	private void OnStartAnimationEnded(bool isGameStartAnimation)
	{
		if(isGameStartAnimation == true)
		{
			characterLoaded = true;
			GUIManager.guiInstance.ShowInGamePanel();
		}
		else
		{
			//GUIManager.guiInstance.ShowGameOverPanel();
		}
	}
	#endregion

	#region Particle Methods
	public Transform SpawnParticles(string particleType)
	{
		Transform spawnedTransform = null;

		if(particleType == "Choco")
		{
			spawnedTransform = Instantiate(chocoParticles, character.position, Quaternion.AngleAxis(90.0f, Vector3.up)) as Transform;
		}
		else if(particleType == "Spin")
		{
			spawnedTransform = Instantiate(cranberrySpinParticles, character.position, Quaternion.identity) as Transform;
		}
		else if(particleType == "Blast")
		{
			spawnedTransform = Instantiate(honeyBlastParticles, character.position, Quaternion.AngleAxis(-90.0f, Vector3.right)) as Transform;
		}

		return spawnedTransform;
	}

	public void DestroyParticles(Transform particle)
	{
		Destroy(particle.gameObject);
	}
	#endregion
}
