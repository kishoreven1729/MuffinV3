#region References
using UnityEngine;
using System.Collections;
#endregion

public class SpinState : State 
{
	#region Private Variables
	private int				_eventCount;
	private float			_defaultSphereRadius;
	private SphereCollider 	_characterCollider;

	private float			_targetSphereRadius;

	private Transform		_createdExplosion;

	private MuffinControl 	_muffinControl;

	private Transform		_spawnedParticle;
	#endregion
	
	#region Constructor
	public SpinState(StateManager stateManager, Transform character) : base(stateManager, character, "Spin", "Spin")
	{
		_eventCount = 0;

		_characterCollider = _character.collider as SphereCollider;

		_defaultSphereRadius = _characterCollider.radius;

		_targetSphereRadius = 20.0f;

		_muffinControl = _character.GetComponent<MuffinControl>();
	}
	#endregion
	
	#region Override Methods
	public override void OnStateEnter ()
	{
		base.OnStateEnter ();
		
		_character.rigidbody.Sleep();
		
		_characterAnimator.SetTrigger(animationTriggerString);

		_spawnedParticle = GameDirector.gameInstance.SpawnParticles("Spin");
	}

	public override void UpdateFunction()
	{
		if(_spawnedParticle != null)
		{
			_spawnedParticle.Rotate(Vector3.up, Time.deltaTime * 1000.0f, Space.World);
		}

		if(_eventCount == 1)
		{
			if(_characterCollider.radius < _targetSphereRadius)
			{
				_characterCollider.radius += 0.5f;
			}
		}
	}

	public override void CollisionFunction (Collider collidee)
	{
		base.CollisionFunction (collidee);

		StateHelpers.EnemyCollisionCheck(collidee, true);
	}

	public override void ReceiveAnimationEvent ()
	{
		base.ReceiveAnimationEvent ();

		_eventCount ++;

		if(_eventCount == 1)
		{

		}
		else if(_eventCount == 2)
		{
			_eventCount = 0;

			_characterCollider.radius = _defaultSphereRadius;

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
