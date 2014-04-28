#region References
using UnityEngine;
using System.Collections;
#endregion

public class IdleState : State 
{
	#region Private Variables
	#endregion

	#region Constructor
	public IdleState(StateManager stateManager, Transform character) : base(stateManager, character, "Idle", "Idle")
	{

	}
	#endregion

	#region Override Methods
	public override void OnStateEnter ()
	{
		base.OnStateEnter();

		_character.rigidbody.Sleep();

		_characterAnimator.SetTrigger(animationTriggerString);
	}

	public override void UpdateFunction ()
	{
		base.UpdateFunction ();

		Vector3 movementDirection = StateHelpers.MovementInput();

		if(movementDirection.magnitude > 0.0f)
		{
			_stateManager.PushTransitionData("Move", movementDirection);

			_stateManager.SwitchToState("Move");
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
	#endregion
}
