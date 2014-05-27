#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System;
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

	private long					_lastChunkValue;
	private long					_lastChunkCount;

	private long					_upgradeRatChunkCount;
	private long					_upgradeRatSpeedsChunkCount;
	private long					_powerupThreshold;
	#endregion

	#region Public Variables
	public static ScoringDirector 	scoringInstance;

	public long 					gameScore;
	#endregion

	#region Facebook Variables
	private Dictionary<string, string> 	_profile;
	public string 						facebookName;
	#endregion

	#region Constructor
	void Awake()
	{
		scoringInstance = this;

		facebookName = "none";

		CallFBInit();
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
		_upgradeRatChunkCount = 10;
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
		GUI.Label(new Rect(30, 30, 150, 150), "Name: " + facebookName);
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
			if(_lastChunkCount > _upgradeRatSpeedsChunkCount)
			{
			}
		}
	}

	public void AdjustPowerupThreshold(long cost)
	{
		_powerupThreshold += cost;
	}
	#endregion

	#region Facebook Methods
	private void CallFBInit()
	{
		FB.Init(OnInitComplete, OnHideUnity);
	}
	
	void OnInitComplete()
	{
		//Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);

		if(FB.IsLoggedIn == false)
		{
			FB.Login("email,publish_actions", LoginCallback);
		}
	}
	
	void OnHideUnity(bool isGameShown)
	{
		Debug.Log("Is game showing? " + isGameShown);
	}
	
	void LoginCallback(FBResult result)
	{
		/*if (result.Error != null)
			lastResponse = "Error Response:\n" + result.Error;
		else if (!FB.IsLoggedIn)
		{
			lastResponse = "Login cancelled by Player";
		}
		else
		{
			lastResponse = "Login was successful!";
		}*/

		ProfileFetch();
	}

	void ProfileFetch()
	{
		Debug.Log("Logged in. ID: " + FB.UserId);
		
		// Reqest player info and profile picture
		FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, ProfileFetchCallback);
	}

	void ProfileFetchCallback(FBResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError(result.Error);
			// Let's just try again
			FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, ProfileFetchCallback);
			return;
		}
		
		_profile = DeserializeJSONProfile(result.Text);

		facebookName = _profile["first_name"];

		Debug.Log("Logged in User: " + facebookName);
	}

	public Dictionary<string, string> DeserializeJSONProfile(string response)
	{
		var responseObject = Json.Deserialize(response) as Dictionary<string, object>;
		object nameH;
		var profile = new Dictionary<string, string>();
		if (responseObject.TryGetValue("first_name", out nameH))
		{
			profile["first_name"] = (string)nameH;
		}
		return profile;
	}
	
	public void PostOnFacebook()
	{
		if(FB.IsLoggedIn)
		{
			FB.Feed(
				linkCaption: "I just scored " + gameScore + " on the test version of Muffin Morphosis!",
				linkName: "Muffin Morphosis - Quest for more crumbs",
				picture: "http://kishorevenkateshan.com/Downloads/MuffinSplashScreen.png",
				callback: OnPostComplete
				);

			Debug.Log("Posting On facebook");
		}
	}

	void OnPostComplete(FBResult response)
	{
		Debug.Log("Facebook Post Compelete");
	}

	public void PostChallenge()
	{
		if(FB.IsLoggedIn)
		{
			FB.AppRequest(
				message: "Do you think you can surpass my score," + gameScore + "? ",
				callback: OnPostChallengeComplete
				);
		}
	}

	void OnPostChallengeComplete(FBResult response)
	{
		Debug.Log("Facebook Post Challenge Compelete");
	}
	#endregion
}
