#region References
using UnityEngine;
using System.Collections;
#endregion

public class CameraFollow : MonoBehaviour 
{
	#region Private Variables
	private Vector3			_initialCameraPosition;
	private Quaternion		_initialCameraRotation;

	private Vector3			_zoomedCameraPosition;
	private Quaternion		_zoomedCameraRotation;

	private Rect			_cameraMovementBoundary;
	private Transform		_character;
	private Vector3			_characterCameraOffset;

	private bool			_hasCharacterLoaded;
	private bool			_isCameraLoadDone;
	#endregion

	#region Public Variables
	#endregion

	#region Constructor
	void Start() 
	{
		_hasCharacterLoaded = false;
		_isCameraLoadDone = false;

		_initialCameraPosition = transform.position;
		_initialCameraRotation = transform.rotation;

		_zoomedCameraPosition = new Vector3(0.0f, 5.0f, -70.0f);
		_zoomedCameraRotation = Quaternion.identity;

		try
		{
			Vector3 bottomRight = GameObject.Find("CameraBoundaryBottomRight").transform.position;
			Vector3 topLeft = GameObject.Find("CameraBoundaryTopLeft").transform.position;

			_cameraMovementBoundary = new Rect(topLeft.x, bottomRight.z, Mathf.Abs(bottomRight.x - topLeft.x), Mathf.Abs(topLeft.z - bottomRight.z));
		}
		catch (System.Exception ex)
		{
			Debug.Log("CameraFollow-Start: \n" + ex.Message); 
		}
	}

	void InitializeCamera()
	{
		_character = GameDirector.gameInstance.character;

		transform.position = _initialCameraPosition;
		transform.rotation = _initialCameraRotation;

		_characterCameraOffset = new Vector3(transform.position.x - _character.position.x, 0, transform.position.z - _character.position.z);
	}
	#endregion
	
	#region Loop
	void Update() 
	{
		if(_hasCharacterLoaded == true)
		{
			_hasCharacterLoaded = GameDirector.gameInstance.characterLoaded;

			if(_character != null)
			{
				Vector3 cameraPosition = new Vector3(_character.position.x + _characterCameraOffset.x, transform.position.y, _character.position.z + _characterCameraOffset.z);;

				if(cameraPosition.x < _cameraMovementBoundary.xMin)
				{
					cameraPosition.x = _cameraMovementBoundary.xMin;
				}
				else if(cameraPosition.x > _cameraMovementBoundary.xMax)
				{
					cameraPosition.x = _cameraMovementBoundary.xMax;
				}

				if(cameraPosition.z < _cameraMovementBoundary.yMin)
				{
					cameraPosition.z = _cameraMovementBoundary.yMin;
				}
				else if(cameraPosition.z > _cameraMovementBoundary.yMax)
				{
					cameraPosition.z = _cameraMovementBoundary.yMax;
				}

				transform.position = cameraPosition;
			}
		}
		else
		{
			if(_isCameraLoadDone == false)
			{
				transform.position = _zoomedCameraPosition;
				transform.rotation = _zoomedCameraRotation;
				
				_isCameraLoadDone = true;
			}
			else if(GameDirector.gameInstance.characterLoaded == true)
			{
				InitializeCamera();

				/*Start the Spawn Manager*/
				EnemySpawnManager.enemySpawnManagerInstance.ResumeSpawning();

				/*Allow Camera Movement*/
				_hasCharacterLoaded = GameDirector.gameInstance.characterLoaded;
			}
		}
	}
	#endregion

	#region Methods
	public void AnimateToStart()
	{
		CameraAnimate.cameraAnimationInstance.PlayCameraAnimation(_initialCameraPosition, _initialCameraRotation, 10.0f, true);
	}

	public void AnimateToDeath()
	{
		_hasCharacterLoaded = false;

		GameDirector.gameInstance.characterLoaded = false;

		CameraAnimate.cameraAnimationInstance.PlayCameraAnimation(_zoomedCameraPosition, _zoomedCameraRotation, 2.0f, false);
	}
	#endregion
}
