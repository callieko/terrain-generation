using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh : MonoBehaviour {

	public float Exponential;

	/*
	 * Creates terrain mesh vertices given an array of Perlin Noise values and a voxel size.
	 */
	public Vector3[] CreateMesh (double[,] noiseArray, float voxelSize, float baseHeight, float maxHeightVariance) {
		int size = noiseArray.GetLength (0) - 1;

		Vector3[] vertices = new Vector3[size * size * 4];

		int index = 0;
		for (int r = 0; r < size; ++r) {
			for (int c = 0; c < size; ++c) {
				float centerX = ((r - (float)size / 2) * voxelSize);
				float centerZ = ((c - (float)size / 2) * voxelSize);

				float w = voxelSize / 2f;

				if (Exponential > 0) {
					vertices [index] = new Vector3 (centerX - w, Mathf.Pow (Exponential, (float)noiseArray [r, c]) * maxHeightVariance + baseHeight, centerZ - w);
					vertices [index + 1] = new Vector3 (centerX + w, Mathf.Pow (Exponential, (float)noiseArray [r + 1, c]) * maxHeightVariance + baseHeight, centerZ - w);
					vertices [index + 2] = new Vector3 (centerX + w, Mathf.Pow (Exponential, (float)noiseArray [r + 1, c + 1]) * maxHeightVariance + baseHeight, centerZ + w);
					vertices [index + 3] = new Vector3 (centerX - w, Mathf.Pow (Exponential, (float)noiseArray [r, c + 1]) * maxHeightVariance + baseHeight, centerZ + w);
				} else {
					vertices [index] = new Vector3 (centerX - w, (float)noiseArray [r, c] * maxHeightVariance + baseHeight, centerZ - w);
					vertices [index + 1] = new Vector3 (centerX + w, (float)noiseArray [r + 1, c] * maxHeightVariance + baseHeight, centerZ - w);
					vertices [index + 2] = new Vector3 (centerX + w, (float)noiseArray [r + 1, c + 1] * maxHeightVariance + baseHeight, centerZ + w);
					vertices [index + 3] = new Vector3 (centerX - w, (float)noiseArray [r, c + 1] * maxHeightVariance + baseHeight, centerZ + w);
				}

				index += 4;
			}
		}

		return vertices;
	}


	/*
	 * Creates tunnel mesh vertices given an array of Perlin Noise values and a voxel size.
	 */
	public Vector3[] CreateMesh (Vector3[] vectors, float voxelSize, int segmentSize, float radius, int degree) {

		int numOfSegments = vectors.GetLength (0);
		Vector3 b = vectors[0];
		Vector3 baseRadius = new Vector3 (0, -radius, 0);

		Vector3[] vertices = new Vector3[degree * 2 * 3 * 2 * segmentSize * numOfSegments];
		int index = 0;

		// Each segment
		for (int a = 1; a < numOfSegments; ++a) {
			Vector3 direction = vectors [a] * voxelSize;

			// Each voxel
			for (int s = 0; s < segmentSize; ++s) {
				Vector3 adjust = Quaternion.Euler ((360 / degree), 0, 0) * baseRadius;
				Vector3 normal = getNormal(direction);
				Vector3 binormal = Vector3.Cross (direction, normal);
				Quaternion rotation = Quaternion.FromToRotation(binormal, adjust);

				Vector3 rVector = Quaternion.FromToRotation (new Vector3(0,-1,0), direction) * baseRadius;

				// Loop around
				for (int r = 0; r < degree; ++r) {
					vertices [index] = b + rVector;
					vertices [index + 1] = b + (rVector + direction);
					vertices [index + 3] = b + (rotation * rVector);
					vertices [index + 2] = b + ((rotation * rVector) + direction);
					vertices [index + 4] = b + (rotation * rVector);
					vertices [index + 5] = b + (rVector + direction);

					vertices [index + 6] = vertices [index + 5];
					vertices [index + 7] = vertices [index + 4];
					vertices [index + 8] = vertices [index + 3];
					vertices [index + 9] = vertices [index + 2];
					vertices [index + 10] = vertices [index + 1];
					vertices [index + 11] = vertices [index];

					//print (vertices [index] + ", " + vertices [index + 1] + ", " + vertices [index + 2] + ", " +
					//vertices [index + 3] + ", " + vertices [index + 4] + ", " + vertices [index + 5]);

					index += 12;
					rVector = rotation * rVector;
					//rotation = rotation * rotation;
				}
				b += direction;
			}
		}

		return vertices;
	}


	/*
	 * Creates test mesh vertices using set vertices. The resulting mesh should be a plan of size
	 * 2 at the origin.
	 */
	public Vector3[] TestMesh () {
		print ("Test Mesh!");
		Vector3[] vertices = new Vector3[4];

		vertices [0] = new Vector3 (-1, 0, 1);
		vertices [1] = new Vector3 (-1, 0, -1);
		vertices [2] = new Vector3 (1, 0, 1);
		vertices [3] = new Vector3 (1, 0, -1);

		return vertices;
	}


	/*
	 * Creates a test tunnel mesh.
	 */
	public Vector3[] TestTunnelMesh (float _radius, int degree) {
		print ("Test Tunnel Mesh!");
		Vector3 b = new Vector3(0, 10, 0);

		Vector3 direction = new Vector3 (1, 0, 0);

		Vector3 radius = new Vector3 (0, -_radius, 0);

		Quaternion rotation = Quaternion.Euler (360/degree, 0, 0);

		Vector3[] vertices = new Vector3[degree * 2 * 3 * 2];

		int index = 0;
		for (int r = 0; r < degree; ++r) {
			vertices [index] = b + (radius);
			vertices [index + 1] = b + (radius + direction);
			vertices [index + 2] = b + (rotation * radius);
			vertices [index + 3] = b + ((rotation * radius) + direction);
			vertices [index + 4] = b + (rotation * radius);
			vertices [index + 5] = b + (radius + direction);
			radius = rotation * radius;

			vertices [index + 6] = vertices [index + 5];
			vertices [index + 7] = vertices [index + 4];
			vertices [index + 8] = vertices [index + 3];
			vertices [index + 9] = vertices [index + 2];
			vertices [index + 10] = vertices [index + 1];
			vertices [index + 11] = vertices [index];

			print (vertices [index] + ", " + vertices [index + 1] + ", " + vertices [index + 2] + ", " +
				vertices [index + 3] + ", " + vertices [index + 4] + ", " + vertices [index + 5]);

			index += 12;
		}

		return vertices;
	}


	/*
	 * Calculates the indices array that will indicate to the graphics processor what vertices
	 * make up which triangles, given an input of vertices. Size number of squares we are dealing
	 * with.
	 */
	public int[] CalculateIndices (Vector3[] vertices, int size) {
		// Each square is made up of two triangles, and these triangles require three indices
		size = (size * 2) * 3;

		// Note that normals face the side with vertices in clockwise order
		int[] indices = new int[size];

		int index = 0;
		for (int i = 0; i < size; i+=6) {
			indices[i] = index+2;
			indices[i+1] = index+1;
			indices[i+2] = index;
			indices[i+3] = index;
			indices[i+4] = index+3;
			indices[i+5] = index+2;
			index += 4;
		}
		return indices;
	}

	/*
	 * Gets the normal of a given tangent vector.
	 */
	private Vector3 getNormal (Vector3 tangent) {
		Vector3 v = new Vector3 ();
		int axis = -1;
		float min = float.MaxValue;
		for (int c = 0; c < 3; ++c) {
			if (min > Mathf.Abs (tangent [c])) {
				min = Mathf.Abs (tangent [c]);
				axis = c;
			}
		}
		v [axis] = 1;
		return Vector3.Cross (tangent, v);
	}
}
