using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelSetup : MonoBehaviour {

	public int TunnelLength = 1;	// number of segments
	public int SegmentSize;			// number of voxels per tunnel segment
	public float Radius;
	public float VoxelSize;
	public int Degree;
	public int PerlinWormsOctave;

	private GeneratePerlin perlin;
	private GenerateMesh generateMesh;
	private Mesh mesh;
	private MeshCollider tunnelCollider;
	private int currentX;

	void Start () {
		tunnelCollider = GetComponent<MeshCollider> ();
		perlin = GetComponent<GeneratePerlin> ();
		generateMesh = GetComponent<GenerateMesh> ();

		Mesh mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;

		// Create Perlin Noise
		double[,] noiseX = perlin.GenerateNoise (TunnelLength, PerlinWormsOctave, 0);
		double[,] noiseY = perlin.GenerateNoise (TunnelLength, PerlinWormsOctave, 3);

		// Creates Perlin Worm vectors
		Vector3[] vectors = CreateTunnel (TunnelLength, new Vector3(0,5,0), noiseX, noiseY);

		// Generates Mesh
		Vector3[] testVector = { new Vector3 (0, 3, 0), new Vector3 (1, 1, 0), new Vector3 (1, 0, 0), new Vector3(0, 0, 1)};
		Vector3[] vertices = generateMesh.CreateMesh (testVector, VoxelSize, SegmentSize, Radius, Degree);
		int[] triangles = generateMesh.CalculateIndices (vertices, Degree * 3 * SegmentSize * 3); // <- Tunnel Length

		// Set mesh data
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		// Set Collider
		tunnelCollider.sharedMesh = mesh;
		tunnelCollider.sharedMesh.RecalculateNormals ();
		tunnelCollider.sharedMesh.RecalculateBounds ();
	}


	/*
	 * Calculates a single vector give two noise arrays
	 */
	Vector3 VectorCalculation (double[,] noiseX, double[,] noiseY, int x, int y) {
		Vector3 result = new Vector3 (Mathf.Cos((float)noiseX[x,y]), Mathf.Sin((float)noiseX[x,y]), Mathf.Tan((float)noiseY[x,y]));
		result.Normalize();
		return result;
	}


	/*
	 * Creates the string of vectors that will make up the tunnel
	 */
	public Vector3[] CreateTunnel (int numOfSegments, Vector3 start, double[,] noiseX, double[,] noiseY) {
		Vector3[] result = new Vector3[numOfSegments + 1];
		result [0] = start;

		// For each segment
		for (int n = 1; n <= numOfSegments; ++n) {
			result [n] = VectorCalculation (noiseX, noiseY, 2, n - 1);
		}
		return result;
	}
}
