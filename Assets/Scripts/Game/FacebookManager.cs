using UnityEngine;
using System.Collections;
using Facebook;
using Facebook.MiniJSON;
using System.Collections.Generic;
using System;

public class FacebookManager : MonoBehaviour
{
	#region Private Variables
	private Dictionary<string, string> 	_profile;
	private List<object>			    _fetchedScores;
	#endregion

	#region Public Variables
	public string 						facebookName;
	public int							facebookScore;

	public Dictionary<string, int>		friendsHighScore;

	public static FacebookManager		facebookInstance;
	#endregion

	#region Constructor
	void Awake()
	{
		if(facebookInstance == null)
		{
			facebookInstance = this;
		}
	}

	void Start()
	{
		CallFBInit();
	}
	#endregion

	#region Loop
	void OnGUI()
	{
		GUI.Label(new Rect(30, 30, 150, 150), "Name: " + facebookName);
		GUI.Label(new Rect(30, 60, 150, 150), "Score: " + facebookScore);
	}
	#endregion

	#region Facebook Methods
	public void CallFBInit()
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
		ProfileFetch();

		//FetchScore();
	}
	
	void ProfileFetch()
	{
		Debug.Log("Logged in. ID: " + FB.UserId);
		
		// Reqest player info and profile picture
		FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, ProfileFetchCallback);
	}
	
	void ProfileFetchCallback(FBResult response)
	{
		if (response.Error != null)
		{
			Debug.LogError(response.Error);
			// Let's just try again
			FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, ProfileFetchCallback);
			return;
		}
		
		_profile = DeserializeJSONProfile(response.Text);
		
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
				linkCaption: "I just scored " + ScoringDirector.scoringInstance.gameScore + " on the test version of Muffin Morphosis!",
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
		
		PostScore();
	}
	
	public void PostChallenge()
	{
		if(FB.IsLoggedIn)
		{
			FB.AppRequest(
				message: "Do you think you can surpass my score," + ScoringDirector.scoringInstance.gameScore + "? ",
				callback: OnPostChallengeComplete
				);
		}
	}
	
	void OnPostChallengeComplete(FBResult response)
	{
		Debug.Log("Facebook Post Challenge Compelete");
	}
	
	public void PostScore()
	{
		if(FB.IsLoggedIn)
		{
			int score = ScoringDirector.scoringInstance.gameScore;

			if(facebookScore < score)
			{			
				Dictionary<string, string> scoreData = new Dictionary<string, string>();
				scoreData.Add("score", score.ToString());
				
				FB.API ("/me/scores", HttpMethod.POST, OnPostScoreComplete, scoreData);
			}
		}
	}
	
	void OnPostScoreComplete(FBResult response)
	{
		Debug.Log ("Post score on Facebook complete");
		
		FetchScore();
	}
	
	public void FetchScore()
	{
		if(FB.IsLoggedIn)
		{
			FB.API("/app/scores?fields=score,user.limit(5)", Facebook.HttpMethod.GET, OnFetchScoreComplete);
		}
	}
	
	void OnFetchScoreComplete(FBResult response)
	{
		if (response.Error != null)
		{
			Debug.LogError(response.Error);
			return;
		}
		
		_fetchedScores = new List<object>();
		List<object> scoresList = DeserializeScores(response.Text);
		
		foreach(object score in scoresList) 
		{
			var entry = (Dictionary<string,object>) score;

			var user = (Dictionary<string,object>) entry["user"];
			
			string userId = (string)user["id"];
			
			if (string.Equals(userId,FB.UserId))
			{
				// This entry is the current player
				int playerScore = getScoreFromEntry(entry);

				facebookScore = playerScore;				
			}
		
			_fetchedScores.Add(entry);
		}
		
		// Now sort the entries based on score
		_fetchedScores.Sort(
			delegate(object firstObj,object secondObj)
			{
				return -getScoreFromEntry(firstObj).CompareTo(getScoreFromEntry(secondObj));
			}
		);

		if(friendsHighScore == null)
		{
			friendsHighScore = new Dictionary<string, int>();
		}
		else
		{
			friendsHighScore.Clear();
		}

		foreach(object score in _fetchedScores)
		{
			var entry = (Dictionary<string,object>) score;

			var user = (Dictionary<string,object>) entry["user"];
			
			string userName = (string)user["name"];

			int userScore = getScoreFromEntry(entry);

			friendsHighScore.Add(userName, userScore);
		}
	}
	
	private int getScoreFromEntry(object obj)
	{
		Dictionary<string,object> entry = (Dictionary<string,object>) obj;
		return Convert.ToInt32(entry["score"]);
	}
	
	public static List<object> DeserializeScores(string response) 
	{
		
		var responseObject = Json.Deserialize(response) as Dictionary<string, object>;
		object scoresh;
		var scores = new List<object>();
		if (responseObject.TryGetValue ("data", out scoresh)) 
		{
			scores = (List<object>) scoresh;
		}
		
		return scores;
	}
	#endregion
}
