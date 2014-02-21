#region References
using UnityEngine;
using System.Collections;
#endregion

public class TrapControl : MonoBehaviour 
{
	#region Private Variables
	private bool		_hasRegenerated;
	private float		_regenTimer;

	private bool		_isPaused;

	private float		_timeLeftOnPause;

	private Transform 	_explosion;
	private Transform	_cheese;

	private bool		_isExploded;
	#endregion

	#region Public Variables
	#endregion

	#region Constructor
	void Awake()
	{
		_isPaused = false;

		_hasRegenerated = false;

		_isExploded = false;

		_regenTimer = Time.time + TrapManager.trapManagerInstance.regenerationCooldown;
	}

	void Start() 
	{	
		try
		{
			_explosion 	= transform.FindChild("Explosion");
			_cheese		= transform.FindChild("Cheese");
		}
		catch(System.Exception ex)
		{
			Debug.Log("TrapControl-Start: \n" + ex.Message);
		}
	}
	#endregion
	
	#region Loop
	void Update () 
	{	
		if(_hasRegenerated == false)
		{
			if(_isPaused == false)
			{
				if(Time.time > _regenTimer)
				{
					_hasRegenerated = true;

					TrapManager.trapManagerInstance.RegenerateTrap();

					if(_isExploded == true)
					{
						Destroy(gameObject);
					}
				}
			}
		}
	}

	void OnTriggerEnter(Collider otherCollider)
	{
		if(otherCollider.CompareTag("Enemy"))
		{
			_explosion.gameObject.SetActive(true);

			collider.enabled = false;

			StartCoroutine("DestroyTrap");
		}
	}
	#endregion

	#region Methods
	public void PauseRegenTimer()
	{
		_isPaused = true;

		_timeLeftOnPause = _regenTimer - Time.time;
	}

	public void RestartTimer()
	{
		_regenTimer = Time.time + _timeLeftOnPause;

		_isPaused = false;
	}

	public IEnumerator DestroyTrap()
	{
		yield return new WaitForSeconds(0.2f);

		_explosion.gameObject.SetActive(false);

		TrapManager.trapManagerInstance.DestroyTrap(gameObject.name);

		if(_hasRegenerated == false)
		{
			_cheese.renderer.enabled = false;

			_isExploded = true;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	#endregion
}
