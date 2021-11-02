using System.Collections;
using System.Linq;
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

    [Header("Attributs")]
    public float depthSpacer;
    public float widthSpacer;

    // graph definition
    private VisualizableGraph<int, Edge<int>> graph;

    // graph data cache
    Dictionary<int, DrawnVertex> drawnVertices = new Dictionary<int, DrawnVertex>();
    List<DrawnEdge> drawnLines = new List<DrawnEdge>();

    void Awake() {
        // Create a graph
        // var edges = new[] {new Edge<int>(0, 1), 
        //                    new Edge<int>(0, 2),
        //                    new Edge<int>(1, 3),
        //                    new Edge<int>(3, 4),
        //                    new Edge<int>(2, 3),
        //                    new Edge<int>(0, 5),
        //                    new Edge<int>(2, 6),
        //                    new Edge<int>(2, 7),
        //                    new Edge<int>(2, 8),
        //                    new Edge<int>(5, 9),
        //                    new Edge<int>(0, 10),
        //                    new Edge<int>(10, 11),
        //                    new Edge<int>(10, 12),
        //                    new Edge<int>(10, 13),
        //                    new Edge<int>(10, 14),
        //                    new Edge<int>(5, 15),
        //                    new Edge<int>(1, 15)};

        var edges = new[] {new Edge<int>(0, 1), 
                           new Edge<int>(0, 2),
                           new Edge<int>(0, 3),
                           new Edge<int>(0, 4),
                           new Edge<int>(0, 5),
                           new Edge<int>(0, 6),
                           new Edge<int>(1, 10),
                           new Edge<int>(1, 11),
                           new Edge<int>(4, 20),
                           new Edge<int>(4, 21),
                           new Edge<int>(4, 22),
                           new Edge<int>(5, 30),
                           new Edge<int>(5, 31),
                           new Edge<int>(5, 32),
                           new Edge<int>(5, 33),
                           new Edge<int>(30, 40),
                           new Edge<int>(30, 41),
                           new Edge<int>(30, 42),
                           new Edge<int>(30, 43),
                           new Edge<int>(30, 44),
                           new Edge<int>(30, 45),
                           new Edge<int>(10, 51),
                           new Edge<int>(10, 52),
                           new Edge<int>(10, 53),
                           new Edge<int>(10, 54),
                           new Edge<int>(10, 55),
                           new Edge<int>(10, 56)};

        graph = new RadialTreeGraph<int, Edge<int>>(edges);              
        // graph = new TreeGraph<int, Edge<int>>(edges);
        // graph  = new ForceGraph<int, Edge<int>>(edges);
    }

    void Start() {
        // Calculate display bounds
        displayBounds = parentDisplay.sizeDelta;

        DrawGraph();

        CenterGraph();
    }

    void DrawGraph() {
        Dictionary<int, VisualizableGraph<int, Edge<int>>.NodeData> nodeData = graph.GetNodeData();
        drawnVertices = new Dictionary<int, DrawnVertex>();

        foreach (var edge in graph.graph.Edges) {
            // Draw vertices
            if (!drawnVertices.ContainsKey(edge.Source)) {
                Vector3 position = new Vector3(nodeData[edge.Source].x * widthSpacer, nodeData[edge.Source].y * depthSpacer, 0);
                string displayText = nodeData[edge.Source].id.ToString();
                DrawnVertex newVertex = CreateVertex(position, displayText);
                drawnVertices.Add(edge.Source, newVertex);
            }

            if (!drawnVertices.ContainsKey(edge.Target)) {
                Vector3 position = new Vector3(nodeData[edge.Target].x * widthSpacer, nodeData[edge.Target].y * depthSpacer, 0);
                string displayText = nodeData[edge.Target].id.ToString();
                DrawnVertex newVertex = CreateVertex(position, displayText);
                drawnVertices.Add(edge.Target, newVertex);
            }

            // Draw edge
            CreateEdge(drawnVertices[edge.Source], drawnVertices[edge.Target]);
        }
    }

    // TODO - centering on content rect
    void CenterGraph() {
        DrawnVertex firstVertex = drawnVertices[graph.graph.Edges.First().Source];
        Vector3 vertexPosition = firstVertex.vertexObject.GetPosition();
        vertexPosition.y += displayBounds.y;

        foreach (DrawnVertex drawnVertex in drawnVertices.Values) {
            drawnVertex.vertexObject.SetPosition(drawnVertex.vertexObject.GetPosition() - vertexPosition);
        }
    }

    DrawnVertex CreateVertex(Vector3 position, string displayText) {
        VertexObject newVertex = Instantiate(vertexObject, Vector3.zero, Quaternion.identity, parentDisplay);
        newVertex.SetPosition(position);
        newVertex.SetText(displayText);

        return new DrawnVertex(newVertex.transform, newVertex);
    }

    DrawnEdge CreateEdge(DrawnVertex start, DrawnVertex end) {
        EdgeObject newEdge = Instantiate(edgeObject, Vector3.zero, Quaternion.identity, parentDisplay);
        newEdge.DefinePoints(new Transform[] {start.transform, end.transform});

        return new DrawnEdge(start, end);
    }

    class DrawnVertex {
        public Transform transform;
        public VertexObject vertexObject;

        public DrawnVertex (Transform _transform, VertexObject _vertexObject) {
            transform = _transform;
            vertexObject = _vertexObject;
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
