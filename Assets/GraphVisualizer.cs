using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using QuikGraph.Algorithms.Search;
using UnityEngine.UI;

public class GraphVisualizer : MonoBehaviour
{
    public RectTransform parentDisplay;
    [SerializeField]
    private Vector2 displayBounds;
    public VertexObject vertexObject;
    public EdgeObject edgeObject;

    // graph definition
    private VisualizableGraph<int, Edge<int>> graph;

    // graph data cache
    Dictionary<int, DrawnVertex> drawnVertices = new Dictionary<int, DrawnVertex>();
    List<DrawnEdge> drawnLines = new List<DrawnEdge>();

    void Awake() {
        // Create a graph
        var edges = new[] {new Edge<int>(0, 1), 
                           new Edge<int>(0, 2),
                           new Edge<int>(1, 3),
                           new Edge<int>(3, 4),
                           new Edge<int>(2, 3),
                           new Edge<int>(0, 5),
                           new Edge<int>(2, 6),
                           new Edge<int>(2, 7),
                           new Edge<int>(2, 8),
                           new Edge<int>(5, 9)};
                           
        graph = new HierarchicalGraph<int, Edge<int>>(edges);
    }

    void Start() {
        // Calculate display bounds
        displayBounds = parentDisplay.sizeDelta;

        DrawGraph();
    }

    void DrawGraph() {
        drawnVertices = new Dictionary<int, DrawnVertex>();

        foreach (var edge in graph.graph.Edges) {
            // Draw vertices
            if (!drawnVertices.ContainsKey(edge.Source)) {
                Vector3 position = new Vector3(graph.nodeGraph[edge.Source].depthRank * 100, graph.nodeGraph[edge.Source].depth * 100, 0);
                string displayText = graph.nodeGraph[edge.Source].id.ToString();
                DrawnVertex newVertex = CreateVertex(position, displayText);
                drawnVertices.Add(edge.Source, newVertex);
            }

            if (!drawnVertices.ContainsKey(edge.Target)) {
                Vector3 position = new Vector3(graph.nodeGraph[edge.Target].depthRank * 100, graph.nodeGraph[edge.Target].depth * 100, 0);
                string displayText = graph.nodeGraph[edge.Target].id.ToString();
                DrawnVertex newVertex = CreateVertex(position, displayText);
                drawnVertices.Add(edge.Target, newVertex);
            }

            // Draw edge
            CreateEdge(drawnVertices[edge.Source], drawnVertices[edge.Target]);
        }
    }

    DrawnVertex CreateVertex(Vector3 position, string displayText) {
        VertexObject newVertex = Instantiate(vertexObject, Vector3.zero, Quaternion.identity, parentDisplay);
        newVertex.SetPosition(position);
        newVertex.SetText(displayText);

        return new DrawnVertex(newVertex.transform);
    }

    DrawnEdge CreateEdge(DrawnVertex start, DrawnVertex end) {
        EdgeObject newEdge = Instantiate(edgeObject, Vector3.zero, Quaternion.identity, parentDisplay);
        newEdge.DefinePoints(new Transform[] {start.transform, end.transform});

        return new DrawnEdge(start, end);
    }

    class DrawnVertex {
        public Transform transform;

        public DrawnVertex (Transform _transform) {
            transform = _transform;
        }
    }

    class DrawnEdge {
        public DrawnVertex source;
        public DrawnVertex target;

        public DrawnEdge (DrawnVertex _source, DrawnVertex _target) {
            source = _source;
            target = _target;
        }
    }
}
