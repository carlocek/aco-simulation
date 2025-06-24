using System.Collections.Generic;
using UnityEngine;

public class PheromoneMatrix
{
    private float[,] pheromones;
    private int size;

    public PheromoneMatrix(int size)
    {
        this.size = size;
        pheromones = new float[size, size];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                pheromones[i, j] = 1.0f;
    }

    public float Get(int i, int j) => pheromones[i, j];

    public void Evaporate(float rate)
    {
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                pheromones[i, j] = Mathf.Max(pheromones[i, j] * (1f - rate), 0.0001f);
    }

    public void AddPheromone(List<int> tour, float amount)
    {
        for (int i = 0; i < tour.Count; i++)
        {
            int from = tour[i];
            int to = tour[(i + 1) % tour.Count];
            pheromones[from, to] += amount;
            pheromones[to, from] += amount;
        }
    }

    public float GetMaxPheromone()
    {
        float max = float.MinValue;
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                max = Mathf.Max(max, pheromones[i, j]);
        return max;
    }

}
