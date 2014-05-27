#region References
using UnityEngine;
using System.Collections;
#endregion

public class TrapState : State 
{
	#region Private Variables
	private bool	_trapDropped;
	#endregion
	
	#region Constructor
	public TrapState(StateManager stateManager, Transform character) : base(stateManager, character, "Trap", "Trap")
	{
		_trapDropped = false;
	}
	#endregion
	
	#region Override Methods
	public override void OnStateEnter()
	{
		base.OnStateEnter();
		
		_character.rigidbody.Sleep();
		
		_characterAnimator.SetTrigger(animationTriggerString);
	}
	
	public override void CollisionFunction (Collider collidee)
	{
		base.CollisionFunction (collidee);
		
		if(StateHelpers.EnemyCollisionCheck(collidee) == true)
		{
			_stateManager.SwitchToState("Die");
		}
	}

	public override void ReceiveAnimationEvent()
	{
		if(_trapDropped == false)
		{
			TrapManager.trapManagerInstance.AddTrap();
			_trapDropped = true;
		}
		else
		{
			_trapDropped = false;
			_stateManager.SwitchToState("Idle");
		}
	}
	#endregion
}
