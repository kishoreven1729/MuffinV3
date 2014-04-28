#region References
using UnityEngine;
using System.Collections;
#endregion

public class PowerupIndexGenerator 
{
	#region Private Variables
	private float[]			_probabilityList;
	private float			_defaultProbability;
	private float			_halfDefaultProbability;
	private float			_probabilityAdjuster;
	private int				_powerupCount;
	#endregion

	#region Constructor
	public PowerupIndexGenerator(int powerupCount) 
	{
		_powerupCount = powerupCount;

		_defaultProbability = 1.0f / powerupCount;

		_halfDefaultProbability = 0.5f * _defaultProbability;

		_probabilityAdjuster = _halfDefaultProbability / (_powerupCount - 1);

		_probabilityList = new float[powerupCount];

		ResetProbabilities();
	}
	#endregion

	#region Methods
	private void AdjustPowerupProbability(int generatedIndex)
	{
		_probabilityList[generatedIndex] += _halfDefaultProbability;

		for(int index = 0; index < _powerupCount; index++)
		{
			if(index != generatedIndex)
			{
				_probabilityList[index] -= _probabilityAdjuster;
			}

			_probabilityList[index] = 1 - _probabilityList[index];
		}
	}

	public int GeneratePowerup()
	{
		float randomProb = Random.Range(0.0f, 1.0f);

		float cummulative = 0.0f;

		for(int index = 0; index < _powerupCount; index++)
		{
			cummulative += _probabilityList[index];

			if(cummulative > randomProb)
			{
				AdjustPowerupProbability(index);

				return index;
			}
		}

		return -1;
	}

	public void ResetProbabilities()
	{
		for(int index = 0; index < _powerupCount; index++)
		{
			_probabilityList[index] = _defaultProbability;
		}
	}
	#endregion
}
