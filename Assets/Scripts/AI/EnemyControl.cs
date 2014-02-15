#region References
using UnityEngine;
using System.Collections;
#endregion

public class EnemyControl : MonoBehaviour 
{
	#region Private Variables
	private NavMeshAgent	_enemyNavMeshAgent;
	private float			_enemyDistanceThreshold;
	private bool			_enableEnemy;
	#endregion

	#region Public Variables
	public int				enemyLevel;
	#endregion

	#region Constructor
	void Start()
	{
		_enemyDistanceThreshold = 2.0f;

		try
		{
			_enemyNavMeshAgent = GetComponent<NavMeshAgent>();
		}
		catch (System.Exception ex)
		{
			Debug.Log("EnemyControl-Start: \n" + ex.Message);
		}

		_enableEnemy = true;
	}
	#endregion
	
	#region Loop
	void Update()
	{
		if(_enableEnemy == true)
		{
			if(GameDirector.gameInstance.character != null)
			{
				Vector3 destination = GameDirector.gameInstance.character.position;

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
	}
	#endregion

	#region Methods
	public void PauseEnemy()
	{
		_enableEnemy = false;
	}

	public void ResumeEnemy()
	{
		_enableEnemy = true;
	}
	#endregion
}