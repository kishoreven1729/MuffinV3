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

	private Transform		_enemySpawnedParticles;

	private Transform		_enemyAsset;
	#endregion

	#region Public Variables
	public int				enemyLevel;

	public Transform		enemyParticles;
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

			_enemyAsset = transform.GetChild(0);

			_enemyAnimator = _enemyAsset.GetComponent<Animator>();
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

		if(_enemyAnimator != null)
		{
			_enemyAnimator.SetFloat("Velocity", _enemyNavMeshAgent.velocity.magnitude);
		}
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

		_enemyAsset.gameObject.SetActive(false);

		GetComponent<BoxCollider>().enabled = false;

		_enemySpawnedParticles = Instantiate(enemyParticles, transform.position, Quaternion.AngleAxis(-90.0f, Vector3.right)) as Transform;

		StartCoroutine(DestroyMouse());
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

	#region Coroutines
	public IEnumerator DestroyMouse()
	{
		yield return new WaitForSeconds(0.8f);

		Destroy(_enemySpawnedParticles.gameObject);

		Destroy(transform.gameObject);
	}
	#endregion
}