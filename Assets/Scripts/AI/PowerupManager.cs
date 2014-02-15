#region References
using UnityEngine;
using System.Collections;
#endregion

public class PowerupManager : MonoBehaviour 
{
	#region Enum
	public enum PowerupType
	{
		A = 0,
		B = 1,
		C = 2,
		D = 3,
		E = 4,
		None
	}
	#endregion

	#region Private Variables
	private bool 			_shouldGeneratePowerup;
	private float			_generatorTimer;
	private int				_powerupTypesCount;
	private Rect			_powerupSpawnBoundary;
	private float			_distanceToTrapThreshold;
	#endregion

	#region Public Variables
	public static PowerupManager 	powerupManagerInstance;

	public float					generatorTimeInterval;
	public Transform[]				powerupTypesPrefabs;					/* Order Matters -> Same as enum decleration */
	#endregion

	#region Constructor
	void Start() 
	{	
		_shouldGeneratePowerup = false;

		_powerupTypesCount = powerupTypesPrefabs.Length;

		_distanceToTrapThreshold = 1.5f;

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
		if(Time.time > _generatorTimer)
		{
			_generatorTimer = Time.time + generatorTimeInterval;

			int index = ChoosePowerupIndex();

			if(index >= 0)
			{
				Vector3 position = ChoosePowerupLocation();

				try
				{
					Instantiate(powerupTypesPrefabs[index], position, Quaternion.identity);
				}
				catch (System.Exception ex)
				{
					Debug.Log("PowerupManager-Update: \n" + ex.Message);
				}
			}
		}
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

	public Vector3 ChoosePowerupLocation()
	{
		Vector3 powerupLocation = Vector3.zero;

		powerupLocation.x = Random.Range(_powerupSpawnBoundary.xMin, _powerupSpawnBoundary.xMax);
		powerupLocation.z = Random.Range(_powerupSpawnBoundary.yMin, _powerupSpawnBoundary.yMax);

		/*Fetch traps data and compute distance*/
		bool awayFromTraps = false;
		int maxIterations = 100;

		while(awayFromTraps == false && maxIterations > 0)
		{
			awayFromTraps = true;

			powerupLocation.x = Random.Range(_powerupSpawnBoundary.xMin, _powerupSpawnBoundary.xMax);
			powerupLocation.z = Random.Range(_powerupSpawnBoundary.yMin, _powerupSpawnBoundary.yMax);

			foreach(Transform trap in TrapManager.trapManagerInstance.trapsCollection.Values)
			{
				float distance = Vector3.Distance(powerupLocation, trap.position);

				if(distance < _distanceToTrapThreshold)
				{
					awayFromTraps = false;
					break;
				}
			}

			maxIterations--;
		}

		return powerupLocation;
	}
	#endregion

	#region Event Handler
	#endregion

	#region Events
	#endregion
}
