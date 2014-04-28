#region References
using UnityEngine;
using System.Collections;
#endregion

public class PowerupControl : MonoBehaviour 
{
	#region Private Variables
	private float		_powerupExpiryInterval;
	private float 		_powerupExpiryTimer;

	private bool		_isPowerupTaken;
	private bool 		_isPowerupUsed;

	private float		_impactTime;

	private bool		_isPaused;
	private float		_leftOverTime;

	private Transform	_powerupJar;
	#endregion

	#region Public Variables
	public PowerupManager.PowerupType 	powerupType;
	#endregion

	#region Constructor
	void Awake()
	{
		_powerupExpiryInterval = 5.0f;

		_impactTime = 0.0f;

		_powerupExpiryTimer = Time.time + _powerupExpiryInterval;

		_isPowerupTaken = false;

		_isPowerupUsed = false;

		_isPaused = false;
	}

	void Start() 
	{
		try
		{
			_powerupJar = transform.FindChild("PowerupJar");
		}
		catch(System.Exception ex)
		{
			Debug.Log("PowerupControl-Start: \n" + ex.Message);
		}
	}
	#endregion
	
	#region Loop
	void Update() 
	{
		if(_isPaused == false)
		{
			if(_isPowerupTaken == false)
			{
				if(Time.time > _powerupExpiryTimer)
				{
					RemovePowerup();
				}
			}
			else
			{
				if(_isPowerupUsed == true)
				{
					if(Time.time > _powerupExpiryTimer)
					{
						EnemySpawnManager.enemySpawnManagerInstance.FreezeEnemies(true);
						RemovePowerup();
					}
				}
				/*else
				{
					if(_isPowerupTaken == true)
					{
						if(Time.time > _powerupExpiryTimer)
						{
							// Send UI Message 

							if(PowerupManager.powerupManagerInstance.availablePowerupType != PowerupManager.PowerupType.None)
							{
								RemovePowerup();
							}
						}
					}
				}*/
			}
		}
	}

	public void OnTriggerEnter(Collider otherCollider)
	{
		if(_isPaused == false)
		{
			if(otherCollider.CompareTag("Player") && _isPowerupTaken == false)
			{
				if(PowerupManager.powerupManagerInstance.availablePowerupType != PowerupManager.PowerupType.None)
				{
					PowerupManager.powerupManagerInstance.RemovePowerup(PowerupManager.powerupManagerInstance.availablePowerup.gameObject.name);
					Destroy(PowerupManager.powerupManagerInstance.availablePowerup.gameObject);
				}

				PowerupManager.powerupManagerInstance.availablePowerup = transform;
				PowerupManager.powerupManagerInstance.availablePowerupType = powerupType;

				_isPowerupTaken = true;

				_powerupJar.gameObject.SetActive(false);
				collider.enabled = false;

				_powerupExpiryTimer = Time.time + _powerupExpiryTimer;
			}
		}
	}
	#endregion

	#region Methods
	public void RemovePowerup(bool totalDestroy = true)
	{
		if(_isPowerupUsed == false)
		{
			PowerupManager.powerupManagerInstance.RemovePowerup(gameObject.name);
			PowerupManager.powerupManagerInstance.availablePowerupType = PowerupManager.PowerupType.None;
		}

		if(totalDestroy == true)
		{
			Destroy(gameObject);
		}
	}

	public void UsePowerup()
	{
		if(powerupType == PowerupManager.PowerupType.HoneyBlast)
		{
			EnemySpawnManager.enemySpawnManagerInstance.FreezeEnemies(false);

			RemovePowerup(false);

			_powerupExpiryTimer = Time.time + _impactTime;
			_isPowerupUsed = true;
		}
		else
		{
			RemovePowerup();
		}
	}

	public void AssignPowerup(PowerupManager.PowerupType powerup)
	{
		powerupType = powerup;

		if(powerupType == PowerupManager.PowerupType.HoneyBlast)
		{
			_impactTime = 3.0f;
		}
	}

	public void PausePowerupTimer()
	{
		_isPaused = true;
		_leftOverTime = _powerupExpiryTimer - Time.time;
	}

	public void ResumePowerupTimer()
	{
		_isPaused = false;
		_powerupExpiryTimer = _leftOverTime + Time.time;
	}
	#endregion
}
