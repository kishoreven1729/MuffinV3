#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#endregion

public class State
{
	#region Protected Variables
	protected object		_stateTransitionData;
	protected Transform		_character;
	protected Animator		_characterAnimator;
	protected StateManager 	_stateManager;
	#endregion

	#region Public Variables
	public string 			stateName;
	public string			animationTriggerString;
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="CharacterState"/> class. Each state is a node in the <see cref="CharacterStateManager"/>
	/// </summary>
	/// <param name="name">Name of the State for use in dictionary.</param>
	/// <param name="udpateFunction">Udpate function that will called in character update loop when the state is active.</param>
	/// <param name="animationTriggerValue">Animation trigger string if the state changes needs an animation to be played. Pass null if there isn't any.</param>
	public State(StateManager stateManager, Transform character, string name, string animationTriggerValue)
	{	
		stateName = name;
		animationTriggerString = animationTriggerValue;

		_stateManager = stateManager;

		_character = character;

		_characterAnimator = _character.FindChild("Asset").GetComponent<Animator>();
	}
	#endregion
	
	#region Methods
	public void PushTransitionData(object transitionData)
	{
		_stateTransitionData = transitionData;
	}

	public object PullTransitionData()
	{
		if(_stateTransitionData != null)
		{
			object returnData = _stateTransitionData;
			_stateTransitionData = null;

			return returnData;
		}

		return null;
	}
	#endregion

	#region Virtual Methods
	public virtual void OnStateEnter()
	{
	}

	public virtual void UpdateFunction()
	{
	}
	
	public virtual void CollisionFunction(Collider collidee)
	{
	}

	public virtual void OnStateExit()
	{
	}

	public virtual void ReceiveAnimationEvent()
	{
	}
	#endregion
}
