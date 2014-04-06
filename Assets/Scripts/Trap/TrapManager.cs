#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class TrapManager : MonoBehaviour
{
	#region Private Variables
	private int								_maxTrapCount;
	private	Quaternion						_dropRotation;
	#endregion

	#region Public Variables
	public static TrapManager 				trapManagerInstance;

	public Dictionary<string, Transform> 	trapsCollection;

	public Transform						trapPrefab;
	public float							regenerationCooldown;

	public int								availableTrapCount;
	#endregion

	#region Constructor
	void Awake()
	{
		trapManagerInstance = this;
	}

	void Start() 
	{
		trapsCollection = new Dictionary<string, Transform>();

		_maxTrapCount = 3;
		availableTrapCount = _maxTrapCount;

		_dropRotation = Quaternion.AngleAxis(150.0f, Vector3.up);
	}
	#endregion
	
	#region Loop
	void Update() 
	{	
	}
	#endregion

	#region Methods
	public void AddTrap()
	{
		if(availableTrapCount > 0)
		{
			string name = "Trap_" + Time.time;

			try
			{
				Transform trap = Instantiate(trapPrefab, GameDirector.gameInstance.characterDropLocation.position, _dropRotation) as Transform;
				trap.name = name;

				trapsCollection.Add(name, trap);
			}
			catch(System.Exception ex)
			{
				Debug.Log("TrapManager-AddTrap: \n" + ex.Message);
			}

			availableTrapCount--;
		}
	}

	public void DestroyTrap(string trapName)
	{
		try
		{
			trapsCollection.Remove(trapName);
		}
		catch(System.Exception ex)
		{
			Debug.Log("TrapManager-DestroyTrap: \n" + ex.Message);
		}
	}

	public void RegenerateTrap()
	{
		if(availableTrapCount < _maxTrapCount)
		{
			availableTrapCount++;
		}
	}

	public void PauseTrapTimers()
	{
		foreach(Transform trap in trapsCollection.Values)
		{
			trap.SendMessage("PauseRegenTimer", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ResumeTrapTimers()
	{
		foreach(Transform trap in trapsCollection.Values)
		{
			trap.SendMessage("RestartTimer", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void DestroyAllTraps()
	{
		try
		{
			foreach(Transform trap in trapsCollection.Values)
			{
				Destroy(trap.gameObject);
			}
			
			trapsCollection.Clear();
		}
		catch(System.Exception ex)
		{
			Debug.Log("TrapManager-DestroyAllTraps: \n" + ex.Message);
		}
	}

	public void ResetTrapManager()
	{
		DestroyAllTraps();

		availableTrapCount = _maxTrapCount;
	}
	#endregion
}
