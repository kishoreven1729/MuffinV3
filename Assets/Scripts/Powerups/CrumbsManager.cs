#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class CrumbsManager : MonoBehaviour 
{
	#region Private Variables
	private Dictionary<string, Transform> _crumbsCollection;
	#endregion

	#region Public Variables
	public Transform	crumbPrefab;

	public int			numberOfCrumbs;

	public static CrumbsManager crumbsInstance;
	#endregion

	#region Constructor
	void Awake()
	{
		crumbsInstance = this;
	}

	void Start()
	{
		_crumbsCollection = new Dictionary<string, Transform>();
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

		for(int index = 0; index < numberOfCrumbs; index ++)
		{
			string name = "Crumbs_" + index;

			try
			{
				Vector3 localPosition = transform.position;

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


				Transform crumb = Instantiate(crumbPrefab, localPosition, Quaternion.AngleAxis(Random.Range(0.0f, 180.0f), Vector3.up)) as Transform;
				crumb.name = name;
				_crumbsCollection.Add(name, crumb);
			}
			catch(System.Exception ex)
			{
				Debug.Log("CrumbsManager-SprinkleCrumbs: \n" + ex.Message);
			}
		}
	}

	public void RemoveCrumb(string name)
	{
		try
		{
			_crumbsCollection.Remove(name);
		}
		catch(System.Exception ex)
		{
			Debug.Log("CrumbsManager-RemoveCrumb: \n" + ex.Message);
		}
	}

	public void PauseCrumbs()
	{
		foreach(Transform crumb in _crumbsCollection.Values)
		{
			crumb.SendMessage("PauseCrumbsTimer", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ResumeCrumbs()
	{
		foreach(Transform crumb in _crumbsCollection.Values)
		{
			crumb.SendMessage("ResumeCrumbsTimer", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void DestroyAllCrumbs()
	{
		try
		{
			foreach(Transform crumb in _crumbsCollection.Values)
			{
				Destroy(crumb.gameObject);
			}

			_crumbsCollection.Clear();
		}
		catch(System.Exception ex)
		{
			Debug.Log("CrumbsManager-DestroyAllCrumbs: \n" + ex.Message); 
		}
	}
	#endregion
}
