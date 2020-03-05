using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
	public int lives = 3;
	public int points = 0;

	public bool endless;

	public Camera camera;

	public GameObject ship;
	public GameObject bullet;

	public GameObject asteroid1;
	public GameObject asteroid2;
	public GameObject asteroid3;
	public GameObject asteroid4;
	private GameObject tempAsteroid;

	private float totalCamHeight;
	private float totalCamWidth;

	private Asteroid _asteroid;
	private Vehicle _vehicle;
	private ShootBullet _bullet;


	public float shootTimer = 0;
	public float spawnTimer = 0;
	public float secsPerAsteroid = 2;
	public float secsPerShot = .4f;

	private GameObject tempBullet;

	private SpriteRenderer tempRenderer;
	private SpriteRenderer shipRenderer;
	private SpriteRenderer bulletRenderer;
	private float shipRadius;
	private float tempRadius;


	public List<GameObject> asteroids;
	public List<GameObject> bullets;

	private MenuManager _menuManager;
	private Vector3 heading;
	private float distance;


    // Start is called before the first frame update
    void Start()
    {
		_menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
		this.endless = _menuManager.getEndless();

		//creates ship collision information
		shipRenderer = ship.GetComponent<SpriteRenderer>();
		shipRadius = Mathf.Pow((shipRenderer.bounds.max.x - shipRenderer.bounds.min.x) / 2, 2) +
					 Mathf.Pow((shipRenderer.bounds.max.y - shipRenderer.bounds.min.y) / 2, 2);


		totalCamHeight = camera.orthographicSize;
		totalCamWidth = totalCamHeight * camera.aspect;

		_bullet = bullet.GetComponent<ShootBullet>();
		_vehicle = ship.GetComponent<Vehicle>();

		asteroids = new List<GameObject>();
		bullets = new List<GameObject>();
	}

	// Update is called once per frame
	void Update()
    {
		CheckCollision();
		SpawnAsteroids();
		Shoot();
		DeleteBullets();
		ChangeScene();

    }
/// <summary>
/// Checks asteroids for collision with ship or bullets
/// </summary>
	void CheckCollision() {

		foreach (GameObject asteroid in asteroids) {
			//check collision with ship
			tempRenderer = asteroid.GetComponent<SpriteRenderer>();
			tempRadius = Mathf.Pow((tempRenderer.bounds.max.x - tempRenderer.bounds.min.x) / 2, 2) + Mathf.Pow((tempRenderer.bounds.max.y - tempRenderer.bounds.min.y) / 2, 2);
			if (Mathf.Pow((tempRenderer.bounds.center.x - shipRenderer.bounds.center.x), 2) + Mathf.Pow((tempRenderer.bounds.center.y - shipRenderer.bounds.center.y), 2) < tempRadius + shipRadius)
			{
				lives--;
				asteroids.Remove(asteroid);
				Destroy(asteroid);
				break;
			}
			//check collision with every bullet
			foreach (GameObject bullet in bullets) {
				bulletRenderer = bullet.GetComponent<SpriteRenderer>();
				if (Mathf.Pow(bulletRenderer.bounds.center.x - tempRenderer.bounds.center.x, 2) + Mathf.Pow(bulletRenderer.bounds.center.y - tempRenderer.bounds.center.y, 2) < tempRadius)
				{
					if (asteroid.tag == "Stage1")
					{
						points += 20;
						//split asteroid into 2 smaller ones
						for (int i = 0; i < 2; i++)
						{
							if (Random.Range(1, 2) == 1)
							{
								tempAsteroid = Instantiate(asteroid3,asteroid.transform.position, Quaternion.Euler(0, Random.Range(-90, 90), 0));
							}
							else
							{
								tempAsteroid = Instantiate(asteroid4, asteroid.transform.position, Quaternion.Euler(0, Random.Range(-90, 90), 0));
							}
							//set new asteroid into motion
							_asteroid = tempAsteroid.GetComponent<Asteroid>();
							_asteroid.position = asteroid.transform.position;
							_asteroid.direction = Quaternion.Euler(0, Random.Range(-90, 90), 0) * asteroid.GetComponent<Asteroid>().direction;
							asteroids.Add(tempAsteroid);

						}
					}
					else { points += 50; }
					asteroids.Remove(asteroid);
					bullets.Remove(bullet);
					Destroy(asteroid);
					Destroy(bullet);
					break;
				}
			}
		}
	}

	/// <summary>
	/// Spawns asteroids after a set time. Random locations and random directions.
	/// </summary>
	void SpawnAsteroids() {
		if (spawnTimer<Time.time) {
			//random type of asteroid
			switch (Random.Range(1,4)) {
				case 1:
					tempAsteroid = Instantiate(asteroid1);
					break;
				case 2:
					tempAsteroid = Instantiate(asteroid2);
					break;

				case 3:
					tempAsteroid = Instantiate(asteroid3);
					break;

				case 4:
					tempAsteroid = Instantiate(asteroid4);
					break;
			}

			//random spawning locations
			switch (Random.Range(1,4)) {
				//above the screen
				case 1:
					tempAsteroid.transform.position = new Vector3(Random.Range(-totalCamWidth, totalCamWidth), totalCamHeight + 1,0);
					break;
				//right of the screen
				case 2:
					tempAsteroid.transform.position = new Vector3(totalCamWidth+1, Random.Range(-totalCamHeight,totalCamHeight), 0);
					break;
				//left of the screen
				case 3:
					tempAsteroid.transform.position = new Vector3(-totalCamWidth - 1, Random.Range(-totalCamHeight, totalCamHeight), 0);
					break;
				//bottom of the screen
				case 4:
					tempAsteroid.transform.position = new Vector3(Random.Range(-totalCamWidth, totalCamWidth), -totalCamHeight -1, 0);
					break;
			}
			//set asteroid movement in a random direction
			_asteroid = tempAsteroid.GetComponent<Asteroid>();
			_asteroid.position = tempAsteroid.transform.position;
			heading = new Vector3(Random.Range(-totalCamWidth/2,totalCamWidth),Random.Range(-totalCamHeight/2,totalCamHeight/2),0) - _asteroid.position;
			distance = heading.magnitude;
			_asteroid.direction = heading / distance;
			_asteroid.direction = Quaternion.Euler(0,Random.Range(-60,60),0)*_asteroid.direction;

			asteroids.Add(tempAsteroid);

	
			spawnTimer = Time.time + secsPerAsteroid;
		}
	}

	/// <summary>
	/// Sets the bullets into motion
	/// </summary>
	private void Shoot()
	{
		//time interval to make sure it doesn't rapid fire
		if (shootTimer < Time.time && Input.GetKey(KeyCode.Space))
		{
			tempBullet = Instantiate(bullet);
			_bullet = tempBullet.GetComponent<ShootBullet>();
			_bullet.direction = _vehicle.direction;
			_bullet.position = _vehicle.position;
			bullets.Add(tempBullet);
			shootTimer = Time.time + secsPerShot;
		}
	}

	//if the bullet goes off screen, remove the bullet
	private void DeleteBullets() {
		if (bullets.Count >10)
		{
			for (int i = 0; i < bullets.Count; i++)
			{
				if (bullets[i].transform.position.x < totalCamWidth)
				{
					Destroy(bullets[i]);
					bullets.RemoveAt(i);
					i--;
				}
				else if (bullets[i].transform.position.x > -totalCamWidth)
				{
					Destroy(bullets[i]);
					bullets.RemoveAt(i);
					i--;
				}
				else if (bullets[i].transform.position.y < totalCamHeight)
				{
					Destroy(bullets[i]);
					bullets.RemoveAt(i);
					i--;
				}
				else if (bullets[i].transform.position.y > -totalCamHeight)
				{
					Destroy(bullets[i]);
					bullets.RemoveAt(i);
					i--;
				}
			}
		}
	}

	//changes scene based on game-mode requirements
	void ChangeScene() {
		if (lives == 0) SceneManager.LoadScene(2);
		if (!endless&&points >= 1000) SceneManager.LoadScene(3);
	}

	//prints out lives, points, and game mode
	void OnGUI() {
		GUI.color = Color.white;

		// Increase text size
		GUI.skin.box.fontSize = 20;

		GUI.skin.box.wordWrap = true;

		if (endless)
		{
			GUI.Box(new Rect(0, 0, 150, 75), "Points: " + points + "\nLives: " + lives + "\nENDLESS MODE");
		}
		else {
			GUI.Box(new Rect(0, 0, 150, 75), "Points: " + points + "\nLives: " + lives + "\nPOINTS MODE");
		}

	}
}
