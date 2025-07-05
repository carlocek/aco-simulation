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
    public bool simulationRunning = false;
    public bool simulationPaused = false;
    public event System.Action<string> OnLog;
    private PheromoneMatrix pheromones;
    private Coroutine simulationCoroutine;

    public void Log(string msg)
    {
        Debug.Log(msg);
        OnLog?.Invoke(msg); // notifica iscritti (es. UI)
    }

    public void Awake()
    {
        if (graph == null)
        {
            graph = GetComponent<Graph>();
            if (graph == null)
                Log("ACOController: Nessun componente Graph trovato sullo stesso GameObject.");
        }
    }

    public void ToggleSimulation()
    {
        if (!simulationRunning)
        {
            simulationRunning = true;
            simulationPaused = false;
            if (simulationCoroutine == null)
                simulationCoroutine = StartCoroutine(RunACO());
        }
        else
        {
            simulationPaused = !simulationPaused;
        }
    }

    public void StopSimulation()
    {
        simulationRunning = false;
        simulationPaused = false;
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
        }
    }


    IEnumerator RunACO()
    {
        Log("Simulation started...");

        pheromones = new PheromoneMatrix(graph.NodeCount);
        List<int> bestTour = null;
        float bestLength = float.MaxValue;

        for (int it = 0; it < iterations; it++)
        {
            Log($"Iteration {it + 1}/{iterations}");

            List<Ant> ants = new List<Ant>();

            for (int i = 0; i < numAnts; i++)
            {
                while (simulationPaused)
                    yield return null;
                Ant ant = new Ant(graph, pheromones, alpha, beta);
                yield return StartCoroutine(ant.TraverseStepByStep(graph.DrawEdge, stepDelay));
                ants.Add(ant);

                float length = ant.GetTourLength();
                Log($"Ant {i}: tour length = {length:F2}");

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

            while (simulationPaused)
                yield return null;
        }

        if (bestTour != null)
            graph.DrawTour(bestTour);

        Log($"Simulazione completed... length of best tour: {bestLength:F2}");
        StopSimulation();
    }
}
