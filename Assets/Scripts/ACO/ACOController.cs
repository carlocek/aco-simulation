using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ACOController : MonoBehaviour
{
    public Graph graph;
    public int numAnts = 10;
    public int iterations = 50;
    public float alpha = 1f;
    public float beta = 2f;
    public float evaporation = 0.5f;
    public float pheromoneDeposit = 100f;
    public float stepDelay = 0.5f;

    private PheromoneMatrix pheromones;

    public void Awake()
    {
        if (graph == null)
        {
            graph = GetComponent<Graph>();
            if (graph == null)
                Debug.LogError("🚨 ACOController: Nessun componente Graph trovato sullo stesso GameObject.");
        }
    }

    public void StartSimulation()
    {
        if (graph.NodeCount < 2)
        {
            Debug.LogError("🚨 ACOController: Numero di nodi insufficiente per simulazione.");
            return;
        }
        StopAllCoroutines();
        StartCoroutine(RunACO());
    }

    IEnumerator RunACO()
    {
        Debug.Log("▶️ Simulation started...");

        pheromones = new PheromoneMatrix(graph.NodeCount);
        List<int> bestTour = null;
        float bestLength = float.MaxValue;

        for (int it = 0; it < iterations; it++)
        {
            Debug.Log($"🔁 Iteration {it + 1}/{iterations}");

            List<Ant> ants = new List<Ant>();

            for (int i = 0; i < numAnts; i++)
            {
                Ant ant = new Ant(graph, pheromones, alpha, beta);
                yield return StartCoroutine(ant.TraverseStepByStep(graph.DrawEdge, stepDelay));
                ants.Add(ant);

                float length = ant.GetTourLength();
                Debug.Log($"🐜 Ant {i}: tour length = {length:F2}");

                if (length < bestLength)
                {
                    bestLength = length;
                    bestTour = new List<int>(ant.tour);
                }
            }

            pheromones.Evaporate(evaporation);
            foreach (var ant in ants)
            {
                float length = ant.GetTourLength();
                pheromones.AddPheromone(ant.tour, pheromoneDeposit / length);
            }

            float maxPheromone = pheromones.GetMaxPheromone();

            for (int i = 0; i < graph.NodeCount; i++)
            {
                for (int j = i + 1; j < graph.NodeCount; j++)
                {
                    float p = pheromones.Get(i, j);
                    graph.UpdatePheromoneVisual(i, j, p, maxPheromone);
                }
            }
        }

        if (bestTour != null)
            graph.DrawTour(bestTour);

        Debug.Log($"✅ Simulazione completed... length of best tour: {bestLength:F2}");
    }
}
