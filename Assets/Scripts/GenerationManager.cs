using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour {

	public GameObject TerrainGenerator;
	public GameObject TunnelGenerator;
	public GameObject player;

	public int NumberOfVoxels;		// Number of voxel subdivisions in a grid
	public float VoxelSize;			// Size of a single voxel
	private float gridSide;

	private int[,] seedGrid;
	private int seedSize = 100;
	private GameObject[,] currentGrids;

	private Vector2 planeCenter;
	private Vector2 gridIndex;


	void Start () {
		// Initialize Seed Grid
		gridSide = NumberOfVoxels * VoxelSize;
		Random.InitState (0);
		seedGrid = new int[seedSize, seedSize];
		for (int i = 0; i < seedSize; ++i) {
			for (int j = 0; j < seedSize; ++j) {
				seedGrid [i, j] = Random.Range (0, 1000);
			}
		}

		// Initalize grid that will hold all currently generated terrain grid squares
		currentGrids = new GameObject[3, 3];

		for (int i = -1; i < 2; ++i) {
			for (int j = -1; j < 2; ++j) {
				currentGrids[i+1,j+1] = createGrid (i, j);
			}
		}

		// Keeps track of player position
		planeCenter = new Vector2 (0, 0);
		gridIndex = new Vector2 (0, 0);

		// TODO Fix Perlin Worms
		//createTunnel ();

	}


	/*
	 * Update is called once per frame. Checks player's position and generates/destroys grid squares
	 * of the map as needed
	 */
	void Update () {
		Vector3 position = player.transform.position;
		Vector2 p = new Vector2 (position.x, position.z);

		if (gridSide / 2 < Mathf.Abs((p - planeCenter).x)) {
			spawnGridRowX (p.x > planeCenter.x);
		}
		if (gridSide / 2 < Mathf.Abs((p - planeCenter).y)) {
			spawnGridRowY (p.y > planeCenter.y);
		}
	}


	/*
	 * Creates a grid at the given point
	 */
	GameObject createGrid (int gridIndexX, int gridIndexZ) {
		GameObject obj = Instantiate (TerrainGenerator);
		TerrainSetup g = obj.GetComponent<TerrainSetup> ();

		int X = Mathf.Abs((int)(gridIndexX) % seedSize);
		int Y = Mathf.Abs((int)(gridIndexZ) % seedSize);

		g.createCube (seedGrid, X, Y, NumberOfVoxels, VoxelSize);
		obj.transform.position = new Vector3 ((gridIndexX * (gridSide - VoxelSize)), 0, (gridIndexZ * (gridSide - VoxelSize)));
		return obj;
	}

	//TODO Add start vector location
	GameObject createTunnel () {
		GameObject obj = Instantiate (TunnelGenerator);
		return obj;
	}


	/*
	 * Creates new grid squares if the player moves left or right on the map
	 */
	private void spawnGridRowX (bool positive) {
		if (positive) {
			planeCenter [0] += gridSide;
			gridIndex [0]++;

			// Destroy grid squares behind player
			for (int i = 0; i < 3; ++i) {
				Destroy (currentGrids [0, i]);
			}

			// Move grid squares in the currentGrid to properly keep track
			for (int i = 0; i < 2; ++i) {
				for (int j = 0; j < 3; ++j) {
					currentGrids [i, j] = currentGrids [i + 1, j];
				}
			}

			// Create new grid squares in front of the player
			for (int i = 0; i < 3; ++i) {
				currentGrids [2, i] = createGrid ((int)(planeCenter.x / gridSide) + 1, (int)(planeCenter.y / gridSide) + i - 1);
			}
		} else {
			planeCenter[0] -= gridSide;
			gridIndex[0]--;

			// Destroy grid squares behind player
			for (int i = 0; i < 3; ++i) {
				Destroy(currentGrids [2, i]);
			}

			// Move grid squares in the currentGrid to properly keep track
			for (int i = 2; i > 0; --i) {
				for (int j = 2; j > -1; --j) {
					currentGrids [i, j] = currentGrids [i - 1, j];
				}
			}
				
			// Create new grid squares in front of the player
			for (int i = 0; i < 3; ++i) {
				currentGrids [0, i] = createGrid ((int)(planeCenter.x / gridSide) - 1, (int)(planeCenter.y / gridSide) + i - 1);
			}
		}
	}


	/*
	 * Creates new grid squares if the player moves up or down on the map
	 */
	private void spawnGridRowY (bool positive) {
		if (positive) {
			planeCenter[1] += gridSide;
			gridIndex[1]++;

			// Destroy grid squares behind player
			for (int i = 0; i < 3; ++i) {
				Destroy(currentGrids [i, 0]);
			}

			// Move grid squares in the currentGrid to properly keep track
			for (int i = 0; i < 3; ++i) {
				for (int j = 0; j < 2; ++j) {
					currentGrids [i, j] = currentGrids [i, j + 1];
				}
			}

			// Create new grid squares in front of the player
			for (int i = 0; i < 3; ++i) {
				currentGrids [i, 2] = createGrid ((int)(planeCenter.x / gridSide) + i - 1, (int)(planeCenter.y / gridSide) + 1);
			}
		} else {
			planeCenter[1] -= gridSide;
			gridIndex[1]--;

			// Destroy grid squares behind player
			for (int i = 0; i < 3; ++i) {
				Destroy(currentGrids [i, 2]);
			}

			// Move grid squares in the currentGrid to properly keep track
			for (int i = 2; i > -1; --i) {
				for (int j = 2; j > 0; --j) {
					currentGrids [i, j] = currentGrids [i, j - 1];
				}
			}

			// Create new grid squares in front of the player
			for (int i = 0; i < 3; ++i) {
				currentGrids [i, 0] = createGrid ((int)(planeCenter.x / gridSide) + i - 1, (int)(planeCenter.y / gridSide) - 1);
			}
		}
	}
}
