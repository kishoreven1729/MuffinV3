#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#endregion

public class StateManager 
{
	#region Private Variables
	private Animator 								_characterAnimator;
	private Dictionary<string, State>				_stateDictionary;
	private string									_defaultStateName;
	#endregion

	#region Public Variables
	public State 		currentCharacterState;
	#endregion

	#region Constructor
	public StateManager(Animator characterAnimator)
	{
		_characterAnimator = characterAnimator;

		_stateDictionary = new Dictionary<string, State>();
	}

	/// <summary>
	/// Sets the default state. The default state is also the starting state of the StateManager. Eg Idle. Must be called after Adding states to the StateManager
	/// </summary>
	/// <param name="defaultStateName">Default state name given in the dictionary.</param>
	public void SetDefaultState(string defaultStateName)
	{
		currentCharacterState = _stateDictionary[defaultStateName];

		_defaultStateName = defaultStateName;
	}
	#endregion

	#region Methods
	public void AddCharacterState(State state)
	{
		try
		{
			_stateDictionary.Add(state.stateName, state);
		}
		catch(System.Exception ex)
		{
			Debug.Log("CharacterStateManager-AddCharacterState: " + ex.Message);
		}
	}

	public void RemoveCharacterState(string stateName)
	{
		try
		{
			_stateDictionary.Remove(stateName);
		}
		catch(System.Exception ex)
		{
			Debug.Log("CharacterStateManager-RemoveCharacterState: " + ex.Message);
		}
	}

	public void SwitchToState(string stateName)
	{
		try
		{
			currentCharacterState.OnStateExit();

			currentCharacterState = _stateDictionary[stateName];

			currentCharacterState.OnStateEnter();

			/*if(currentCharacterState.animationTriggerString != null)
			{
				SetAnimationTrigger();
			}*/
		}
		catch (System.Exception ex)
		{
			Debug.Log("CharacterStateManager-SwitchToState: " + ex.Message);
		}
	}

	public void PushTransitionData(string stateName, object data)
	{
		_stateDictionary[stateName].PushTransitionData(data);
	}

	public object PullTransitionDataFromCurrentState()
	{
		return currentCharacterState.PullTransitionData();
	}

	public void ReturnToDefaultState()
	{
		if(_defaultStateName != null)
		{
			SwitchToState(_defaultStateName);
		}
	}

	public void SetAnimationTrigger(string value)
	{
		if(currentCharacterState != null && _characterAnimator.GetCurrentAnimatorStateInfo(0).IsName(currentCharacterState.animationTriggerString) == false)
		{
			_characterAnimator.SetTrigger(currentCharacterState.animationTriggerString);
		}
	}

	public void RequestAnimationTriggerInCurrentState(string triggerValue)
	{
		_characterAnimator.SetTrigger(triggerValue);
	}
	#endregion
}
