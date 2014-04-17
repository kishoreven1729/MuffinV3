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
	#endregion

	#region Public Variables
	public static ScoringDirector 	scoringInstance;
	public long 					gameScore;
	#endregion

	#region Facebook Variables
	private Dictionary<string, string> profile;
	#endregion

	#region Constructor
	void Awake()
	{
		scoringInstance = this;

		CallFBInit();
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

	#region Facebook Methods
	private void CallFBInit()
	{
		FB.Init(OnInitComplete, OnHideUnity);
	}
	
	private void OnInitComplete()
	{
		//Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);

		if(FB.IsLoggedIn == false)
		{
			FB.Login("email,publish_actions", LoginCallback);
		}
	}
	
	private void OnHideUnity(bool isGameShown)
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

		OnLoggedIn();
	}

	void OnLoggedIn()
	{
		Debug.Log("Logged in. ID: " + FB.UserId);
		
		// Reqest player info and profile picture
		FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, APICallback);
	}

	void APICallback(FBResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError(result.Error);
			// Let's just try again
			FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, APICallback);
			return;
		}
		
		profile = DeserializeJSONProfile(result.Text);

		Debug.Log("Logged in User: " + profile["first_name"]);
	}

	public static Dictionary<string, string> DeserializeJSONProfile(string response)
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
				linkCaption: "I just scored " + gameScore + " on the test version of Muffin Morphosis",
				linkName: "Test post from Muffin Morphosis"
				);

			Debug.Log("Posting On facebook");
		}
	}
	#endregion
}
