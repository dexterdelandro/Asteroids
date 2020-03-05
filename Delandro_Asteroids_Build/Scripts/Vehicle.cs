using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{

	public float speed = 1;

	// The speed that the vehicle turns, in angles/s
	public float turnSpeed;
	public Camera camera;
	public Vector3 position;
	public Vector3 direction;
	public Vector3 velocity;
	public Vector3 acceleration;
	public float accelerationRate;
	public float maximumSpeed;

	private float totalCamHeight;
	private float totalCamWidth;

	public Vector3 Direction {
		get { return direction; }
	}

	public Vector3 Position {
		get { return position; }
	}

	// Start is called before the first frame update
	void Start()
	{
		acceleration = new Vector3(0, 0, 0);
		accelerationRate = 0.01f;
		totalCamHeight = camera.orthographicSize;
		totalCamWidth = totalCamHeight * camera.aspect;
		// Set the initial position to (0, 0, 0)
		position = Vector3.zero;

		// Vector3.right is shorthand for (1, 0, 0) in Unity
		direction = Vector3.right;

		// Set the initial velocity to (0, 0, 0)
		velocity = Vector3.zero;
	}

	// Update is called once per frame
	void Update()
	{
		// First we rotate the vehicle
		RotateVehicle();


		// Next, make the vehicle drive
		Drive();

		// Then, wrap the vehicle around the screen
		WrapVehicle();

		// Finally, update the position and rotation of the vehicle
		SetTransform();

	}

	/// <summary>
	/// Rotates the vehicle if the player is holding the left or right arrow keys
	/// </summary>
	private void RotateVehicle()
	{
		// Rotate the vehicle counter clockwise if the left arrow is held down
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			direction = Quaternion.Euler(0, 0, turnSpeed * Time.deltaTime) * direction;
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			direction = Quaternion.Euler(0, 0, -turnSpeed * Time.deltaTime) * direction;
		}
		

		// Rotate the vehicle clockwise if the right arrow is held down
	}

	/// <summary>
	/// Drives the vehicle if the player is holding up
	/// </summary>
	private void Drive()
	{
		// Increase the velocity using acceleration if the up arrow is held down
		if (Input.GetKey(KeyCode.UpArrow))
		{
			acceleration = direction * accelerationRate;
			velocity += acceleration;
		}
		else
		{
			velocity *= (1 - accelerationRate * 5);
		}

		// Decrease the velocity using acceleration if the up arrow isn't pressed

		// Limit the velocity to a max-speed
		velocity = Vector3.ClampMagnitude(velocity, maximumSpeed);
		// This is the way we did velocity in class, you can get rid of this once you're using acceleration to control velocity
		//velocity = Time.deltaTime * speed * direction;

		// Change the position using the velocity
		position += velocity;
	}

	/// <summary>
	/// Checks to see if the vehicle is offscreen, and wraps it around to the other side if needed
	/// </summary>
	private void WrapVehicle()
	{
		// Check to see if the vehicle is off screen
		if (position.x > totalCamWidth) position.x = -totalCamWidth;
		if (position.x < -totalCamWidth) position.x = totalCamWidth;
		if (position.y > totalCamHeight) position.y = -totalCamHeight;
		if (position.y < -totalCamHeight) position.y = totalCamHeight;

		// Wrap the vehicle to the other side of the screen if it's off screen
	}

	/// <summary>
	/// Sets the vehicle's position and rotation
	/// </summary>
	private void SetTransform()
	{
		// Set the position
		transform.position = position;

		// Set the rotation
		// First we want to get the angle of rotation using inverse tangent
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		// Then get a quaternion, rotated around the forward axis (the direction the camera is facing)
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
