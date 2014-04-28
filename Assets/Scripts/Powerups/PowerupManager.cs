﻿#region References
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
		ChocoRush
	}
	#endregion

	#region Private Variables
	/*
	private bool 								_shouldGeneratePowerup;
	private float								_generatorTimer;
	private float								_leftOverTime;
	*/

	private int									_powerupTypesCount;
	private Rect								_powerupSpawnBoundary;
	private float								_distanceToTrapThreshold;

	private Dictionary<string, Transform>		_powerupsCollection;

	private PowerupIndexGenerator				_powerupIndexGenerator;
	#endregion

	#region Grid Variables
	private Vector2								_gridDimensions;
	private Vector3[,]							_powerupGrid;
	private int 								_gridCountX = 6; 
	private int 								_gridCountY = 4;
	#endregion

	#region Public Variables
	public static PowerupManager 	powerupManagerInstance;

	//public float					generatorTimeInterval;
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
		//_shouldGeneratePowerup = false;

		_distanceToTrapThreshold = 1.5f;

		_powerupTypesCount = 3;

		_powerupsCollection = new Dictionary<string, Transform>();

		availablePowerupType = PowerupType.None;

		_powerupIndexGenerator = new PowerupIndexGenerator(_powerupTypesCount);

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

		MakeGrid();
	}
	#endregion

	#region Startup Methods
	private void MakeGrid()
	{
		_gridDimensions = new Vector2(_powerupSpawnBoundary.width / _gridCountX, _powerupSpawnBoundary.height / _gridCountY);

		_powerupGrid = new Vector3[_gridCountY, _gridCountX];

		Vector3 initialLocation = new Vector3(_powerupSpawnBoundary.xMin + _gridDimensions.x / 2, 0.0f, _powerupSpawnBoundary.yMin + _gridDimensions.y / 2);

		for(int x = 0; x < _gridCountY; x++)
		{
			for(int y = 0; y < _gridCountX; y++)
			{
				Vector3 gridLocation = initialLocation + new Vector3(_gridDimensions.x * y, 0.0f, _gridDimensions.y * x);

				_powerupGrid[x,y] = gridLocation;
			}
		}
	}

	private Vector3 GetPowerupLocation()
	{
		Vector3 location = Vector3.zero;

		Vector3 characterLocation = GameDirector.gameInstance.character.position;

		int xIndex = 0, yIndex = 0;

		if(characterLocation.x < _powerupSpawnBoundary.xMin)
		{
			xIndex = 0;
		}
		else if(characterLocation.x > _powerupSpawnBoundary.xMax)
		{
			xIndex = _gridCountY - 1;
		}
		else
		{
			float yDist = characterLocation.y - _powerupSpawnBoundary.yMin;

			xIndex = (int) (yDist / _gridDimensions.y);
		}

		if(characterLocation.y < _powerupSpawnBoundary.yMin)
		{
			yIndex = 0;
		}
		else if(characterLocation.y > _powerupSpawnBoundary.yMax)
		{
			yIndex = _gridCountX - 1;
		}
		else
		{
			float xDist = characterLocation.x - _powerupSpawnBoundary.xMin;
			
			yIndex = (int) (xDist / _gridDimensions.x);
		}

		/*Find a random point around the character's location*/
		List<int> possibleIndices = new List<int>();

		int minIndexX = xIndex - 2, maxIndexX = xIndex + 2, minIndexY = yIndex - 2, maxIndexY = yIndex + 2;

		if(minIndexX < 0)
		{
			minIndexX = 0;
		}

		if(maxIndexX >= _gridCountY)
		{
			maxIndexX = _gridCountY - 1;
		}

		if(minIndexY < 0)
		{
			minIndexY = 0;
		}
		
		if(maxIndexY >= _gridCountX)
		{
			maxIndexY = _gridCountX - 1;
		}

		int randIndexX = Random.Range(minIndexX, maxIndexX + 1);
		int randIndexY = Random.Range(minIndexY, maxIndexY + 1);

		location = _powerupGrid[randIndexX, randIndexY];

		return location;
	}
	#endregion
	
	#region Loop
	void Update () 
	{
		/*if(Time.time > _generatorTimer && _shouldGeneratePowerup == true)
		{
			_generatorTimer = Time.time + generatorTimeInterval;

			int index = ChoosePowerupIndex();

			Vector3 position = ChoosePowerupLocation();

			if(index >= 0)
			{
				AddPowerup(position, index);
			}
		}*/
	}

	void OnGUI()
	{
	}

	void OnDrawGizmos()
	{
		for(int x = 0; x < _gridCountY; x++)
		{
			for(int y = 0; y < _gridCountX; y++)
			{
				//if(_powerupGrid != null)
				//{
				//	Gizmos.DrawSphere(_powerupGrid[x,y], 1.0f);
				//}
			}
		}
	}
	#endregion

	#region Methods
	public void GeneratePowerup()
	{
		int index = ChoosePowerupIndex();
		
		//Vector3 position = ChoosePowerupLocation();
		Vector3 position = GetPowerupLocation();
		
		if(index >= 0)
		{
			AddPowerup(position, index);
		}
	}

	private int ChoosePowerupIndex()
	{
		int index = -1;

		float shouldSpawnProbability = Random.Range(0.0f, 1.0f);

		if(shouldSpawnProbability > 0.15f)
		{
			index = _powerupIndexGenerator.GeneratePowerup();
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
				powerupType = PowerupType.ChocoRush;
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

	public void PausePowerupGeneration(bool isFreeze = false)
	{
		//_shouldGeneratePowerup = false;

		foreach(Transform powerup in _powerupsCollection.Values)
		{
			powerup.SendMessage("PausePowerupTimer", SendMessageOptions.DontRequireReceiver);
		}

		/*if(isFreeze == true)
		{
			_leftOverTime = _generatorTimer - Time.time;
		}*/
	}

	public void ResumePowerupGeneration(bool isFreeze = false)
	{
		/*_shouldGeneratePowerup = true;

		_generatorTimer = Time.time + generatorTimeInterval;*/

		availablePowerupType = PowerupType.None;

		if(isFreeze == true)
		{
			//_generatorTimer = _leftOverTime + Time.time;

			foreach(Transform powerup in _powerupsCollection.Values)
			{
				powerup.SendMessage("ResumePowerupTimer", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void RemoveAllPowerups()
	{
		try
		{
			//_shouldGeneratePowerup = false;

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

	public void ResetPowerups()
	{
		RemoveAllPowerups();

		//ResumePowerupGeneration();
	}
	#endregion

	#region Event Handler
	#endregion

	#region Events
	#endregion
}
