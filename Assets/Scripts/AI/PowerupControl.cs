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
					Destroy(gameObject);
				}
			}
			else
			{
				if(_isPowerupTaken == true)
				{
					if(Time.time > _powerupExpiryTimer)
					{
						/* Send UI Message */

						if(PowerupManager.powerupManagerInstance.availablePowerupType != PowerupManager.PowerupType.None)
						{
							PowerupManager.powerupManagerInstance.availablePowerupType = PowerupManager.PowerupType.None;
							Destroy(gameObject);
						}
					}
				}
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
	public void UsePowerup()
	{
		PowerupManager.powerupManagerInstance.RemovePowerup(gameObject.name);
		PowerupManager.powerupManagerInstance.availablePowerupType = PowerupManager.PowerupType.None;
		Destroy(gameObject);
	}

	public void AssignPowerup(PowerupManager.PowerupType powerup)
	{
		powerupType = powerup;
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
