#region References
using UnityEngine;
using System.Collections;
#endregion

public class BlastState : State 
{
	#region Private Variables
	private int				_eventCount;
	private Transform		_explosionPrefab;
	
	private Transform		_createdExplosion;
	
	private MuffinControl 	_muffinControl;
	#endregion
	
	#region Constructor
	public BlastState(StateManager stateManager, Transform character) : base(stateManager, character, "Blast", "Blast")
	{
		_explosionPrefab = Resources.Load<Transform>("DynamicPrefabs/ExplosionWave");
		
		_muffinControl = _character.GetComponent<MuffinControl>();
	}
	#endregion
	
	#region Override Methods
	public override void OnStateEnter ()
	{
		base.OnStateEnter ();
		
		_character.rigidbody.Sleep();
		
		_characterAnimator.SetTrigger(animationTriggerString);
	}

	public override void ReceiveAnimationEvent ()
	{
		base.ReceiveAnimationEvent ();
		
		_eventCount ++;
		
		if(_eventCount == 1)
		{
			_createdExplosion = _muffinControl.CreateEplosionWave(_explosionPrefab);
		}
		else if(_eventCount == 2)
		{
			_eventCount = 0;
			
			_muffinControl.DestroyExplosionWave(_createdExplosion);
			
			_stateManager.SwitchToState("Idle");
		}
	}
	#endregion
	
	#region Methods
	#endregion
}
