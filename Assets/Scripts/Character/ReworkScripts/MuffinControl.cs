﻿#region References
using UnityEngine;
using System.Collections;
#endregion

public class MuffinControl : MonoBehaviour 
{
	#region Private Variables
	private Animator						_muffinAnimator;
	private StateManager					_stateManager;

	private float							_characterTurningStrength;
	private float							_characterMovementThreshold;
	private Vector3							_characterMovementDirection;
	#endregion

	#region Public Variables
	#endregion

	#region Constructor
	void Start() 
	{
		_muffinAnimator = transform.FindChild("Asset").GetComponent<Animator>();

		_characterTurningStrength = 1000.0f;		
		_characterMovementThreshold = 0.1f;		
		_characterMovementDirection = Vector3.zero;

		InitialzeStateManager();

		CameraAnimate.CameraAnimationEnded += new CameraAnimate.CameraAnimationEndedHandler(OnCameraAnimationEnded);
	}

	private void InitialzeStateManager()
	{
		_stateManager = new StateManager(_muffinAnimator);

		IdleState 		idle = new IdleState(_stateManager, transform);
		MoveState 		move = new MoveState(_stateManager, transform);
		TrapState 		trap = new TrapState(_stateManager, transform);
		DieState 		die	= new DieState(_stateManager, transform); 
		SpinState		spin = new SpinState(_stateManager, transform);
		BlastState		blast = new BlastState(_stateManager, transform);
		ChocoRushState	chocoRush = new ChocoRushState(_stateManager, transform);

		PauseState		pause = new PauseState(_stateManager, transform);

		_stateManager.AddCharacterState(idle);
		_stateManager.AddCharacterState(move);
		_stateManager.AddCharacterState(trap);
		_stateManager.AddCharacterState(die);
		_stateManager.AddCharacterState(spin);
		_stateManager.AddCharacterState(blast);
		_stateManager.AddCharacterState(chocoRush);

		_stateManager.AddCharacterState(pause);

		_stateManager.SetDefaultState("Pause");
	}
	#endregion
	
	#region Loop
	void Update() 
	{
		_stateManager.currentCharacterState.UpdateFunction();
	}

	void OnTriggerEnter(Collider collidee)
	{
		_stateManager.currentCharacterState.CollisionFunction(collidee);
	}

	void OnTriggerStay(Collider collidee)
	{
		_stateManager.currentCharacterState.CollisionFunction(collidee);
	}
	#endregion

	#region Methods
	public Transform CreateEplosionWave(Transform explosionPrefab)
	{
		Quaternion rotateAboutX = Quaternion.AngleAxis(90.0f, Vector3.right);
		Vector3 position = transform.position; 
		position.y = 0.5f;

		Transform explosionWave = Instantiate(explosionPrefab, position, rotateAboutX) as Transform;
		
		return explosionWave;
	}

	public void DestroyExplosionWave(Transform wave)
	{
		Destroy(wave.gameObject);
	}
	#endregion

	#region State Methods
	#endregion

	#region Animation Callbacks
	public void OnAnimationEvent()
	{
		_stateManager.currentCharacterState.ReceiveAnimationEvent();
	}

	private void OnCameraAnimationEnded(bool animationStatus)
	{
		_stateManager.currentCharacterState.ReceiveAnimationEvent();
	}
	#endregion
}
