﻿#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class EnemySpawnManager : MonoBehaviour 
{
	#region Private Variables
	private Transform[] 			_enemySpawnPoints;
	private int						_enemySpawnPointsCount;

	private float					_spawnInterval;
	private float					_spawnTimer;
		
	private int						_numberOfSpawns;

	private bool					_isSpawning;

	private int						_enemyLevel;
	private int						_maxEnemyLevel;
	#endregion

	#region Public Variables
	public static EnemySpawnManager			enemySpawnManagerInstance;
	public Transform[]						enemyPrefabs;
	public Dictionary<string, Transform>	enemyCollection;
	#endregion

	#region Constructor
	void Awake()
	{
		enemySpawnManagerInstance = this;
	}

	void Start()
	{
		_spawnTimer = 0.0f;
		_spawnInterval = 5.0f;

		_numberOfSpawns = 1;

		_isSpawning = false;

		_enemyLevel = 1;
		_maxEnemyLevel = enemyPrefabs.Length;

		enemyCollection = new Dictionary<string, Transform>();

		try
		{
			_enemySpawnPointsCount = transform.childCount;

			_enemySpawnPoints = new Transform[_enemySpawnPointsCount];

			for(int childIndex = 0; childIndex < _enemySpawnPointsCount; childIndex ++)
			{
				_enemySpawnPoints[childIndex] = transform.GetChild(childIndex);
			}
		}
		catch(System.Exception ex)
		{
			Debug.Log("EnemySpawnManager-Start: \n" + ex.Message);
		}
	}
	#endregion
	
	#region Loop
	void Update()
	{
		if(_isSpawning == true)
		{
			float currentTime = Time.time;

			if(currentTime > _spawnTimer)
			{
				_spawnTimer += _spawnInterval;

				SpawnEnemy(currentTime);
			}
		}
	}
	#endregion

	#region Methods
	private void SpawnEnemy(float time)
	{
		int index = ChooseRandomIndex(_enemySpawnPointsCount);

		try
		{
			string enemyName = "Enemy_" + index + "_" + time;

			Transform enemySpawnPoint = _enemySpawnPoints[index];

			int enemyLevelIndex = ChooseRandomIndex(_maxEnemyLevel);

			Transform enemy = Instantiate(enemyPrefabs[enemyLevelIndex], enemySpawnPoint.position, enemySpawnPoint.rotation) as Transform;
			enemy.name = enemyName;

			enemyCollection.Add(enemyName, enemy);
		}
		catch(System.Exception ex)
		{
			Debug.Log("EnemySpawnManager-SpawnEnemy: \n" + ex.Message); 
		}
	}

	private int ChooseRandomIndex(int maxExclusiveValue)
	{
		float randomValue = Random.Range(0.0f, 1.0f);

		randomValue *= maxExclusiveValue;

		int index = (int)randomValue;

		return index;
	}

	public void UpdgradeEnemySpawnCount()
	{
		_numberOfSpawns++;
	}

	public void PauseSpawning()
	{
		_isSpawning = false;

		PauseAllEnemies();
	}

	public void ResumeSpawning()
	{
		_isSpawning = true;
	}

	public void KillEnemy(string enemyName)
	{
		try
		{
			enemyCollection.Remove(enemyName);
		}
		catch(System.Exception ex)
		{
			Debug.Log("EnemySpawnManager-KillEnemy: \n" + ex.Message);
		}
	}

	public void PauseAllEnemies()
	{
		foreach(Transform enemy in enemyCollection.Values)
		{
			enemy.SendMessage("PauseEnemy", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ResumeAllEnemies()
	{
		foreach(Transform enemy in enemyCollection.Values)
		{
			enemy.SendMessage("ResumeEnemy", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void KillAllEnemies()
	{
		foreach(Transform enemy in enemyCollection.Values)
		{
			Destroy(enemy.gameObject);
		}

		enemyCollection.Clear();
	}

	public void UpEnemyLevel()
	{
		_enemyLevel ++;

		if(_enemyLevel > _maxEnemyLevel)
		{
			_enemyLevel = _maxEnemyLevel;
		}
	}
	#endregion
}
