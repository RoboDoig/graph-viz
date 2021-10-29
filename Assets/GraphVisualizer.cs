using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using QuikGraph.Algorithms.Search;

public class GraphVisualizer : MonoBehaviour
{
    public RectTransform parentDisplay;
    [SerializeField]
    private Vector2 displayBounds;
    public Vertex vertexObject;
    public EdgeLine lineObject;

    // graph definition
    private AdjacencyGraph<int, Edge<int>> graph;
    // map vertices to transforms
    private Dictionary<int, DrawnVertex> vizGraph = new Dictionary<int, DrawnVertex>();

    void Awake() {
        // Create a graph
        var edges = new[] {new Edge<int>(0, 1), 
                           new Edge<int>(0, 2),
                           new Edge<int>(1, 3)};
        graph = edges.ToAdjacencyGraph<int, Edge<int>>();
    }

    void Start() {
        // Calculate display bounds
        displayBounds = parentDisplay.sizeDelta;

        DrawGraph(graph);
        ArrangeGraph(graph);
    }

    void DrawGraph(AdjacencyGraph<int, Edge<int>> graph) {
        vizGraph = new Dictionary<int, DrawnVertex>();

        foreach (var edge in graph.Edges) {
            // if source vertex hasn't been instantiated, create it
            if (!vizGraph.ContainsKey(edge.Source)) {
                DrawnVertex newVertex = new DrawnVertex(edge.Source, 0, CreateVertex());
                vizGraph.Add(edge.Source, newVertex);
            }

            // if the target vertex hasn't been isntantiated, instantiate it
            if (!vizGraph.ContainsKey(edge.Target)) {
                DrawnVertex newVertex = new DrawnVertex(edge.Target, 0, CreateVertex());
                vizGraph.Add(edge.Target, newVertex);
            }

            CreateLine(vizGraph[edge.Source].transform, vizGraph[edge.Target].transform);
        }
    }

    void ArrangeGraph(AdjacencyGraph<int, Edge<int>> graph) {
        var dfs = new DepthFirstSearchAlgorithm<int, Edge<int>>(graph);
        dfs.TreeEdge += (Edge<int> edge) => {
            vizGraph[edge.Target].depth += vizGraph[edge.Source].depth + 1;
            Debug.Log(vizGraph[edge.Target].depth);
        };
        dfs.Compute();
    }

    Transform CreateVertex() {
        Vertex newVertex = Instantiate(vertexObject);
        newVertex.transform.SetParent(parentDisplay);
        newVertex.transform.localScale = Vector3.one;
        newVertex.transform.position = Vector3.zero;
        newVertex.transform.localRotation = Quaternion.identity;
        newVertex.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, displayBounds.y/2, 0);

        return newVertex.transform;
    }

    Transform CreateLine(Transform start, Transform end) {
        EdgeLine newLine = Instantiate(lineObject);
        newLine.transform.SetParent(parentDisplay);
        newLine.transform.localScale = Vector3.one;
        newLine.transform.position = Vector3.zero;
        newLine.transform.localRotation = Quaternion.identity;
        newLine.DefinePoints(new Transform[] {start, end});

        return newLine.transform;
    }

    class DrawnVertex {
        public int id;
        public int depth;
        public Transform transform;

        public DrawnVertex(int _id, int _depth, Transform _transform) {
            id = _id;
            depth = _depth;
            transform = _transform;
        }
    }
}
