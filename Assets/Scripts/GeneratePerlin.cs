using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePerlin : MonoBehaviour {

	public double[] Weights;

	private int SquareSideSize;
	private int Octaves;
	private Random rand;
	private bool UseWeights;


	/*
	 * Creates a Perlin Noise array using the given values.
	 */
	public double[,] GenerateNoise (int size, int octave, int seed) {
		Random.InitState(seed);
		SquareSideSize = size;
		Octaves = octave;
		UseWeights = true;

		// Checking the validity of the weights provided
		if (Weights.GetLength (0) != octave) {
			print ("Weights doesn't match octave!");
			Weights = new double[0];
			UseWeights = false;
		}
		if (UseWeights) {
			double total = 0;
			for (int i = 0; i < Weights.GetLength (0); ++i) {
				total += Weights [i];
			}
			if (total != 1) {
				print ("Warning! Weights do not add up to 1. Some data will be lost. Total: " + total);
			}
		}

		double[,] noiseArray = new double[SquareSideSize,SquareSideSize];

		int currentNoiseLevel = Octaves;
		while (currentNoiseLevel >= 1) {
			double weight = 1;
			if (UseWeights)
				weight = Weights[Octaves - currentNoiseLevel];
			
			double[,] s = Generate ((int)(SquareSideSize / Mathf.Pow(2f, (float)currentNoiseLevel)));

			for (int r = 0; r < SquareSideSize; ++r) {
				for (int c = 0; c < SquareSideSize; ++c) {
					noiseArray [r, c] += s [r, c] * weight;
				}
			}

			currentNoiseLevel /= 2;
		}

		if (!UseWeights) {
			for (int r = 0; r < SquareSideSize; ++r) {
				for (int c = 0; c < SquareSideSize; ++c) {
					noiseArray [r, c] /= Octaves;
				}
			}
		}

		return noiseArray;
	}


	/*
	 * Generates a square array of given size and populates it with random values.
	 */
	private double[,] Generate (int size) {
		double[,] s = new double[size,size];

		for (int r = 0; r < size; ++r) {
			for (int c = 0; c < size; ++c) {
				s[r,c] = Random.Range(0.0f,1.0f);
			}
		}
		return Zoom(s);
	}


	/*
	 * Given an array of variable size, it will use bilinear interpolation to scale that array to size
	 * SquareSideSize.
	 */
	private double[,] Zoom (double[,] s) {
		if (s.GetLength (0) == SquareSideSize)
			return s;
		
		double[,] result = new double[SquareSideSize,SquareSideSize];

		double proportion = (double)s.GetLength (0) / (double)SquareSideSize;

		for (int r = 0; r < result.GetLength (0); ++r) {
			for (int c = 0; c < result.GetLength (0); ++c) {
				result [r,c] = BilinearInterpolation ((double)r * proportion, (double)c * proportion, s);
			}
		}

		return result;
	}


	/*
	 * Calculates the interpolation given a noise array and two values. Uses bilinear interpolation.
	 */
	public static double BilinearInterpolation (double x, double y, double[,] n) {
		int size = n.GetLength (0);
		double fractX = (x - (int)x + size) % size;
		double fractY = (y - (int)y + size) % size;

		int x1 = ((int)x + size) % size;
		int y1 = ((int)y + size) % size;

		int x2 = (x1 - 1 + size) % size;
		int y2 = (y1 - 1 + size) % size;

		double result = 0.0;
		result += fractX * fractY * n[y1,x1];
		if (x2 >= 0)
			result += (1 - fractX) * fractY * n[y1,x2];
		if (y2 >= 0)
			result += fractX * (1 - fractY) * n[y2,x1];
		if (x2 >= 0 && y2 >= 0)
			result += (1 - fractX) * (1 - fractY) * n[y2,x2];

		return result;
	}


	/*
	 * Given a particular noise array and array of seeds, returns a version of the noise array that
	 * will connect seamlessly with other noise arrays.
	 */
	public double[,] InterpolateNoise (double[,] noise,  int[,] seedGrid, int x, int y, int octave) {
		int size = noise.GetLength (0);
		int interpolSize = (int)Mathf.Log ((float)size, 2.0f);

		// Ideally, there would be a noise array for each direction that this noise array the four ajacent
		// arrays would share, but I couldn't get it to work for some reason
		double[,] inoise = GenerateNoise (size, octave, 0);

		for (int r = 0; r < interpolSize; ++r) {
			double p = 1 / (double)(Mathf.Pow(2.0f, r));
			for (int c = 0; c < size; ++c) {
				// Connection on top
				noise [r,c] = (noise[r,c] * (1 - p)) + (inoise[(size / 2) - r,c] * p);
				// Connection on bottom
				noise [size - r - 1,c] = (noise[size - r - 1,c] * (1 - p)) + (inoise[(size / 2) + r,c] * p);
			}
		}
		for (int c = 0; c < interpolSize; ++c) {
			double p = 1 / (double)(Mathf.Pow(2.0f, c));
			for (int r = 0; r < size; ++r) {
				// Connection to left
				noise [r,c] = (noise[r,c] * (1 - p)) + (inoise[r,(size / 2) - c] * p);
				// Connection to right
				noise [r,size - c - 1] = (noise[r,size - c - 1] * (1 - p)) + (inoise[r,(size / 2) + c] * p);
			}
		}
		return noise;

	}

}
