#region References
using UnityEngine;
using System.Collections;
#endregion

public class GameDirector : MonoBehaviour 
{
	#region Private Variables
	private Vector3				_initialCharacterPosition;
	private Quaternion			_initialCharacterRotation;
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

		SpawnCharacter();
	}
	#endregion
	
	#region Loop
	void Update()
	{	
		if(Input.GetKeyDown(KeyCode.J))
		{
			SpawnCharacter();
		}
	}
	#endregion

	#region Methods
	public void SpawnCharacter()
	{
		try
		{
			character = Instantiate(characterPrefab, _initialCharacterPosition, _initialCharacterRotation) as Transform;
			character.name = "Character";

			characterDropLocation = character.GetChild(1) as Transform;

			gameCamera.SendMessage("AnimateToStart", SendMessageOptions.DontRequireReceiver);
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
	#endregion

	#region Event Handlers
	private void OnStartAnimationEnded(bool isGameStartAnimation)
	{
		if(isGameStartAnimation == true)
		{
			characterLoaded = true;
		}
	}
	#endregion
}
