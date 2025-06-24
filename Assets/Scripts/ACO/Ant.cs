using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ant
{
    private Graph graph;
    private PheromoneMatrix pheromones;
    private float alpha;
    private float beta;
    public List<int> tour = new List<int>();

    public Ant(Graph graph, PheromoneMatrix pheromones, float alpha, float beta)
    {
        this.graph = graph;
        this.pheromones = pheromones;
        this.alpha = alpha;
        this.beta = beta;
    }

    public IEnumerator TraverseStepByStep(Action<List<int>> onStep, float delay)
    {
        int n = graph.NodeCount;
        HashSet<int> visited = new HashSet<int>();
        int current = UnityEngine.Random.Range(0, n);
        tour.Add(current);
        visited.Add(current);
        onStep?.Invoke(new List<int>(tour));
        yield return new WaitForSeconds(delay);

        while (tour.Count < n)
        {
            int next = SelectNextNode(current, visited);
            tour.Add(next);
            visited.Add(next);
            current = next;
            onStep?.Invoke(new List<int>(tour));
            yield return new WaitForSeconds(delay);
        }
    }

    int SelectNextNode(int current, HashSet<int> visited)
    {
        int n = graph.NodeCount;
        float[] probabilities = new float[n];
        float sum = 0;

        for (int j = 0; j < n; j++)
        {
            if (visited.Contains(j)) continue;
            float tau = Mathf.Pow(pheromones.Get(current, j), alpha);
            float eta = Mathf.Pow(1.0f / graph.GetDistance(current, j), beta);
            probabilities[j] = tau * eta;
            sum += probabilities[j];
        }

        float r = UnityEngine.Random.Range(0f, sum);
        float cumulative = 0;
        for (int j = 0; j < n; j++)
        {
            if (visited.Contains(j)) continue;
            cumulative += probabilities[j];
            if (r <= cumulative) return j;
        }

        for (int j = 0; j < n; j++)
            if (!visited.Contains(j)) return j;

        return 0;
    }

    public float GetTourLength()
    {
        float total = 0;
        for (int i = 0; i < tour.Count; i++)
        {
            int from = tour[i];
            int to = tour[(i + 1) % tour.Count];
            total += graph.GetDistance(from, to);
        }
        return total;
    }
}
