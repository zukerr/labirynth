using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform CamTarget;
	private Transform Player;
	public Vector2
	Margin,
	Smoothing;
	public BoxCollider2D Bounds;
	private Vector3
	_min,
	_max;
	public bool IsFollowing { get; set; }
	private Camera mycam;
	public float CameraSize = 3f;

	public Transform middle;

	// Use this for initialization
	public void Start () {

		mycam = GetComponent<Camera> ();
		IsFollowing = true;
		Player = CamTarget;
		transform.position = new Vector3 (Player.position.x, Player.position.y, transform.position.z);

	}

	// Update is called once per frame
	public void Update () {


		_min = Bounds.bounds.min;
		_max = Bounds.bounds.max;
		var x = transform.position.x;
		var y = transform.position.y;


		if (IsFollowing) {

			if (Mathf.Abs (x - CamTarget.position.x) > Margin.x)
				x = Mathf.Lerp (x, CamTarget.position.x, Smoothing.x * Time.deltaTime);

			if (Mathf.Abs (y - CamTarget.position.y) > Margin.y)
				y = Mathf.Lerp (y, CamTarget.position.y, Smoothing.y * Time.deltaTime);
		}

		var cameraHalfWidth = mycam.orthographicSize * ((float) Screen.width / Screen.height);

		x = Mathf.Clamp (x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
		y = Mathf.Clamp (y, _min.y + mycam.orthographicSize, _max.y - mycam.orthographicSize);

		transform.position = new Vector3 (x, y, transform.position.z);
	}


	public void ZoomCamOut()
	{
		CamTarget = middle;
		GetComponent<Animator> ().SetBool ("zoomed", true);
	}

	public void ZoomCamIn()
	{
		CamTarget = Player;
		GetComponent<Animator> ().SetBool ("zoomed", false);
	}
}
