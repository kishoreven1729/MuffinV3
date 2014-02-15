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
		Die
	}
	#endregion

	#region Private Variables
	private float						_characterMovementSpeed;
	private float						_characterTurningStrength;
	private float						_characterMovementThreshold;
	private Vector3						_characterMovementDirection;
	private bool						_canCharacterMove;

	private Animator					_characterAnimator;
	private CharacterState				_currentCharacterState;

	private bool						_isResumed;
	private bool						_requestForDeathAnimation;

	private PowerupManager.PowerupType	_characterPowerupState;
	#endregion

	#region Public Variables
	#endregion

	#region Constructor
	void Start() 
	{
		_characterMovementSpeed = 5.0f;
		_characterTurningStrength = 1000.0f;

		_characterMovementThreshold = 0.2f;

		_characterMovementDirection = Vector3.zero;

		try
		{
			_characterAnimator = transform.GetChild(0).GetComponent<Animator>();
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
				/*_characterMovementDirection = new Vector3 (Input.acceleration.x, 0, Input.acceleration.y);*/
				_characterMovementDirection = Vector3.zero;

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

				if(Input.GetKeyDown(KeyCode.E))
				{
					newCharacterState = CharacterState.Trap;
				}
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
					WalkCharacter();
					break;
				case CharacterState.Trap:
					DropTrap();
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
		rigidbody.velocity = _characterMovementDirection * _characterMovementSpeed;

		transform.LookAt(_characterMovementDirection * _characterTurningStrength);
	}

	public void DropTrap()
	{
		rigidbody.Sleep();

		_canCharacterMove = false;
	}

	public void KillCharacter()
	{
		rigidbody.Sleep();

		_currentCharacterState = CharacterState.Die;

		_canCharacterMove = false;

		_isResumed = false;

		EnemySpawnManager.enemySpawnManagerInstance.PauseSpawning();

		GameDirector.gameInstance.gameCamera.SendMessage("AnimateToDeath", SendMessageOptions.DontRequireReceiver);
	}

	public IEnumerator AnimateCharacter()
	{
		_characterAnimator.SetTrigger(_currentCharacterState.ToString());

		if(_currentCharacterState == CharacterState.Trap)
		{
			yield return new WaitForSeconds(_characterAnimator.GetCurrentAnimatorStateInfo(0).length);

			_canCharacterMove = true;
			_currentCharacterState = CharacterState.Idle;
		}
	}

	public IEnumerator AnimateCharacterDeath()
	{
		_characterAnimator.SetTrigger("Die");

		float animationLength = _characterAnimator.GetCurrentAnimatorStateInfo(0).length;

		yield return new WaitForSeconds(animationLength);

		EnemySpawnManager.enemySpawnManagerInstance.KillAllEnemies();

		GameDirector.gameInstance.KillCharacter();
	}

	public bool ToggleCharacterMovement()
	{
		_isResumed = !_isResumed;

		return _isResumed;
	}

	private void ApplyPowerup(PowerupManager.PowerupType powerupType)
	{
		_characterPowerupState = powerupType;

		switch(powerupType)
		{
		case PowerupManager.PowerupType.A:
			break;
		case PowerupManager.PowerupType.B:
			break;
		case PowerupManager.PowerupType.C:
			break;
		case PowerupManager.PowerupType.D:
			break;
		case PowerupManager.PowerupType.E:
			break;
		}
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
			if(_currentCharacterState == CharacterState.Die)
			{
				_requestForDeathAnimation = true;
			}
		}
	}
	#endregion
}
