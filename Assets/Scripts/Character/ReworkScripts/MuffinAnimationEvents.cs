#region References
using UnityEngine;
using System.Collections;
#endregion

public class MuffinAnimationEvents : MonoBehaviour 
{
	#region Private Variables
	private MuffinControl _muffinControl;
	#endregion

	#region Constructor
	void Start()
	{
		_muffinControl = GameObject.FindGameObjectWithTag("Player").GetComponent<MuffinControl>();
	}
	#endregion

	#region Animation Events
	public void SendAnimationEvent()
	{
		_muffinControl.OnAnimationEvent();
	}
	#endregion
}
