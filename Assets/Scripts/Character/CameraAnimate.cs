#region References
using UnityEngine;
using System.Collections;
#endregion

public class CameraAnimate : MonoBehaviour 
{
	#region Private Variables
	private Vector3				_animationFinalLocation;
	private Quaternion			_animationFinalRotation;

	private float				_animationStartTime;
	private float				_animationEndTime;

	private bool				_playAnimation;
	private bool				_animationType;				/* True = Initial, False = Death */

	private bool				_callbackCalled;
	#endregion

	#region Public Variables
	public static CameraAnimate cameraAnimationInstance;
	#endregion

	#region Constructor
	void Awake()
	{
		cameraAnimationInstance = this;
	}

	void Start() 
	{
		_playAnimation = false;

		_callbackCalled = false;
	}
	#endregion
	
	#region Loop
	void Update()
	{
		if(_playAnimation == true)
		{
			float factor = (Time.time - _animationStartTime)/_animationEndTime;

			factor = Mathf.Clamp(factor, 0.0f, 1.0f);

			transform.rotation = Quaternion.Slerp(transform.rotation, _animationFinalRotation, factor);

			transform.position = Vector3.Slerp(transform.position, _animationFinalLocation, factor);

			if(_callbackCalled == false)
			{
				if(Vector3.Distance(transform.position, _animationFinalLocation) < 2.0f && Quaternion.Angle(transform.rotation, _animationFinalRotation) < 10.0f)
				{
					if(CameraAnimationEnded != null)
					{
						_callbackCalled = true;

						CameraAnimationEnded(_animationType);
					}
				}
			}

			if(transform.position == _animationFinalLocation && transform.rotation == _animationFinalRotation)
			{
				_playAnimation = false;

				_callbackCalled = false;
			}
		}
	}
	#endregion

	#region Methods
	public void PlayCameraAnimation(Vector3 finalLocation, Quaternion finalRotation, float animationTime, bool isGameStartAnimation)
	{
		_animationFinalLocation = finalLocation;
		_animationFinalRotation = finalRotation;

		_playAnimation = true;

		_animationStartTime = Time.time;

		//_animationEndTime = _animationStartTime + animationTime;
		_animationEndTime = animationTime;

		_animationType = isGameStartAnimation;
	}
	#endregion

	#region Events
	/*Create Event for Animation End*/
	public delegate void CameraAnimationEndedHandler(bool isGameStartAnimation);

	public static event CameraAnimationEndedHandler CameraAnimationEnded;
	#endregion
}
