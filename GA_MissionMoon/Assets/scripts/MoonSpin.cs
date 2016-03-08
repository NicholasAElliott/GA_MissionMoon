using UnityEngine;
using System.Collections;

public class MoonSpin : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public float speed = 10f;


	void Update ()
	{
		transform.Rotate(Vector3.back, speed * Time.deltaTime);
	}
}
