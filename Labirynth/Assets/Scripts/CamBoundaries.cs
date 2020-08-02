using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBoundaries : MonoBehaviour {

	[SerializeField]
	private Transform map = null;


	// Use this for initialization
	void Start () {
		float tempX = map.position.x + 6.4f;
		float tempY = map.position.y - 6.4f;
		transform.position = new Vector3 (tempX, tempY, map.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
