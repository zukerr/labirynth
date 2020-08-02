using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction 
{
	Right,
	Down,
	Left,
	Up
}

public class PlayerBehaviour : MonoBehaviour {

	[SerializeField]
	private GameObject cam = null;

	[SerializeField]
	private int myX;

	[SerializeField]
	private int myY;

	[SerializeField]
	private Tile currentTile;

	[SerializeField]
	private GridGenerator mapReference = null;

	[SerializeField]
	private AudioBase audioFunctions = null;

	private Rigidbody2D body;

	private bool isMoving = false;

	private bool carryOrb = false;
	private Objective myOrb = null;

    private bool normalMovementEnabled = true;

    private void DisableNormalMovement()
    {
        normalMovementEnabled = false;
        GetComponent<PlayerMovement>().enabled = false;
    }

    public void SetupPlayerForSingleMapMode(GridGenerator mapRef)
    {
        DisableNormalMovement();
        mapReference = mapRef;
    }

	public void Teleport(Tile tile)
	{
		currentTile = tile;
		transform.position = tile.middle;
		cam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
		myX = tile.tileX;
		myY = tile.tileY;
	}

	public Tile GetTileInDirection(Direction dir)
	{
		Tile _targetTile = null;
		switch (dir) 
		{
		case Direction.Right:
			_targetTile = mapReference.getTargetTile (myX + 1, myY);
			break;
		case Direction.Down:
			_targetTile = mapReference.getTargetTile (myX, myY + 1);
			break;
		case Direction.Left:
			_targetTile = mapReference.getTargetTile (myX - 1, myY);
			break;
		case Direction.Up:
			_targetTile = mapReference.getTargetTile (myX, myY - 1);
			break;
		}
		return _targetTile;
	}

	public void Move(Direction direction)
	{
		isMoving = true;
		Tile targetTile = GetTileInDirection(direction);
		if (targetTile != null) 
		{
			if (targetTile.isOpen) 
			{
				switch (direction)
				{
				case Direction.Right:
					myX += 1;
					break;
				case Direction.Down:
					myY += 1;
					break;
				case Direction.Left:
					myX -= 1;
					break;
				case Direction.Up:
					myY -= 1;
					break;
				}

				float x = targetTile.middle.x;
				float y = targetTile.middle.y;
				StartCoroutine (MoveCoroutine (x, y, targetTile));
			} 
			else 
			{
				isMoving = false;
			}
		} 
		else 
		{
			isMoving = false;
		}

	}
	public void GatherOrb()
	{
		if (currentTile.carryingOrb) 
		{
			if (carryOrb) {
				mapReference.SpawnOrbByColor (myOrb.myColor);
			}
			myOrb = currentTile.objective;
			carryOrb = true;
			Destroy (currentTile.gameObject.transform.GetChild (0).gameObject);
			currentTile.carryingOrb = false;
		}
	}
	public void CompleteOrb()
	{
		if (currentTile.carryingRune) 
		{
			if (carryOrb) 
			{
				if (myOrb.myColor == currentTile.objective.myColor) 
				{
					myOrb = null;
					carryOrb = false;
					Destroy (currentTile.gameObject.transform.GetChild (0).gameObject);
					currentTile.carryingRune = false;
					mapReference.nonStaticRunesCompleted++;
					if (mapReference.nonStaticRunesCompleted == mapReference.getAmountOfOrbs ()) 
					{
						audioFunctions.playSoundGameComplete ();
					}
				}
			}
		}
	}

	public IEnumerator MoveCoroutine(float _x, float _y, Tile targ)
	{
		Vector2 target = new Vector2 (_x, _y);
		Vector2 moveVector = new Vector2 (target.x - transform.position.x, target.y - transform.position.y);
		//Debug.Log ("Move Vector: " + moveVector.x + " " + moveVector.y);
		while (Mathf.Abs(target.magnitude - body.position.magnitude) > 0.02f)
		{
			body.MovePosition (body.position + moveVector * Time.deltaTime * 1.0f);
			yield return null;
		}
		body.MovePosition (new Vector2 (_x, _y));
		currentTile = targ;
		CompleteOrb ();
		isMoving = false;
	}

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update ()
	{
        if(!normalMovementEnabled)
        {
            if (!isMoving)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    Move(Direction.Right);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    Move(Direction.Down);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    Move(Direction.Left);
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    Move(Direction.Up);
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Application.Quit();
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    GatherOrb();
                }
            }
        }
	}
}
