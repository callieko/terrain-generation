using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSetup : MonoBehaviour {

	public float MaxHeight;
	public int PerlinNoiseOctave;

	public double[,] noise;

	public Mesh mesh;
	public MeshCollider terrainCollider;

	private GeneratePerlin perlin;
	private GenerateMesh generateMesh;
	private int NumberOfVoxels;
	private float VoxelSize;
	

	public void createCube (int[,] seedGrid, int seedX, int seedY, int numVoxels, float voxelSize) {
		NumberOfVoxels = numVoxels;
		VoxelSize = voxelSize;

		perlin = GetComponent<GeneratePerlin> ();
		generateMesh = GetComponent<GenerateMesh> ();
		terrainCollider = GetComponent<MeshCollider> ();

		mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;

		// Create Perlin Noise
		double[,] baseNoise = perlin.GenerateNoise(NumberOfVoxels, PerlinNoiseOctave, seedGrid[seedX,seedY]);
		noise = perlin.InterpolateNoise (baseNoise, seedGrid, seedX, seedY, PerlinNoiseOctave);

		// Generate Mesh
		Vector3[] vertices = generateMesh.CreateMesh(noise, VoxelSize, 0, MaxHeight);
		int[] triangles = generateMesh.CalculateIndices(vertices, (noise.GetLength(0) - 1) * (noise.GetLength(0) - 1));

		// Set up mesh data
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		// Set up collider
		terrainCollider.sharedMesh = mesh;
		terrainCollider.sharedMesh.RecalculateNormals ();
		terrainCollider.sharedMesh.RecalculateBounds ();
	}

}
