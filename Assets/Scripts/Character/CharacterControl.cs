#region References
using UnityEngine;
using System.Collections;
#endregion

public class CharacterControl : MonoBehaviour 
{
	#region Enum
	public enum CharacterState
	{
		Idle,
		Walk,
		Trap,
		Powerup,
		Die
	}
	#endregion

	#region Private Variables
	private float						_characterTurningStrength;
	private float						_characterMovementThreshold;
	private Vector3						_characterMovementDirection;
	private bool						_canCharacterMove;

	private Animator					_characterAnimator;
	private CharacterState				_currentCharacterState;

	private bool						_isResumed;
	private bool						_requestForDeathAnimation;

	private Transform					_explosionSphere;
	#endregion

	#region Public Variables
	public float						characterMovementSpeed;
	#endregion

	#region Constructor
	void Start() 
	{
		//characterMovementSpeed = 8.0f;
		_characterTurningStrength = 1000.0f;

		_characterMovementThreshold = 0.1f;

		_characterMovementDirection = Vector3.zero;

		try
		{
			_characterAnimator = transform.FindChild("AssetMuffin").GetComponent<Animator>();

			_explosionSphere = transform.FindChild("SpinBlast") as Transform;
		}
		catch (System.Exception ex)
		{
			Debug.Log("CharacterControl-Start: \n" + ex.Message);
		}

		_currentCharacterState = CharacterState.Idle;

		_canCharacterMove = true;

		_isResumed = false;
		_requestForDeathAnimation = false;

		CameraAnimate.CameraAnimationEnded += new CameraAnimate.CameraAnimationEndedHandler(OnCameraAnimationEnded);
	}
	#endregion
	
	#region Loop
	void Update() 
	{	
		if(_isResumed == true)
		{
			_characterMovementDirection = Vector3.zero;

			CharacterState newCharacterState = _currentCharacterState;

			if(_canCharacterMove == true)
			{
#if UNITY_ANDROID || UNITY_IOS
				_characterMovementDirection = new Vector3 (Input.acceleration.x, 0, Input.acceleration.y);

				if(Vector3.Distance(Vector3.zero, _characterMovementDirection) > _characterMovementThreshold)
				{
					newCharacterState = CharacterState.Walk;
				}
				else
				{
					newCharacterState = CharacterState.Idle;
				}

				if(Input.GetButtonDown("Fire1"))
				{
					Vector2 touchPosition = Input.GetTouch(0).position;

					if(touchPosition.x < Screen.width / 2)
					{
						newCharacterState = CharacterState.Trap;
						DropTrap();
					}
					else
					{
						newCharacterState = CharacterState.Powerup;
						ApplyPowerup();
					}
				}
#else 
				if(Input.GetKey(KeyCode.A))
				{
					_characterMovementDirection.x = -1.0f;
				}
				if(Input.GetKey(KeyCode.D))
				{
					_characterMovementDirection.x = 1.0f;
				}
				if(Input.GetKey(KeyCode.W))
				{
					_characterMovementDirection.z = 1.0f;;
				}
				if(Input.GetKey(KeyCode.S))
				{
					_characterMovementDirection.z = -1.0f;
				}

				if(Vector3.Distance(Vector3.zero, _characterMovementDirection) > _characterMovementThreshold)
				{
					newCharacterState = CharacterState.Walk;
				}
				else
				{
					newCharacterState = CharacterState.Idle;
				}

				if(Input.GetKeyDown(KeyCode.Space))
				{
					newCharacterState = CharacterState.Trap;
					DropTrap();
				}
				if(Input.GetKeyDown(KeyCode.E))
				{
					newCharacterState = CharacterState.Powerup;
					ApplyPowerup();
				}
#endif
			}

			if(newCharacterState == CharacterState.Walk)
			{
				WalkCharacter();
			}

			if(newCharacterState != _currentCharacterState)
			{
				_currentCharacterState = newCharacterState;

				switch(_currentCharacterState)
				{
				case CharacterState.Idle:
					IdleCharacter();
					break;
				case CharacterState.Walk:
					break;
				}

				StartCoroutine("AnimateCharacter");
			}

		}
		else
		{
			if(_currentCharacterState != CharacterState.Die)
			{
				IdleCharacter();
			}
		}

		if(_requestForDeathAnimation == true)
		{
			StartCoroutine("AnimateCharacterDeath");

			_requestForDeathAnimation = false;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.CompareTag("Enemy"))
		{
			KillCharacter();
		}
	}

	void OnDrawGizmos()
	{
		Debug.DrawRay(transform.position, transform.forward * 1000, Color.red);
	}
	#endregion

	#region Methods
	public void IdleCharacter()
	{
		rigidbody.Sleep();
	}

	public void WalkCharacter()
	{
		_characterMovementDirection.Normalize();

		rigidbody.velocity = _characterMovementDirection * characterMovementSpeed;

		transform.LookAt(_characterMovementDirection * _characterTurningStrength);
	}

	public void DropTrap()
	{
		rigidbody.Sleep();

		_canCharacterMove = false;

		_currentCharacterState = CharacterState.Trap;

		StartCoroutine("AnimateCharacter");
	}

	public void ApplyPowerup()
	{
		if(PowerupManager.powerupManagerInstance.availablePowerupType != PowerupManager.PowerupType.None)
		{
			rigidbody.Sleep();

			_canCharacterMove = false;

			_currentCharacterState = CharacterState.Powerup;

			StartCoroutine("AnimatePowerup");
		}
	}

	public void KillCharacter()
	{
		rigidbody.Sleep();

		_currentCharacterState = CharacterState.Die;

		_canCharacterMove = false;

		_isResumed = false;

		EnemySpawnManager.enemySpawnManagerInstance.PauseSpawning();

		PowerupManager.powerupManagerInstance.RemoveAllPowerups();

		GameDirector.gameInstance.gameCamera.SendMessage("AnimateToDeath", SendMessageOptions.DontRequireReceiver);
	}

	public IEnumerator AnimateCharacter()
	{
		if(_currentCharacterState != CharacterState.Powerup)
		{
			_characterAnimator.SetTrigger(_currentCharacterState.ToString());
		}

		if(_currentCharacterState == CharacterState.Trap)
		{
			float halfAnimationTime = _characterAnimator.GetCurrentAnimatorStateInfo(0).length / 3;

			yield return new WaitForSeconds(halfAnimationTime);

			TrapManager.trapManagerInstance.AddTrap();

			yield return new WaitForSeconds(halfAnimationTime);

			_canCharacterMove = true;
			_currentCharacterState = CharacterState.Idle;
		}
	}

	public IEnumerator AnimatePowerup()
	{
		PowerupManager.PowerupType powerupType = PowerupManager.powerupManagerInstance.availablePowerupType;

		_characterAnimator.SetTrigger(powerupType.ToString());

		float halfAnimationTime = _characterAnimator.GetCurrentAnimatorStateInfo(0).length / 3;

		yield return new WaitForSeconds(halfAnimationTime);

		switch(powerupType)
		{
		case PowerupManager.PowerupType.CranberrySpin:
			_explosionSphere.gameObject.SetActive(true);
			break;
		case PowerupManager.PowerupType.Crumbs:
			CrumbsManager.crumbsInstance.SprinkleCrumbs();
			break;
		}

		if(PowerupManager.powerupManagerInstance.availablePowerup != null)
		{
			PowerupManager.powerupManagerInstance.availablePowerup.SendMessage("UsePowerup", SendMessageOptions.DontRequireReceiver);
		}

		yield return new WaitForSeconds(halfAnimationTime);

		_canCharacterMove = true;
		_currentCharacterState = CharacterState.Idle;
	}

	public IEnumerator AnimateCharacterDeath()
	{
		_characterAnimator.SetTrigger("Die");

		float animationLength = _characterAnimator.GetCurrentAnimatorStateInfo(0).length;

		ScoringDirector.scoringInstance.PauseScoring();

		EnemySpawnManager.enemySpawnManagerInstance.KillAllEnemies();
		
		CrumbsManager.crumbsInstance.DestroyAllCrumbs();
		
		TrapManager.trapManagerInstance.DestroyAllTraps();
		
		PowerupManager.powerupManagerInstance.RemoveAllPowerups();

		yield return new WaitForSeconds(animationLength);

		GameDirector.gameInstance.KillCharacter();
	}

	public bool ToggleCharacterMovement()
	{
		_isResumed = !_isResumed;

		if(_explosionSphere.gameObject.activeSelf == true)
		{
			if(_isResumed == false)
			{
				_explosionSphere.SendMessage("PauseExplosion", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				_explosionSphere.SendMessage("ResumeExplosion", SendMessageOptions.DontRequireReceiver);
			}
		}

		return _isResumed;
	}
	#endregion

	#region Event Handlers
	private void OnCameraAnimationEnded(bool isGameStartAnimation)
	{
		if(isGameStartAnimation == true)
		{
			_isResumed = true;
		}
		else
		{
			/*if(_currentCharacterState == CharacterState.Die)
			{*/
				_requestForDeathAnimation = true;
			//}
		}
	}
	#endregion
}