#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class CrumbsManager : MonoBehaviour 
{
	#region Private Variables
	private Transform		_crumbGroup;
	private List<Transform> _crumbsList;
	private int				_crumbsCount;

	private float			_crumbMaxX;
	private float			_crumbMinX;
	private float			_crumbMaxZ;
	private float			_crumbMinZ;
	#endregion

	#region Public Variables
	public static CrumbsManager crumbsInstance;
	#endregion

	#region Constructor
	void Awake()
	{
		crumbsInstance = this;
	}

	void Start()
	{
		_crumbsList = new List<Transform>();

		try
		{
			_crumbGroup = GameObject.FindGameObjectWithTag("CrumbsPowerup").transform;

			_crumbsCount = _crumbGroup.childCount;

			for(int index = 0; index < _crumbsCount; index++)
			{
				_crumbsList.Add(_crumbGroup.GetChild(index));
			}

			DestroyAllCrumbs();
		}
		catch(System.Exception ex)
		{
			Debug.Log("CrumbsManager-Start: \n" + ex.Message);
		}

		_crumbMinX = 1.0f;
		_crumbMaxX = 2.0f;

		_crumbMinZ = 1.0f;
		_crumbMaxZ = 2.0f;
	}
	#endregion
	
	#region Loop
	void Update()
	{
	
	}
	#endregion

	#region Methods
	public void SprinkleCrumbs()
	{
		DestroyAllCrumbs();

		//_crumbGroup.gameObject.SetActive(true);

		Vector3 groupPosition = transform.position;

		_crumbGroup.position = groupPosition;

		for(int index = 0; index < _crumbsCount; index ++)
		{
			try
			{
				Vector3 localPosition = Vector3.zero;

				int quadrant = (index) / 2 % 4;

				float xMin = _crumbMinX, xMax = _crumbMaxX;
				float zMin = _crumbMinZ, zMax = _crumbMaxZ;

				switch(quadrant)
				{
				case 0:
					{						
						break;
					}
				case 1:
					{
						xMin = -_crumbMaxX;
						xMax = -_crumbMinX;
							
						zMin = _crumbMinZ;
						zMax = _crumbMaxZ;
						break;
					}
				case 2:
					{
						xMin = -_crumbMaxX;
						xMax = -_crumbMinX;
							
						zMin = -_crumbMaxZ;
						zMax = -_crumbMinZ;
						break;
					}
				case 3:
					{
						xMin = _crumbMinX;
						xMax = _crumbMaxX;
						
						zMin = -_crumbMaxZ;
						zMax = -_crumbMinZ;
						break;
					}
				}

				localPosition.x = Random.Range(xMin, xMax);
				localPosition.z = Random.Range(zMin, zMax);

				_crumbsList[index].gameObject.SetActive(true);

				//_crumbsList[index].localPosition = localPosition;

				localPosition.y = 1.0f;

				localPosition.Normalize();

				_crumbsList[index].rigidbody.AddForce(localPosition * 500.0f);
			}
			catch(System.Exception ex)
			{
				Debug.Log("CrumbsManager-SprinkleCrumbs: \n" + ex.Message);
			}
		}

		StartCoroutine(CrumbsExpiry());
	}

	public void RemoveCrumb(string name)
	{
		/*try
		{
			_crumbsCollection.Remove(name);
		}
		catch(System.Exception ex)
		{
			Debug.Log("CrumbsManager-RemoveCrumb: \n" + ex.Message);
		}*/
	}

	public void PauseCrumbs()
	{
		foreach(Transform crumb in _crumbsList)
		{
			crumb.SendMessage("PauseCrumbsTimer", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ResumeCrumbs()
	{
		foreach(Transform crumb in _crumbsList)
		{
			crumb.SendMessage("ResumeCrumbsTimer", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void DestroyAllCrumbs()
	{
		for(int index = 0; index < _crumbsCount; index ++)
		{
			if(_crumbsList[index].gameObject.activeSelf == true)
			{
				_crumbsList[index].gameObject.SetActive(false);
			}
		}
	}
	#endregion

	#region Coroutines
	public IEnumerator CrumbsExpiry()
	{
		yield return new WaitForSeconds(5.0f);

		DestroyAllCrumbs();
	}
	#endregion
}
