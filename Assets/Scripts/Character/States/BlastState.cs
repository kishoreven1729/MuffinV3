#region References
using UnityEngine;
using System.Collections;
#endregion

public class BlastState : State 
{
	#region Private Variables
	private int				_eventCount;

	private Transform		_createdExplosion;
	
	private MuffinControl 	_muffinControl;

	private Transform		_spawnedParticle;
	#endregion
	
	#region Constructor
	public BlastState(StateManager stateManager, Transform character) : base(stateManager, character, "Blast", "Blast")
	{
		_muffinControl = _character.GetComponent<MuffinControl>();
	}
	#endregion
	
	#region Override Methods
	public override void OnStateEnter ()
	{
		base.OnStateEnter ();
		
		_character.rigidbody.Sleep();
		
		_characterAnimator.SetTrigger(animationTriggerString);

		_spawnedParticle = GameDirector.gameInstance.SpawnParticles("Blast");
	}

	public override void ReceiveAnimationEvent ()
	{
		base.ReceiveAnimationEvent ();
		
		_eventCount ++;
		
		if(_eventCount == 1)
		{
			EnemySpawnManager.enemySpawnManagerInstance.BlastEnemies();
		}
		else if(_eventCount == 2)
		{
			_eventCount = 0;
			
			_stateManager.SwitchToState("Idle");
		}
	}

	public override void OnStateExit ()
	{
		base.OnStateExit ();
		
		GameDirector.gameInstance.DestroyParticles(_spawnedParticle);
	}
	#endregion
	
	#region Methods
	#endregion
}
