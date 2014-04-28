#region References
using UnityEngine;
using System.Collections;
#endregion

public class CharacterExplosion : MonoBehaviour 
{
	#region Private Variables
	private float _impactTime;
	private float _impactTimer;
	private float _leftOverTime;
	#endregion

	#region Public Variables
	#endregion

	#region Constructor
	void Awake()
	{
		_impactTime = 0.6f;
		_impactTimer = Time.time + _impactTime;

		Debug.Log("Setup the explosion");
	}

	void Start () 
	{
	}
	#endregion
	
	#region Loop
	void Update () 
	{
		if(Time.time > _impactTimer)
		{
			gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider otherCollider)
	{
		if(otherCollider.CompareTag("Enemy"))
		{
			otherCollider.SendMessage("KillByTrap", SendMessageOptions.DontRequireReceiver);
		}
	}
	#endregion
}
