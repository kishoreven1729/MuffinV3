#region References
using UnityEngine;
using System.Collections;
#endregion

public class PowerupControl : MonoBehaviour 
{
	#region Private Variables
	private float		_powerupExpiryInterval;
	#endregion

	#region Public Variables
	public PowerupManager.PowerupType 	powerupType;
	#endregion

	#region Constructor
	void Awake()
	{
		_powerupExpiryInterval = 5.0f;

		StartCoroutine("Expiry");
	}

	void Start() 
	{
	}
	#endregion
	
	#region Loop
	void Update() 
	{

	}

	public void OnTriggerEnter(Collider otherCollider)
	{
		if(otherCollider.CompareTag("Player"))
		{
			PowerupManager.powerupManagerInstance.availablePowerupType = powerupType;

			StopCoroutine("Expiry");

			RemovePowerup();
		}
	}
	#endregion

	#region Methods
	public IEnumerator Expiry()
	{
		yield return new WaitForSeconds(_powerupExpiryInterval);

		RemovePowerup();
	}

	public void RemovePowerup()
	{
		PowerupManager.powerupManagerInstance.RemovePowerup(transform.name);

		Destroy(transform.gameObject);
	}

	public void AssignPowerup(PowerupManager.PowerupType randomPowerupType)
	{
		powerupType = randomPowerupType;
	}
	#endregion
}
