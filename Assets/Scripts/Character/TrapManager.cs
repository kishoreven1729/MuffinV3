#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class TrapManager : MonoBehaviour
{
	#region Private Variables
	private int								_trapSpawnIndex;
	private int								_maxTrapCount;
	private int								_availableTrapCount;
	private float							_regenerationCoolDown;
	#endregion

	#region Public Variables
	public static TrapManager 				trapManagerInstance;

	public Dictionary<string, Transform> 	trapsCollection;

	public Transform[]						trapPrefabs;					//0 - Default Trap
	#endregion

	#region Constructor
	void Start() 
	{
		trapsCollection = new Dictionary<string, Transform>();

		_trapSpawnIndex = 0;

		_maxTrapCount = 3;
		_availableTrapCount = _maxTrapCount;

		_regenerationCoolDown = 5.0f;
	}
	#endregion
	
	#region Loop
	void Update() 
	{	
	}
	#endregion

	#region Methods
	public void AddTrap(bool isPowerup = false)
	{
		string name = "Trap_" + Time.time;

		int trapIndex = 0;

		if(isPowerup == true && _trapSpawnIndex != -1)
		{
			trapIndex = _trapSpawnIndex;

			if(GameDirector.gameInstance.currentPowerup != null)
			{
				GameDirector.gameInstance.currentPowerup.SendMessage("UsePowerup");
			}

			_trapSpawnIndex = -1;
		}

		try
		{
			Transform trap = Instantiate(trapPrefabs[trapIndex], GameDirector.gameInstance.characterDropLocation.position, Quaternion.identity) as Transform;
			trap.name = name;

			trapsCollection.Add(name, trap);
		}
		catch(System.Exception ex)
		{
			Debug.Log("TrapManager-AddTrap: \n" + ex.Message);

			_availableTrapCount--;

			StartCoroutine("RegenerateTrap");
		}
	}

	public void DestroyTrap()
	{
	}

	public void AddPowerupToNextTrap(PowerupManager.PowerupType powerupType)
	{
		switch(powerupType)
		{
		case PowerupManager.PowerupType.A:
			break;
		case PowerupManager.PowerupType.B:
			break;
		case PowerupManager.PowerupType.C:
			break;
		case PowerupManager.PowerupType.D:
			break;
		case PowerupManager.PowerupType.E:
			break;
		}
	}

	public IEnumerator RegenerateTrap()
	{
		yield return new WaitForSeconds(_regenerationCoolDown);

		if(_availableTrapCount < _maxTrapCount)
		{
			_availableTrapCount++;
		}
	}
	#endregion
}
