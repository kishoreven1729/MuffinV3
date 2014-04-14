#region References
using UnityEngine;
using System.Collections;
#endregion

public class ScoringDirector : MonoBehaviour 
{
	#region Constants
	private const int				PER_KILL_SCORE			= 5;
	#endregion

	#region Private Variables
	private int 					_cummulativeKillScore;
	private long 					_timeScore;
	private float					_survivalTime;

	private bool					_isScoringPaused;
	#endregion

	#region Public Variables
	public static ScoringDirector 	scoringInstance;
	public long 					gameScore;
	#endregion

	#region Constructor
	void Awake()
	{
		scoringInstance = this;
	}

	void Start() 
	{
		_survivalTime = 0.0f;

		_timeScore = 0;

		_cummulativeKillScore = 0;

		_isScoringPaused = true;

		gameScore = 0;
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
	}
	#endregion
}
