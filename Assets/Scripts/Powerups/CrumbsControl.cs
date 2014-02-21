#region References
using UnityEngine;
using System.Collections;
#endregion

public class CrumbsControl : MonoBehaviour 
{
	#region Private Variables
	private float 		_crumbsTimer;
	private float		_crumbsExpiryTime;
	private float		_leftOverTime;
	private bool		_isPaused;
	#endregion

	#region Public Variables
	#endregion

	#region Constructor
	void Awake()
	{

	}

	void Start () 
	{
		_crumbsExpiryTime = 4.5f;

		//rigidbody.AddForce(new Vector3(Random.Range(0.0f, 10.0f), Random.Range(50.0f, 100.0f), Random.Range(0.0f, 10.0f)));

		_crumbsTimer = Time.time + _crumbsExpiryTime;

		_isPaused = false;
	}
	#endregion
	
	#region Loop
	void Update () 
	{
		if(_isPaused == false)
		{
			if(Time.time > _crumbsTimer)
			{
				DestroyCrumb();
			}
		}
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
		if(_isPaused == false)
		{
			if(otherCollider.CompareTag("Enemy"))
			{
				otherCollider.gameObject.SendMessage("KillByTrap", SendMessageOptions.DontRequireReceiver);

				DestroyCrumb();
			}
		}
	}
	#endregion

	#region Methods
	public void PauseCrumbsTimer()
	{
		_isPaused = true;

		_leftOverTime = _crumbsTimer - Time.time;
	}

	public void ResumeCrumbsTimer()
	{
		_isPaused = false;

		_crumbsTimer = Time.time + _leftOverTime;
	}

	private void DestroyCrumb()
	{
		CrumbsManager.crumbsInstance.RemoveCrumb(transform.name);

		Destroy(gameObject);
	}
	#endregion
}
