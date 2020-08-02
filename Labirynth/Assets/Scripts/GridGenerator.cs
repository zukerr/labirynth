using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour {

	//[SerializeField]
	//private GameObject player = null;

	[SerializeField]
	private GameObject objectivePrefab = null;
	[SerializeField]
	private Sprite redOrbSprite = null;
	[SerializeField]
	private Sprite greenOrbSprite = null;
	[SerializeField]
	private Sprite blueOrbSprite = null;
	[SerializeField]
	private Sprite redRuneSprite = null;
	[SerializeField]
	private Sprite greenRuneSprite = null;
	[SerializeField]
	private Sprite blueRuneSprite = null;

	[SerializeField]
	private Material orbMaterial = null;
	[SerializeField]
	private Material runeMaterial = null;

	[SerializeField]
	private Sprite closedSprite = null;

	[SerializeField]
	private Sprite openSprite = null;

	[SerializeField]
	private GameObject tilePrefab = null;

	[SerializeField]
	private int numberOfRows = 0;
	public int getNumberOfRows()
	{
		return this.numberOfRows;
	}

	[SerializeField]
	private int numberOfColumns = 0;
	public int getNumberOfColumns()
	{
		return this.numberOfColumns;
	}

	[SerializeField]
	private float tileLength = 0;
	public float getTileLength()
	{
		return this.tileLength;
	}

	private float baseLength = 0.32f;

	private Tile[,] tileTab;
	//private Tile[] openTileTab;

	[SerializeField]
	private int amountOfOrbs = 3;

	public int getAmountOfOrbs()
	{
		return amountOfOrbs;
	}

	private Tile[] orbSpawns;
	private Tile [] runeSpawns;

	private int wallLength;

	//legacy statics
	public int nonStaticTileCounter = 0;
	public int nonStaticOpenTileCounter = 0;
	public int nonStaticCleanTileCounter = 0;
	public int nonStaticRunesCompleted = 0;

	private bool setupComplete = false;
	public bool getSetupComplete()
	{
		return this.setupComplete;
	}

	public Tile getTargetTile(int x, int y)
	{
		if ((x < numberOfColumns) && (y < numberOfRows) && (x >= 0) && (y >= 0)) 
		{
			return tileTab [x, y];
		} 
		else 
		{
			//Debug.Log ("You just reffered to nonexistent Tile!!");
			return null;
		}
	}
	private Tile[] Get8SurroundingTiles (int x, int y)
	{
		Tile[] surroundings = new Tile[8];

		//normal case
		surroundings[0] = getTargetTile(x-1, y-1);
		surroundings[1] = getTargetTile(x, y-1);
		surroundings[2] = getTargetTile(x+1, y-1);
		surroundings[3] = getTargetTile(x-1, y);
		surroundings[4] = getTargetTile(x+1, y);
		surroundings[5] = getTargetTile(x-1, y+1);
		surroundings[6] = getTargetTile(x, y+1);
		surroundings[7] = getTargetTile(x+1, y+1);

		int emptyTileIndex = FirstNullTile (surroundings);
		Tile[] surr2;
		while (emptyTileIndex != -1) 
		{
			surr2 = new Tile[surroundings.Length - 1];
			for (int i = emptyTileIndex; i < (surroundings.Length - 1); i++) 
			{
				surroundings [i] = surroundings [i + 1];
			}
			for (int i = 0; i < surr2.Length; i++) 
			{
				surr2 [i] = surroundings [i];
			}
			surroundings = surr2;
			emptyTileIndex = FirstNullTile (surroundings);
		}
		return surroundings;
	}
	private int FirstNullTile(Tile[] tab)
	{
		for (int i = 0; i < tab.Length; i++) 
		{
			if (tab [i] == null) 
			{
				return i;
			}
		}
		return -1;
	}
	private bool ClosedTileInSurroundingExists(Tile aTile)
	{
		if (aTile != null) 
		{
			int x = aTile.tileX;
			int y = aTile.tileY;
			Tile[] surr = Get8SurroundingTiles (x, y);
			foreach (Tile t in surr) 
			{
				if (!t.isOpen) 
				{
					return true;
				}
			}
			return false;
		}
		else 
		{
			Debug.LogError ("You passed a null Tile to a function!");
			return true;
		}
	}
	private bool Phase1GenerateClosedTile(Tile tile, bool _horizontal = true)
	{
		int percentageChance = 50;
		if (!ClosedTileInSurroundingExists (tile)) 
		{
			if (!_horizontal) 
			{
				return true;
			}
			int chance = Random.Range ((int)1, (int)101);
			if (chance <= percentageChance) 
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else 
		{
			return false;
		}
	}

	private void GenerateLabirynth1(bool horizontal = true)
	{
		wallLength = Mathf.FloorToInt ((float)numberOfColumns * 0.2f);
		int tempj;
		int firstDimension;
		int secondDimension;
		if (horizontal) 
		{
			firstDimension = numberOfRows;
			secondDimension = numberOfColumns;
		}
		else 
		{
			firstDimension = numberOfColumns;
			secondDimension = numberOfRows;
		}
		for (int i = 0; i < firstDimension; i++) 
		{
			for (int j = 0; j < secondDimension; j++) 
			{
				if (Phase1GenerateClosedTile (getTargetTile (j, i), horizontal))
				{
					tempj = j;
					for (int k = 1; k < wallLength; k++)
					{
						if (getTargetTile (j + k, i) != null) 
						{
							if (Phase1GenerateClosedTile (getTargetTile (j + k, i), horizontal))
							{
								SetTileClosed (getTargetTile (j + k, i));
								j++;
							}
						}
					}
					SetTileClosed (getTargetTile (tempj, i));
				}
			}
		}
	}

	private void SetTileClosed(Tile tile)
	{
		if (tile.isOpen) 
		{
			tile.isOpen = false;
			tile.gameObject.GetComponent<SpriteRenderer> ().sprite = closedSprite;
			tile._collider.enabled = true;
			nonStaticOpenTileCounter--;
		}
	}
	private void SetTileOpen(Tile tile)
	{
		if (!tile.isOpen) 
		{
			tile.isOpen = true;
			tile.gameObject.GetComponent<SpriteRenderer> ().sprite = openSprite;
			tile._collider.enabled = false;
			nonStaticOpenTileCounter++;
		}
	}

	private Tile[] getTabOfOpenTiles()
	{
		//Debug.Log ("Number of open tiles: " + Machine.openTileCounter);
		Tile[] openTiles = new Tile[nonStaticOpenTileCounter];
		int k = 0;
		for (int i = 0; i < numberOfRows; i++) 
		{
			for (int j = 0; j < numberOfColumns; j++) 
			{
				if (getTargetTile (j, i).isOpen) 
				{
					openTiles [k] = getTargetTile (j, i);
					k++;
				}
			}
		}
		return openTiles;
	}
	public Tile getFirstOpenTile()
	{
		Tile[] temp = getTabOfOpenTiles ();
		return temp [0];
	}
	private bool getTargetTileBool(int x, int y)
	{
		if (getTargetTile (x, y) != null)
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}

	private void testPathfinding()
	{
		Tile[] opens = getTabOfOpenTiles ();
		int randStartIndex = Random.Range ((int)0, (int)nonStaticOpenTileCounter);
		int randFinishIndex = Random.Range ((int)0, (int)nonStaticOpenTileCounter);
		Tile randStart = opens [randStartIndex];
		Tile randFinish = opens [randFinishIndex];
		Debug.Log ("Randomly generated start is at: x:" + randStart.tileX + " y:" + randStart.tileY);
		Debug.Log ("Randomly generated finish is at: x:" + randFinish.tileX + " y:" + randFinish.tileY);
		//Debug.Log ("Its possible to get from start to finish: " + canYouGetFromAtoB_main(randStart, randFinish));
		Debug.Log ("Its possible to get to every open tile: " + mapOpenTiles(randStart));
	}

	private Tile[] borderingTab(Tile t)
	{
		Tile rightTile;
		Tile downTile;
		Tile leftTile;
		Tile upTile;
		rightTile = getTargetTile (t.tileX + 1, t.tileY);
		downTile = getTargetTile (t.tileX, t.tileY + 1);
		leftTile = getTargetTile (t.tileX - 1, t.tileY);
		upTile = getTargetTile (t.tileX, t.tileY - 1);
		Tile[] tab1 = new Tile[] { rightTile, downTile, leftTile, upTile };
		return tab1;
	}
	private bool squareTile(Tile t)
	{
		if (t != null) 
		{
			if (t.isOpen) 
			{
				if ((getTargetTileBool (t.tileX + 1, t.tileY)) && (getTargetTile (t.tileX + 1, t.tileY).isOpen)) 
				{
					if ((getTargetTileBool (t.tileX, t.tileY + 1)) && (getTargetTile (t.tileX, t.tileY + 1).isOpen))
					{
						if ((getTargetTileBool (t.tileX + 1, t.tileY + 1)) && (getTargetTile (t.tileX + 1, t.tileY + 1).isOpen))
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}
	private Tile[] squareTileTab(Tile t)
	{
		if (squareTile (t))
		{
			Tile[] tab = new Tile[] 
			{
				t, 
				getTargetTile (t.tileX + 1, t.tileY), 
				getTargetTile (t.tileX, t.tileY + 1),
				getTargetTile (t.tileX + 1, t.tileY + 1)
			};
			return tab;
		} 
		else 
		{
			return null;
		}
	}
	private void removeSquares()
	{
		Tile[] opens = getTabOfOpenTiles ();
		int rngMapField;
		bool flag;
		Tile[] temp = new Tile[4];
		foreach (Tile t in opens) 
		{
			if (squareTile (t)) 
			{
				flag = false;
				temp = squareTileTab (t);
				while (!flag) 
				{
					rngMapField = Random.Range ((int)0, (int)4);
					SetTileClosed (temp [rngMapField]);
					flag = mapOpenTiles (getFirstOpenTile());
					if (!flag) 
					{
						SetTileOpen (temp[rngMapField]);
					}
				}
			}
		}
	}
	private bool mapOpenTiles(Tile start)
	{
		Tile[] opens = getTabOfOpenTiles ();
		foreach (Tile _t in opens) 
		{
			_t.mapperID = -1;
		}
		nonStaticCleanTileCounter = nonStaticOpenTileCounter;
		//Debug.Log ("Count of clean tiles:" + nonStaticCleanTileCounter);
		//Debug.Log ("Count of open tiles:" + nonStaticOpenTileCounter);
		int counter = 0;
		start.mapperID = counter;
		nonStaticCleanTileCounter--;
		Tile[] tempp = new Tile[4];
		tempp = borderingTab (start);
		counter++;
		foreach (Tile t in tempp) 
		{
			if (t != null)
			{
				if(t.isOpen)
				{
					if(t.mapperID == -1)
					{
						t.mapperID = counter;
						//Debug.Log ("x:" + t.tileX + " y:" + t.tileY + " mapperID:" + t.mapperID);
						nonStaticCleanTileCounter--;
					}
				}
			}
		}
		for(int i = 0; i < nonStaticOpenTileCounter; i++)
		{
			foreach (Tile t in opens) 
			{
				//Debug.Log (t.mapperID + " " + x);
				if (t.mapperID == counter) 
				{
					//Debug.Log ("Entered the first if for counter:" + t.mapperID);
					//tempp = new Tile[4];
					tempp = borderingTab (t);
					foreach (Tile tt in tempp) 
					{
						//Debug.Log ("Foreach tt iteration");
						if (tt != null) 
						{
							if (tt.isOpen) 
							{
								if (tt.mapperID == -1) 
								{
									tt.mapperID = counter + 1;
									//Debug.Log ("x:" + tt.tileX + " y:" + tt.tileY + " mapperID:" + tt.mapperID);
									nonStaticCleanTileCounter--;
									//Debug.Log ("Count of clean tiles:" + nonStaticCleanTileCounter);
								}
							}
						}
					}
				}
			}
			counter++;
		}

		if (nonStaticCleanTileCounter == 0) 
		{
			return true;
		} 
		else
		{
			return false;
		}
	}
		
	public void SpawnOrbByColor(Coloor _clr)
	{
		int colorInteger = 0;
		GameObject temp;
		Sprite tempSprite = null;
		switch (_clr)
		{
		case Coloor.Red:
			colorInteger = 0;
			tempSprite = redOrbSprite;
			break;
		case Coloor.Green:
			colorInteger = 1;
			tempSprite = greenOrbSprite;
			break;
		case Coloor.Blue:
			colorInteger = 2;
			tempSprite = blueOrbSprite;
			break;
		}
		temp = Instantiate (objectivePrefab, orbSpawns [colorInteger].gameObject.transform);
		temp.transform.position = orbSpawns [colorInteger].middle;
		temp.GetComponent<Objective> ().myType = ObjectiveType.Orb;
		temp.GetComponent<Objective> ().myColor = _clr;
		temp.GetComponent<SpriteRenderer> ().sprite = tempSprite;
		temp.GetComponent<SpriteRenderer> ().material = orbMaterial;
		orbSpawns [colorInteger].carryingOrb = true;
		orbSpawns [colorInteger].objective = temp.GetComponent<Objective> ();
	}

	private void SetupOrbSpawnpoints()
	{
		Tile[] opens = getTabOfOpenTiles ();
		runeSpawns = new Tile[amountOfOrbs];
		orbSpawns = new Tile[amountOfOrbs];
		int amountOfThings = 2 * amountOfOrbs;
		int[] spawnIndexes = new int[amountOfThings];
		for (int i = 0; i < amountOfThings; i++) 
		{
			spawnIndexes[i] = Random.Range ((int)0, (int)nonStaticOpenTileCounter);
			for (int j = 0; j < i; j++) 
			{
				if (spawnIndexes [i] == spawnIndexes [j]) 
				{
					j = i;
					i--;
				}
			}
		}

		for (int i = 0; i < amountOfOrbs; i++)
		{
			orbSpawns[i] = opens [spawnIndexes [i]];
			orbSpawns [i].carryingOrb = true;
			runeSpawns [i] = opens [spawnIndexes [i + amountOfOrbs]];
			runeSpawns [i].carryingRune = true;
			GameObject tempOrb = Instantiate (objectivePrefab, orbSpawns [i].gameObject.transform);
			GameObject tempRune = Instantiate (objectivePrefab, runeSpawns [i].gameObject.transform);
			tempOrb.transform.position = orbSpawns [i].middle;
			tempRune.transform.position = runeSpawns [i].middle;
			tempOrb.GetComponent<Objective> ().myType = ObjectiveType.Orb;
			tempRune.GetComponent<Objective> ().myType = ObjectiveType.Rune;
			tempOrb.GetComponent<SpriteRenderer> ().material = orbMaterial;
			tempRune.GetComponent<SpriteRenderer> ().material = runeMaterial;
			if (i == 0) 
			{
				tempOrb.GetComponent<Objective> ().myColor = Coloor.Red;
				tempRune.GetComponent<Objective> ().myColor = Coloor.Red;
				tempOrb.GetComponent<SpriteRenderer> ().sprite = redOrbSprite;
				tempRune.GetComponent<SpriteRenderer> ().sprite = redRuneSprite;
			}
			if (i == 1) 
			{
				tempOrb.GetComponent<Objective> ().myColor = Coloor.Green;
				tempRune.GetComponent<Objective> ().myColor = Coloor.Green;
				tempOrb.GetComponent<SpriteRenderer> ().sprite = greenOrbSprite;
				tempRune.GetComponent<SpriteRenderer> ().sprite = greenRuneSprite;
			}
			if (i == 2) 
			{
				tempOrb.GetComponent<Objective> ().myColor = Coloor.Blue;
				tempRune.GetComponent<Objective> ().myColor = Coloor.Blue;
				tempOrb.GetComponent<SpriteRenderer> ().sprite = blueOrbSprite;
				tempRune.GetComponent<SpriteRenderer> ().sprite = blueRuneSprite;
			}
			orbSpawns [i].objective = tempOrb.GetComponent<Objective> ();
			runeSpawns [i].objective = tempRune.GetComponent<Objective> ();
		}
	}

	private void MakeMap()
	{
		tileTab = new Tile[numberOfColumns,numberOfRows];
		Vector3 tempPosition = transform.position;
		GameObject temp;

		for (int i = 0; i < numberOfRows; i++) 
		{
			for (int j = 0; j < numberOfColumns; j++) 
			{
				temp = Instantiate (tilePrefab, tempPosition, transform.rotation, transform);
				temp.GetComponent<Tile> ().isOpen = true;
				temp.transform.localScale = ScaleVector ();
				temp.GetComponent<Tile> ().ID = nonStaticTileCounter;
				nonStaticTileCounter++;
				temp.GetComponent<Tile> ().tileX = j;
				temp.GetComponent<Tile> ().tileY = i;
				temp.GetComponent<Tile> ().middle = temp.transform.position + new Vector3 (tileLength/2, -tileLength/2);
				tileTab [j,i] = temp.GetComponent<Tile> ();
				tempPosition.x += tileLength;
			}
			tempPosition.x = transform.position.x;
			tempPosition.y -= tileLength;
		}
		nonStaticOpenTileCounter = nonStaticTileCounter;
	}

	private Vector3 ScaleVector()
	{
		Vector3 outCome = new Vector3 ();
		outCome.x = tileLength / baseLength;
		outCome.y = tileLength / baseLength;
		outCome.z = tileLength / baseLength;
		return outCome;
	}
		

	// Use this for initialization
	void Start () {
		MakeMap ();
		GenerateLabirynth1 ();
		GenerateLabirynth1 (false);
		//openTileTab = getTabOfOpenTiles ();
		removeSquares ();
		//player.transform.position = tileTab [5, 5].transform.position;
		//player.GetComponent<PlayerBehaviour>().Teleport(getFirstOpenTile());
		//testPathfinding ();
		SetupOrbSpawnpoints ();
		setupComplete = true;
		Debug.Log ("Setup Complete!");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
