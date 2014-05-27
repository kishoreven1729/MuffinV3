#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System;
using Facebook;


#endregion

public class ScoringDirector : MonoBehaviour 
{
	#region Constants
	private const int				PER_KILL_SCORE			= 5;
	#endregion

	#region Private Variables
	private int 					_cummulativeKillScore;
	private int 					_timeScore;
	private float					_survivalTime;

	private bool					_isScoringPaused;

	private long					_lastChunkValue;
	private long					_lastChunkCount;

	private long					_upgradeRatChunkCount;
	private long					_upgradeRatSpeedsChunkCount;
	private long					_powerupThreshold;
	#endregion

	#region Public Variables
	public static ScoringDirector 	scoringInstance;

	public int 						gameScore;
	#endregion

	#region Facebook Variables

	#endregion

	#region Constructor
	void Awake()
	{
		scoringInstance = this;

		//CallFBInit();
	}

	void Start() 
	{
		_survivalTime = 0.0f;

		_timeScore = 0;

		_cummulativeKillScore = 0;

		_isScoringPaused = true;

		gameScore = 0;

		_lastChunkValue = 0;

		_lastChunkCount = 0;

		/*Threshold Setup*/
		_powerupThreshold = 30;
		_upgradeRatChunkCount = 2;
		_upgradeRatSpeedsChunkCount = 20;
	}
	#endregion
	
	#region Loop
	void Update() 
	{
		if(_isScoringPaused == false)
		{
			_survivalTime += Time.deltaTime;

			_timeScore = TimeScore();

			gameScore = _timeScore + _cummulativeKillScore;

			UpdateChunkCount();
		}
	}

	void OnGUI()
	{
	
	}
	#endregion

	#region Methods
	private int TimeScore()
	{
		return Mathf.RoundToInt(_survivalTime);
	}

	public void ResumeScoring()
	{
		_isScoringPaused = false;
	}

	public void PauseScoring()
	{
		_isScoringPaused = true;
	}

	public void ApplyKillScore(int bonus = 1)
	{
		_cummulativeKillScore += bonus * PER_KILL_SCORE;
	}

	public void ResetScoring()
	{
		_isScoringPaused = true;

		_survivalTime = 0.0f;
		_timeScore = 0;
		_cummulativeKillScore = 0;

		_lastChunkValue = 0;
		_lastChunkCount = 0;
	}
	#endregion

	#region Powerup Updates
	public void UpdateChunkCount()
	{
		if(gameScore - _lastChunkValue > _powerupThreshold)
		{
			_lastChunkCount ++;

			_lastChunkValue = _powerupThreshold * _lastChunkCount;

			PowerupManager.powerupManagerInstance.GeneratePowerup();

			if(_lastChunkCount > _upgradeRatChunkCount)
			{
				EnemySpawnManager.enemySpawnManagerInstance.UpEnemyLevel();
			}
		}
	}

	public void AdjustPowerupThreshold(long cost)
	{
		_powerupThreshold += cost;
	}
	#endregion
	
}
