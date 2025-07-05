using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Graph : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject antPrefab;
    public int nodeCount = 5;
    public float radius = 5f;
    public List<Transform> nodes = new List<Transform>();
    public int NodeCount => nodes.Count;

    public Material pheromoneMaterial;
    private Dictionary<(int, int), LineRenderer> pheromoneLines = new();
    private List<LineRenderer> bestTourLines = new();
    private bool placingNodes = false;
    private ACOController acoController;

    public void SetACOController(ACOController controller)
    {
        acoController = controller;
    }

    void Start()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("Graph: nodePrefab not assigned.");
        }
    }

    void Update()
    {
        if (placingNodes && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            AddNode(worldPos);
        }
    }

    public void ToggleNodePlacement()
    {
        placingNodes = !placingNodes;
    }

    public void ClearGraph()
    {
        if (!acoController.simulationPaused)
        {
            acoController.Log("Pause simulation before deleting the graph.");
            return;
        }
        foreach (Transform node in nodes)
                Destroy(node.gameObject);
        nodes.Clear();
        foreach (LineRenderer line in pheromoneLines.Values)
            Destroy(line.gameObject);
        pheromoneLines.Clear();
        foreach (LineRenderer line in bestTourLines)
            Destroy(line.gameObject);
        bestTourLines.Clear();
    }

    public void AddNode(Vector3 position)
    {
        GameObject node = Instantiate(nodePrefab, position, Quaternion.identity, transform);
        node.name = $"Node {nodes.Count}";
        nodes.Add(node.transform);

        var tmp = node.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
            tmp.text = (nodes.Count - 1).ToString();
    }

    public float GetDistance(int i, int j)
    {
        return Vector3.Distance(nodes[i].position, nodes[j].position);
    }

    public void DrawEdge(List<int> tour)
    {
        if (tour.Count < 2) return;

        int from = tour[^2];
        int to = tour[^1];

        CreateAnimatedEdge(from, to);
    }

    public void DrawTour(List<int> tour)
    {
        for (int i = 0; i < tour.Count - 1; i++)
        {
            int from = tour[i];
            int to = tour[i + 1];
            CreateAnimatedEdge(from, to, false);
        }
    }

    private void CreateAnimatedEdge(int from, int to, bool fade = true)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.red;
        lr.startWidth = lr.endWidth = 0.1f;
        lr.positionCount = 2;
        lr.sortingOrder = 1;

        var animatedEdge = lineObj.AddComponent<AnimatedEdge>();
        animatedEdge.antSpritePrefab = antPrefab;
        float delay = acoController != null ? acoController.stepDelay : 0.5f;
        float drawDuration = delay * 0.8f;
        float fadeDelay = delay * 0.2f;
        float fadeDuration = Mathf.Max(0.5f, delay * 1.2f);
        animatedEdge.AnimateEdge(
            nodes[from].position,
            nodes[to].position,
            drawDuration,
            fadeDelay,
            fadeDuration,
            fade
        );
        if (!fade)
            bestTourLines.Add(lr);
    }

    public void UpdatePheromoneVisual(int from, int to, float amount, float maxAmount)
    {
        var key = (Mathf.Min(from, to), Mathf.Max(from, to));

        if (!pheromoneLines.TryGetValue(key, out var line))
        {
            GameObject lineObj = new GameObject($"Pheromone {from}-{to}");
            lineObj.transform.parent = transform;

            line = lineObj.AddComponent<LineRenderer>();
            
            if (pheromoneMaterial != null)
            {
                line.material = pheromoneMaterial;
            }
            else
            {
                Material fallbackMaterial = new Material(Shader.Find("Sprites/Default"));
                fallbackMaterial.color = Color.white;
                line.material = fallbackMaterial;
            }

            // LineRenderer setup
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.numCapVertices = 4;  // Opzionale: migliora l'estetica dei bordi
            line.SetPosition(0, nodes[from].position);
            line.SetPosition(1, nodes[to].position);
            line.sortingOrder = 0;

            pheromoneLines[key] = line;
        }

        float normalized = Mathf.Clamp01(amount / maxAmount);
        float thickness = Mathf.Lerp(0.01f, 0.15f, normalized);
        Color color = Color.Lerp(new Color(0, 1, 0, 0f), new Color(0, 1, 0, 1f), normalized);  // green with alpha
        line.startWidth = line.endWidth = thickness;
        line.startColor = line.endColor = color;
    }

}
