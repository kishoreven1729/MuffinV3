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
	#endregion
	
	#region Constructor
	public ChocoRushState(StateManager stateManager, Transform character) : base(stateManager, character, "ChocoRush", "ChocoRush")
	{
		_movementSpeedFactor = 1.2f;

		_rushMovementSpeed = StateHelpers.characterMovementSpeed * _movementSpeedFactor;

		_impactDuration = 5.0f;

		_impactTimer = 0.0f;
	}
	#endregion
	
	#region Override Methods
	public override void OnStateEnter ()
	{
		base.OnStateEnter ();
		
		_characterAnimator.SetTrigger(animationTriggerString);

		_impactTimer = Time.time + _impactDuration;	
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

			_stateManager.SwitchToState("Idle");
		}
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
