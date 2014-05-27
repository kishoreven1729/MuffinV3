#region References
using UnityEngine;
using System.Collections;
#endregion

public class PauseState : State 
{
	#region Private Variables
	#endregion
	
	#region Constructor
	public PauseState(StateManager stateManager, Transform character) : base(stateManager, character, "Pause", "Idle")
	{
		
	}
	#endregion
	
	#region Override Methods
	public override void OnStateEnter ()
	{
		base.OnStateEnter();
		
		object incomingState = PullTransitionData();
		
		if(incomingState != null)
		{
			string stateString = (string)incomingState;
			
			if(stateString == "Move")
			{
				_character.rigidbody.Sleep();

				_characterAnimator.SetTrigger(animationTriggerString);
			}
			else
			{
				PushTransitionData(incomingState);
			}
		}
	}

	public override void ReceiveAnimationEvent ()
	{
		base.ReceiveAnimationEvent ();

		object incomingState = PullTransitionData();

		if(incomingState != null)
		{
			string stateString = (string)incomingState;

			_stateManager.SwitchToState(stateString);
		}
		else
		{
			_stateManager.SwitchToState("Idle");
		}

		EnemySpawnManager.enemySpawnManagerInstance.ResumeSpawning();
		ScoringDirector.scoringInstance.ResumeScoring();
	}
	#endregion
	
	#region Methods
	#endregion
}

