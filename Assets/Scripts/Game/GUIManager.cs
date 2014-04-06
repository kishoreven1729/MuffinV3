#region References
using UnityEngine;
using System.Collections;
#endregion

public class GUIManager : MonoBehaviour 
{
	#region Private Variables
	#endregion

	#region Public Variables
	#endregion

	#region Constructor
	void Start()
	{
	}
	#endregion
	
	#region Loop
	void Update()
	{
	}

	void OnGUI()
	{
		if(GameDirector.gameInstance.characterLoaded == true)
		{
			GUI.Label(new Rect(10, 10, 150, 50), "Powerup: " + PowerupManager.powerupManagerInstance.availablePowerupType.ToString());

			GUI.Label(new Rect(10, 100, 150, 50), "Score: " + ScoringDirector.scoringInstance.gameScore);
		}
	}
	#endregion

	#region Methods
	#endregion
}
