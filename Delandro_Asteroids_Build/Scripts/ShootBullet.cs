using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBullet : MonoBehaviour
{
	public float speed = 1;
	

	// The speed that the vehicle turns, in angles/s
	public Vector3 position;
	public Vector3 direction;
	public Vector3 velocity;
	public Vector3 acceleration;
	public float accelerationRate;
	public float maximumSpeed;

	private float totalCamHeight;
	private float totalCamWidth;

	// Start is called before the first frame update
	void Start()
	{
		acceleration = new Vector3(0, 0, 0);
		accelerationRate = 0.01f;
		velocity = Vector3.zero;
	}


	//Update is called once per frame
	void Update()
	{
		Move();
		SetTransform();
	}


	void Move() {
		acceleration = direction;
		velocity += acceleration;
		velocity = Vector3.ClampMagnitude(velocity, maximumSpeed);
		position += velocity;
	}


	void SetTransform() {
		// Set the position
		transform.position = position;

		// Set the rotation
		// First we want to get the angle of rotation using inverse tangent
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		// Then get a quaternion, rotated around the forward axis (the direction the camera is facing)
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
