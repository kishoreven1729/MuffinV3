#region References
using UnityEngine;
using System.Collections;
#endregion

public class ChocoRushState : State 
{
	#region Private Variables
	private Vector3 _characterMovementDirection;

	private float	_movementSpeedFactor;
	private float	_rushMovementSpeed;

	private float	_impactDuration;
	private float	_impactTimer;

	private SphereCollider	_characterCollider;
	private	float			_defaultColliderRadius;

	private Transform		_spawnedParticle;
	#endregion
	
	#region Constructor
	public ChocoRushState(StateManager stateManager, Transform character) : base(stateManager, character, "ChocoRush", "ChocoRush")
	{
		_movementSpeedFactor = 1.2f;

		_rushMovementSpeed = StateHelpers.characterMovementSpeed * _movementSpeedFactor;

		_impactDuration = 5.0f;

		_impactTimer = 0.0f;

		_characterCollider = _character.collider as SphereCollider;

		_defaultColliderRadius = _characterCollider.radius;
	}
	#endregion
	
	#region Override Methods
	public override void OnStateEnter ()
	{
		base.OnStateEnter ();
		
		_characterAnimator.SetTrigger(animationTriggerString);

		_impactTimer = Time.time + _impactDuration;	

		_characterCollider.radius *= 2.0f;

		_spawnedParticle = GameDirector.gameInstance.SpawnParticles("Choco");

		_spawnedParticle.parent = _character;

		_spawnedParticle.localRotation = Quaternion.LookRotation(-1 * Vector3.forward);
	}

	public override void UpdateFunction ()
	{
		base.UpdateFunction ();

		Vector3 movementDirection = StateHelpers.MovementInput();
			
		if(movementDirection.magnitude > 0.0f)
		{
			_characterMovementDirection = movementDirection;

			WalkCharacter();
		}

		if(Time.time > _impactTimer)
		{
			_impactTimer = 0.0f;

			_characterCollider.radius = _defaultColliderRadius;

			_stateManager.SwitchToState("Idle");
		}
	}

	public override void OnStateExit ()
	{
		base.OnStateExit ();

		GameDirector.gameInstance.DestroyParticles(_spawnedParticle);
	}

	public override void CollisionFunction (Collider collidee)
	{
		base.CollisionFunction (collidee);

		StateHelpers.EnemyCollisionCheck(collidee, true);
	}
	#endregion
	
	#region Methods
	public void WalkCharacter()
	{
		_characterMovementDirection.Normalize();
		
		_character.rigidbody.velocity = _characterMovementDirection * _rushMovementSpeed;
		
		_character.transform.LookAt(_characterMovementDirection * StateHelpers.characterTurningStrength);
	}
	#endregion
}
