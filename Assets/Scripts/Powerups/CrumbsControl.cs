#region References
using UnityEngine;
using System.Collections;
#endregion

public class CrumbsControl : MonoBehaviour 
{
	#region Private Variables
	#endregion

	#region Public Variables
	#endregion

	#region Constructor
	void Awake()
	{
		Physics.IgnoreLayerCollision(9, 10);
		Physics.IgnoreLayerCollision(10, 10);
	}

	void Start () 
	{
		//rigidbody.AddForce(new Vector3(Random.Range(0.0f, 10.0f), Random.Range(50.0f, 100.0f), Random.Range(0.0f, 10.0f)));
	}
	#endregion
	
	#region Loop
	void Update () 
	{

	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.CompareTag("Ground"))
		{
			rigidbody.useGravity = false;
			rigidbody.Sleep();
			collider.isTrigger = true;
		}
	}

	void OnTriggerEnter(Collider otherCollider)
	{
		if(otherCollider.CompareTag("Enemy"))
		{
			otherCollider.gameObject.SendMessage("KillByTrap", SendMessageOptions.DontRequireReceiver);
			DestroyCrumb();
		}
	}
	#endregion

	#region Methods
	private void DestroyCrumb()
	{
		transform.gameObject.SetActive(false);
	}
	#endregion
}
