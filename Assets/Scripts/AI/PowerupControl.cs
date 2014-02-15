#region References
using UnityEngine;
using System.Collections;
#endregion

public class PowerupControl : MonoBehaviour 
{
	#region Private Variables
	private float	_powerupExpiryInterval;
	private float 	_powerupExpiryTimer;

	private bool	_isPowerupTaken;
	private bool 	_isPowerupUsed;
	#endregion

	#region Public Variables
	public PowerupManager.PowerupType 	powerupType;
	public bool							isOneTimeUse;
	public float						impactTime;
	#endregion

	#region Constructor
	void Awake()
	{
		_powerupExpiryInterval = 5.0f;

		_powerupExpiryTimer = Time.time + _powerupExpiryInterval;

		_isPowerupTaken = false;

		_isPowerupUsed = false;
	}

	void Start() 
	{
	}
	#endregion
	
	#region Loop
	void Update() 
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
			if(_isPowerupUsed == true)
			{
				if(Time.time > _powerupExpiryTimer)
				{
					GameDirector.gameInstance.character.SendMessage("RemovePowerup", powerupType, SendMessageOptions.DontRequireReceiver);

					Destroy(gameObject);
				}
			}
			else if(_isPowerupTaken == true)
			{
				if(Time.time > _powerupExpiryTimer)
				{
					/* Send UI Message */
					Destroy(gameObject);
				}
			}
		}
	}

	public void OnTriggerEnter(Collider collider)
	{
		if(collider.CompareTag("Player") && _isPowerupTaken == false)
		{
			_isPowerupTaken = true;

			/* Send UI Message */

			renderer.enabled = false;

			_powerupExpiryTimer = Time.time + _powerupExpiryInterval * 2.0f;
		}
	}
	#endregion

	#region Methods
	public void UsePowerup()
	{
		_isPowerupUsed = true;

		/*Send UI Message*/

		if(isOneTimeUse == false)
		{
			_powerupExpiryTimer = Time.time + impactTime;

			GameDirector.gameInstance.character.SendMessage("ApplyPowerup", powerupType, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void OverridePowerup()
	{
		Destroy(gameObject);
	}
	#endregion
}
