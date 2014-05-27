#region References
using UnityEngine;
using System.Collections;
#endregion

public class EnemyControl : MonoBehaviour 
{
	#region Private Variables
	private NavMeshAgent	_enemyNavMeshAgent;
	private float			_enemyDistanceThreshold;

	private bool			_glueEnemy;

	private Animator		_enemyAnimator;

	private float 			_upgradeVelocity;
	#endregion

	#region Public Variables
	public int				enemyLevel;
	#endregion

	#region Constructor
	void Awake()
	{
		_upgradeVelocity = 0.0f;
	}

	void Start()
	{
		_enemyDistanceThreshold = 2.0f;

		try
		{
			_enemyNavMeshAgent = GetComponent<NavMeshAgent>();

			_enemyAnimator = transform.GetChild(0).GetComponent<Animator>();
		}
		catch (System.Exception ex)
		{
			Debug.Log("EnemyControl-Start: \n" + ex.Message);
		}

		_glueEnemy = false;

		_enemyNavMeshAgent.speed += _upgradeVelocity; 
	}
	#endregion
	
	#region Loop
	void Update()
	{
		if(_glueEnemy == false)
		{
			if(GameDirector.gameInstance.character != null)
			{
				Vector3 destination = FindNextDestination();

				_enemyNavMeshAgent.destination = destination;

				if(Vector3.Distance(destination, transform.position) < _enemyDistanceThreshold)
				{
					_enemyNavMeshAgent.velocity = Vector3.zero;
				}
			}
		}
		else
		{
			_enemyNavMeshAgent.velocity = Vector3.zero;
		}	

		_enemyAnimator.SetFloat("Velocity", _enemyNavMeshAgent.velocity.magnitude);
	}
	#endregion

	#region Methods
	private Vector3 FindNextDestination()
	{
		Vector3 nextTarget = GameDirector.gameInstance.character.position;

		float distance = Vector3.Distance(transform.position, nextTarget);

		foreach(Transform trap in TrapManager.trapManagerInstance.trapsCollection.Values)
		{
			float trapDistance = Vector3.Distance(transform.position, trap.position);

			if(trapDistance < distance)
			{
				distance = trapDistance;
				nextTarget = trap.position;
			}
		}

		return nextTarget;
	}

	public void KillByTrap()
	{
		ScoringDirector.scoringInstance.ApplyKillScore();

		EnemySpawnManager.enemySpawnManagerInstance.KillEnemy(gameObject.name);

		Destroy(gameObject);
	}

	public void FreezeBlast()
	{
		_glueEnemy = true;
	}

	public void UnFreezeBlast()
	{
		_glueEnemy = false;
	}

	public void SetVelocity(float speed)
	{
		_upgradeVelocity = speed;
	}
	#endregion
}