using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;

public class GraphTest : MonoBehaviour
{
    public Transform parentDisplay;
    public GameObject vertexGO;
    public LineController edgeGO;

    private AdjacencyGraph<int, Edge<int>> graph;
    private Dictionary<int, Transform> vizGraph = new Dictionary<int, Transform>();

    void Awake() {
        // Create a graph
        var edges = new[] {new Edge<int>(1, 2), new Edge<int>(0,1), new Edge<int>(2,0), new Edge<int>(3,1)};
        graph = edges.ToAdjacencyGraph<int, Edge<int>>();
    }

    void Start() {
        foreach (var edge in graph.Edges) {
            // if the source vertex hasn't been instantiated, instantiate it
            if (!vizGraph.ContainsKey(edge.Source)) {
                Transform newVertex = CreateVertex();
                vizGraph.Add(edge.Source, newVertex);
            }

            // if the target vertex hasn't been isntantiated, instantiate it
            if (!vizGraph.ContainsKey(edge.Target)) {
                Transform newVertex = CreateVertex();
                vizGraph.Add(edge.Target, newVertex);
            }

            CreateLine(vizGraph[edge.Source], vizGraph[edge.Target]);
        }
    }

    Transform CreateVertex() {
        GameObject newVertex = Instantiate(vertexGO);
        newVertex.transform.SetParent(parentDisplay);
        newVertex.transform.localScale = Vector3.one;
        newVertex.transform.position = Vector3.zero;
        newVertex.transform.localRotation = Quaternion.identity;
        newVertex.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(Random.Range(-600f, 600f), Random.Range(-500f, 500f), 0);

        return newVertex.transform;
    }

    Transform CreateLine(Transform start, Transform end) {
        LineController newLine = Instantiate(edgeGO);
        newLine.transform.SetParent(parentDisplay);
        newLine.transform.localScale = Vector3.one;
        newLine.transform.position = Vector3.zero;
        newLine.transform.localRotation = Quaternion.identity;
        newLine.points = new Transform[] {start, end};

        return newLine.transform;
    }
}
