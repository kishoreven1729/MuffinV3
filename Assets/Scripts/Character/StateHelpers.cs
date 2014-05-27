#region References
using UnityEngine;
using System.Collections;
#endregion

public class StateHelpers 
{
	#region Const Variables
	public const float characterMovementThreshold 		= 0.1f;
	public const float characterMovementSpeed		 	= 16.0f;
	public const float characterTurningStrength 		= 1000.0f;
	#endregion

	#region Common Input Methods
	public static Vector3 MovementInput()
	{
		Vector3 characterMovementDirection = Vector3.zero;
		
		characterMovementDirection = new Vector3 (Input.acceleration.x, 0, Input.acceleration.y);
		
		/*For Testing*/
		if(Input.GetKey(KeyCode.A))
		{
			characterMovementDirection.x = -1.0f;
		}
		if(Input.GetKey(KeyCode.D))
		{
			characterMovementDirection.x = 1.0f;
		}
		if(Input.GetKey(KeyCode.W))
		{
			characterMovementDirection.z = 1.0f;;
		}
		if(Input.GetKey(KeyCode.S))
		{
			characterMovementDirection.z = -1.0f;
		}
		
		/*Threshold Testing*/
		if(Vector3.Distance(Vector3.zero, characterMovementDirection) > characterMovementThreshold)
		{
			return characterMovementDirection;
		}
		
		return Vector3.zero;
	}
	
	public static bool PowerupInput()
	{
		if(Input.GetButtonDown("Fire1"))
		{
			Vector2 touchPosition = Input.GetTouch(0).position;
			
			if(touchPosition.x > Screen.width / 2)
			{
				return true;
			}
		}
		
		if(Input.GetKeyDown(KeyCode.E))
		{
			return true;
		}

		return false;
	}
	
	public static bool TrapInput()
	{
		if(Input.GetButtonDown("Fire1"))
		{
			Vector2 touchPosition = Input.GetTouch(0).position;
			
			if(touchPosition.x < Screen.width / 2)
			{
				return true;
			}
		}
		
		if(Input.GetKeyDown(KeyCode.Space))
		{
			return true;
		}
		
		return false;
	}
	#endregion


	#region Common Collision Methods
	public static bool EnemyCollisionCheck(Collider collidee, bool isPowerup = false)
	{
		if(collidee.CompareTag("Enemy"))
		{
			if(isPowerup == true)
			{
				collidee.transform.SendMessage("KillByTrap", SendMessageOptions.DontRequireReceiver);
			}
			
			return true;
		}

		return false;
	}
	#endregion
}
