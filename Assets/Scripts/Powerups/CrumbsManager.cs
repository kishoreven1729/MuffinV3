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
			_crumbGroup = transform.GetChild(0);

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

		_crumbGroup.gameObject.SetActive(true);

		for(int index = 0; index < _crumbsCount; index ++)
		{
			try
			{
				Vector3 localPosition = _crumbGroup.position;

				float rangeSide = Random.Range(0.0f, 1.0f);

				if(rangeSide > 0.5f)
				{
					localPosition.x += Random.Range(-2.0f, -1.0f);
				}
				else
				{
					localPosition.x += Random.Range(1.0f, 2.0f);
				}

				rangeSide = Random.Range(0.0f, 1.0f);

				if(rangeSide > 0.5f)
				{
					localPosition.z += Random.Range(-2.0f, -1.0f);
				}
				else
				{
					localPosition.z += Random.Range(1.0f, 2.0f);
				}

				_crumbsList[index].gameObject.SetActive(true);
				_crumbsList[index].localPosition = localPosition;
			}
			catch(System.Exception ex)
			{
				Debug.Log("CrumbsManager-SprinkleCrumbs: \n" + ex.Message);
			}
		}
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
		try
		{
			foreach(Transform crumb in _crumbsList)
			{
				crumb.gameObject.SetActive(false);
			}

			_crumbGroup.gameObject.SetActive(false);
		}
		catch(System.Exception ex)
		{
			Debug.Log("CrumbsManager-DestroyAllCrumbs: \n" + ex.Message); 
		}
	}
	#endregion
}
