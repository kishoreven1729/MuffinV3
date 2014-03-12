#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class PowerupManager : MonoBehaviour 
{
	#region Enum
	public enum PowerupType
	{
		None,
		CranberrySpin,
		HoneyBlast,
		Crumbs
	}
	#endregion

	#region Private Variables
	private bool 								_shouldGeneratePowerup;
	private float								_generatorTimer;
	private int									_powerupTypesCount;
	private Rect								_powerupSpawnBoundary;
	private float								_distanceToTrapThreshold;

	private float								_leftOverTime;
	private Dictionary<string, Transform>		_powerupsCollection;
	#endregion

	#region Public Variables
	public static PowerupManager 	powerupManagerInstance;

	public float					generatorTimeInterval;
	public Transform				powerupPrefab;

	public Transform				availablePowerup;
	public PowerupType				availablePowerupType;
	#endregion

	#region Constructor
	void Awake()
	{
		powerupManagerInstance = this;
	}

	void Start() 
	{	
		_shouldGeneratePowerup = true;

		_distanceToTrapThreshold = 1.5f;

		_powerupTypesCount = 3;

		_powerupsCollection = new Dictionary<string, Transform>();

		availablePowerupType = PowerupType.None;

		try
		{
			Vector3 bottomRight = GameObject.Find("PlaneBoundaryBottomRight").transform.position;
			Vector3 topLeft = GameObject.Find("PlaneBoundaryTopLeft").transform.position;
			
			_powerupSpawnBoundary = new Rect(topLeft.x, bottomRight.z, Mathf.Abs(bottomRight.x - topLeft.x), Mathf.Abs(topLeft.z - bottomRight.z));
		}
		catch (System.Exception ex)
		{
			Debug.Log("PowerupManager-Start: \n" + ex.Message); 
		}

		_generatorTimer = Time.time + generatorTimeInterval;
	}
	#endregion
	
	#region Loop
	void Update () 
	{
		if(Time.time > _generatorTimer && _shouldGeneratePowerup == true)
		{
			_generatorTimer = Time.time + generatorTimeInterval;

			int index = ChoosePowerupIndex();

			Vector3 position = ChoosePowerupLocation();

			if(index >= 0)
			{
				AddPowerup(position, index);
			}
		}
	}

	void OnGUI()
	{
	}
	#endregion

	#region Methods
	private int ChoosePowerupIndex()
	{
		int index = -1;

		float shouldSpawnProbability = Random.Range(0.0f, 1.0f);

		if(shouldSpawnProbability > 0.3f)
		{
			float value = Random.Range(0.0f, 1.0f);
			
			value *= _powerupTypesCount;
			
			index = (int)value;
		}
		else
		{
			index = -1;
		}

		return index;
	}

	public void AddPowerup(Vector3 position, int powerupIndex)
	{
		try
		{
			string name = "Powerup_" + Time.time;
			Transform powerup = Instantiate(powerupPrefab, position, Quaternion.identity) as Transform;
			powerup.name = name;
			
			_powerupsCollection.Add(name, powerup);

			PowerupType powerupType = PowerupType.None;

			switch(powerupIndex)
			{
			case 0:
				powerupType = PowerupType.CranberrySpin;
				break;
			case 1:
				powerupType = PowerupType.HoneyBlast;
				break;
			case 2:
				powerupType = PowerupType.Crumbs;
				break;
			}

			powerup.SendMessage("AssignPowerup", powerupType, SendMessageOptions.DontRequireReceiver);
		}
		catch (System.Exception ex)
		{
			Debug.Log("AddPowerup-Update: \n" + ex.Message);
		}
	}

	public void RemovePowerup(string powerupName)
	{
		try
		{
			_powerupsCollection.Remove(powerupName);
		}
		catch (System.Exception ex)
		{
			Debug.Log("RemovePowerup-Update: \n" + ex.Message);
		}
	}

	public Vector3 ChoosePowerupLocation()
	{
		Vector3 powerupLocation = Vector3.zero;

		powerupLocation.x = Random.Range(_powerupSpawnBoundary.xMin, _powerupSpawnBoundary.xMax);
		powerupLocation.z = Random.Range(_powerupSpawnBoundary.yMin, _powerupSpawnBoundary.yMax);

		/*Fetch traps data and compute distance*/
		bool awayFromTraps = false;
		int maxIterations = 1000;

		while(awayFromTraps == false && maxIterations > 0)
		{
			awayFromTraps = true;

			powerupLocation.x = Random.Range(_powerupSpawnBoundary.xMin, _powerupSpawnBoundary.xMax);
			powerupLocation.z = Random.Range(_powerupSpawnBoundary.yMin, _powerupSpawnBoundary.yMax);

			try
			{
				foreach(Transform trap in TrapManager.trapManagerInstance.trapsCollection.Values)
				{
					float distance = Vector3.Distance(powerupLocation, trap.position);

					if(distance < _distanceToTrapThreshold)
					{
						awayFromTraps = false;
						break;
					}
				}

				foreach(Transform powerup in _powerupsCollection.Values)
				{
					float distance = Vector3.Distance(powerupLocation, powerup.position);
					
					if(distance < _distanceToTrapThreshold)
					{
						awayFromTraps = false;
						break;
					}
				}

				if(Vector3.Distance(powerupLocation, GameDirector.gameInstance.character.position) < _distanceToTrapThreshold)
				{
					awayFromTraps = false;
				}
			}
			catch(System.Exception ex)
			{
				Debug.Log("PowerupManager-ChoosePowerupLocation: \n" + ex.Message);
			}

			maxIterations--;
		}

		return powerupLocation;
	}

	public void PausePowerupGeneration()
	{
		_shouldGeneratePowerup = false;

		_leftOverTime = _generatorTimer - Time.time;

		foreach(Transform powerup in _powerupsCollection.Values)
		{
			powerup.SendMessage("PausePowerupTimer", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ResumePowerupGeneration()
	{
		_shouldGeneratePowerup = true;

		_generatorTimer = _leftOverTime + Time.time;

		foreach(Transform powerup in _powerupsCollection.Values)
		{
			powerup.SendMessage("ResumePowerupTimer", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void RemoveAllPowerups()
	{
		try
		{
			_shouldGeneratePowerup = false;

			foreach(Transform powerup in _powerupsCollection.Values)
			{
				if(powerup != null)
				{
					Destroy(powerup.gameObject);
				}
			}
			
			_powerupsCollection.Clear();
		}
		catch(System.Exception ex)
		{
			Debug.Log("PowerupManager-RemoveAllPowerups: \n" + ex.Message);
		}
	}
	#endregion

	#region Event Handler
	#endregion

	#region Events
	#endregion
}
