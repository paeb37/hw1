using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fruits_tester : MonoBehaviour {

	public GameObject[] fruits;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{	
		// simplified the code using non-deprecated method
		for (int i = 0; i < fruits.Length; i++)
		{
			float direction = (i % 2 == 0) ? 1f : -1f;
			fruits[i].transform.Rotate(Vector3.up * Time.deltaTime * 360f * direction);
		}
	}
}
