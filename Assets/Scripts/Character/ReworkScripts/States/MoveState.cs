#region References
using UnityEngine;
using System.Collections;
#endregion

public class MoveState : State 
{
	#region Private Variables
	private Vector3 _characterMovementDirection;
	#endregion
	
	#region Constructor
	public MoveState(StateManager stateManager, Transform character) : base(stateManager, character, "Move", "Move")
	{
		
	}
	#endregion
	
	#region Override Methods
	public override void OnStateEnter ()
	{
		base.OnStateEnter ();

		_characterAnimator.SetTrigger(animationTriggerString);

		if(_stateTransitionData != null)
		{
			Vector3 movementDirection = (Vector3)PullTransitionData();

			_characterMovementDirection = movementDirection;

			WalkCharacter();
		}
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
		else
		{
			_stateManager.SwitchToState("Idle");
		}

		if(StateHelpers.TrapInput() == true)
		{
			_stateManager.SwitchToState("Trap");
		}
		
		if(StateHelpers.PowerupInput() == true)
		{
			switch(PowerupManager.powerupManagerInstance.availablePowerupType)
			{
			case PowerupManager.PowerupType.CranberrySpin:
				_stateManager.SwitchToState("Spin");
				break;
			case PowerupManager.PowerupType.HoneyBlast:
				_stateManager.SwitchToState("Blast");
				break;
			case PowerupManager.PowerupType.ChocoRush:
				_stateManager.SwitchToState("ChocoRush");
				break;
			}
		}
	}

	public override void CollisionFunction (Collider collidee)
	{
		base.CollisionFunction (collidee);
		
		if(StateHelpers.EnemyCollisionCheck(collidee) == true)
		{
			_stateManager.SwitchToState("Die");
		}
	}
	#endregion
	
	#region Methods	 
	public void WalkCharacter()
	{
		_characterMovementDirection.Normalize();
		
		_character.rigidbody.velocity = _characterMovementDirection * StateHelpers.characterMovementSpeed;
		
		_character.transform.LookAt(_characterMovementDirection * StateHelpers.characterTurningStrength);
	}
	#endregion
}
