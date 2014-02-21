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

	private bool  _isPaused;
	#endregion

	#region Public Variables
	#endregion

	#region Constructor
	void Awake()
	{
		_impactTime = 0.6f;
		_impactTimer = Time.time + _impactTime;

		_isPaused = false;
	}

	void Start () 
	{
	}
	#endregion
	
	#region Loop
	void Update () 
	{
		if(_isPaused == false)
		{
			if(Time.time > _impactTimer)
			{
				gameObject.SetActive(false);
			}
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

	#region Methods
	public void PauseExplosion()
	{
		_leftOverTime = _impactTimer - Time.time;

		_isPaused = true;
	}

	public void ResumeExplosion()
	{
		_isPaused = false;

		_impactTimer = Time.time + _leftOverTime;
	}
	#endregion
}
