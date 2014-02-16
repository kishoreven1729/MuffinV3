#region References
using UnityEngine;
using System.Collections;
#endregion

public class TrapExplosion : MonoBehaviour 
{
	#region Private Variables
	#endregion

	#region Public Variables
	#endregion

	#region Constructor
	void Awake()
	{
	}

	void Start()
	{
	}
	#endregion
	
	#region Loop
	void Update() 
	{
	}

	void OnTriggerEnter(Collider otherCollider)
	{
		if(otherCollider.CompareTag("Enemy"))
		{
			otherCollider.gameObject.SendMessage("KillByTrap", SendMessageOptions.DontRequireReceiver);
		}
	}
	#endregion

	#region Methods

	#endregion
}
