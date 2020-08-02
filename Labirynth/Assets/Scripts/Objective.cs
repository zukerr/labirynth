using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Coloor
{
	Red,
	Green,
	Blue
}

public enum ObjectiveType
{
	Orb,
	Rune
}

public class Objective : MonoBehaviour {

	public Coloor myColor;
	public ObjectiveType myType;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
