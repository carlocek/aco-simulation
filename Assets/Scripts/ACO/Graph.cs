using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Graph : MonoBehaviour
{
    public GameObject nodePrefab;
    public int nodeCount = 5;
    public float radius = 5f;
    public List<Transform> nodes = new List<Transform>();
    public int NodeCount => nodes.Count;
    private Dictionary<(int, int), LineRenderer> pheromoneLines = new();
    public Material pheromoneMaterial;

    private bool placingNodes = false; // modalità attiva/inattiva

    void Start()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("🚨 Graph: nodePrefab non assegnato.");
            return;
        }
    }

    void Update()
    {
        if (placingNodes && Input.GetMouseButtonDown(0))
        {
            // check if the mouse is over a UI element
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; // distance from camera
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
        foreach (Transform node in nodes)
        {
            Destroy(node.gameObject);
        }
        nodes.Clear();
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
        if (tour.Count < 2)
        {
            return;
        }
        int from = tour[tour.Count - 2];
        int to = tour[tour.Count - 1];

        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.red;
        lr.startWidth = lr.endWidth = 0.1f;
        lr.positionCount = 2;
        lr.sortingOrder = 1;

        AnimatedEdge animatedEdge = lineObj.AddComponent<AnimatedEdge>();
        animatedEdge.AnimateEdge(nodes[from].position, nodes[to].position, 0.5f); // durata animazione: 0.5s
    }

    public void DrawTour(List<int> tour)
    {

        for (int i = 0; i < tour.Count - 1; i++)
        {
            int from = tour[i];
            int to = tour[i + 1];

            GameObject lineObj = new GameObject("Line");
            lineObj.transform.parent = transform;

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lr.endColor = Color.red;
            lr.startWidth = lr.endWidth = 0.1f;
            lr.positionCount = 2;
            lr.sortingOrder = 1;

            AnimatedEdge animatedEdge = lineObj.AddComponent<AnimatedEdge>();
            animatedEdge.AnimateEdge(nodes[from].position, nodes[to].position, 0.5f, false); // durata animazione: 0.5s
        }
    }
    
    public void UpdatePheromoneVisual(int from, int to, float amount, float maxAmount)
    {
        var key = (Mathf.Min(from, to), Mathf.Max(from, to));

        if (!pheromoneLines.ContainsKey(key))
        {
            GameObject lineObj = new GameObject($"Pheromone {from}-{to}");
            lineObj.transform.parent = transform;

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = pheromoneMaterial ?? new Material(Shader.Find("Sprites/Default"));
            lr.positionCount = 2;
            lr.SetPosition(0, nodes[from].position);
            lr.SetPosition(1, nodes[to].position);
            lr.sortingOrder = 0;
            pheromoneLines[key] = lr;
        }

        float normalized = Mathf.Clamp01(amount / maxAmount);
        float thickness = Mathf.Lerp(0.01f, 0.15f, normalized);
        Color color = Color.Lerp(Color.clear, Color.cyan, normalized);

        var line = pheromoneLines[key];
        line.startWidth = line.endWidth = thickness;
        line.startColor = line.endColor = color;
    }
}
