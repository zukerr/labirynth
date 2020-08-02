using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMap : MonoBehaviour {

	[SerializeField]
	private GameObject player = null;
	[SerializeField]
	private GameObject mapGeneratorPrefab = null;
	[SerializeField]
	private int numberOfMapColumns = 0;
	[SerializeField]
	private int numberOfMapRows = 0;
	[SerializeField]
	private EdgeCollider2D phisicalCollider = null;
	[SerializeField]
	private BoxCollider2D cameraCollider = null;
    
    private bool singleMapMode = false;

	private GameObject[,] grids = null;
	private GridGenerator mainGrid;

	private float bigMapLength;
	private float bigMapWidth;

	private void MakeBigMap()
	{
		grids = new GameObject[numberOfMapColumns,numberOfMapRows];
		GridGenerator mapGen = mapGeneratorPrefab.GetComponent<GridGenerator> ();
		float singleMapLength = mapGen.getTileLength() * mapGen.getNumberOfColumns();
		float singleMapWidth = mapGen.getTileLength() * mapGen.getNumberOfRows();
		Vector3 tempPos = transform.position;
		for (int i = 0; i < numberOfMapRows; i++) 
		{
			for (int j = 0; j < numberOfMapColumns; j++) 
			{
				grids [i, j] = Instantiate (mapGeneratorPrefab, tempPos, transform.rotation, transform);
				tempPos.x += singleMapLength;
			}
			tempPos.x = transform.position.x;
			tempPos.y -= singleMapWidth;
		}
		mainGrid = grids [0, 0].GetComponent<GridGenerator> ();
		bigMapLength = numberOfMapColumns * singleMapLength;
		bigMapWidth = numberOfMapRows * singleMapWidth;
		//cameraCollider.size.x = bigMapLength;
		//cameraCollider.size.y = bigMapWidth;
		cameraCollider.size = new Vector2 (bigMapLength, bigMapWidth);
		cameraCollider.gameObject.transform.position = new Vector3 (transform.position.x + (bigMapLength / 2), transform.position.y - (bigMapWidth / 2));
		Vector2[] edges = new Vector2[5];
		edges[0] = new Vector2(0, 0);
		edges[1] = new Vector2(bigMapLength, 0);
		edges[2] = new Vector2(bigMapLength, -bigMapWidth);
		edges[3] = new Vector2(0, -bigMapWidth);
		edges[4] = new Vector2(0, 0);
		phisicalCollider.points = edges;
	}

	private IEnumerator PlacePlayer()
	{
		while (!mainGrid.getSetupComplete ()) 
		{
			yield return null;
		}
		player.GetComponent<PlayerBehaviour>().Teleport(mainGrid.getFirstOpenTile());
		Debug.Log ("Placing player.");
	}

    private void CheckForSingleMapMode()
    {
        if(numberOfMapColumns == 1)
        {
            if(numberOfMapRows == 1)
            {
                singleMapMode = true;
            }
        }
    }

    private void SetupSingleMapMode()
    {
        if(singleMapMode)
        {
            player.GetComponent<PlayerBehaviour>().SetupPlayerForSingleMapMode(mainGrid);
        }
    }

	// Use this for initialization
	void Start ()
    {
        CheckForSingleMapMode();
		MakeBigMap ();
        SetupSingleMapMode();
		StartCoroutine (PlacePlayer ());
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
