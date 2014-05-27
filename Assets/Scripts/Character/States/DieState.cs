#region References
using UnityEngine;
using System.Collections;
#endregion

public class DieState : State 
{
	#region Private Variables
	private bool		_isCameraAnimationEnded;
	#endregion
	
	#region Constructor
	public DieState(StateManager stateManager, Transform character) : base(stateManager, character, "Die", "Die")
	{
		_isCameraAnimationEnded = false;
	}
	#endregion
	
	#region Override Methods
	public override void OnStateEnter ()
	{
		base.OnStateEnter ();

		_character.rigidbody.Sleep();

		PowerupManager.powerupManagerInstance.RemoveAllPowerups();

		EnemySpawnManager.enemySpawnManagerInstance.PauseSpawning();

		EnemySpawnManager.enemySpawnManagerInstance.KillAllEnemies();

		ScoringDirector.scoringInstance.PauseScoring();

		GameDirector.gameInstance.gameCamera.SendMessage("AnimateToDeath", SendMessageOptions.DontRequireReceiver);
	}
	#endregion
	
	#region Methods
	public override void ReceiveAnimationEvent ()
	{
		base.ReceiveAnimationEvent ();

		if(_isCameraAnimationEnded == false)
		{
			_characterAnimator.SetTrigger(animationTriggerString);

			_isCameraAnimationEnded = true;
		}
		else
		{
			GameDirector.gameInstance.KillCharacter();

			GUIManager.guiInstance.ShowGameOverPanel();

			_isCameraAnimationEnded = false;

			_stateManager.SwitchToState("Idle");
		}
	}
	#endregion
}