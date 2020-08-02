using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public BoxCollider2D _collider;
	public int tileX;
	public int tileY;
	public int ID;
	public int mapperID = -1;
	public Vector3 middle;
	public bool isOpen = true;
	public bool carryingOrb = false;
	public bool carryingRune = false;
	public Objective objective = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
