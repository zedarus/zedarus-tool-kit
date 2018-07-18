using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTKTestScript : MonoBehaviour {

	[SerializeField]
	private GameObject _prefab;
	[SerializeField]
	private Transform _target;
	[SerializeField]
	private bool _autoCreate;

	private void Start () 
	{
		if (_autoCreate)
		{
			Create();
		}
	}

	public void Create()
	{
		GameObject go = Instantiate(_prefab);
		go.transform.position = _target.position;
	}
}
