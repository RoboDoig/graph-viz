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
    private Vector2 displayBounds;
    public VertexObject vertexObject;
    public EdgeObject edgeObject;

    public enum GraphType {Tree, RadialTree, Force}
    [Header("Attributes")]
    public GraphType graphType;
    public float depthSpacer;
    public float widthSpacer;
    public float rotationDegrees;

    // graph definition
    private VisualizableGraph<int, Edge<int>> graph;

    // graph data cache
    Dictionary<int, VisualizableGraph<int, Edge<int>>.NodeData> nodeData;
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

        // var edges = new[] {new Edge<int>(0, 1), 
        //                    new Edge<int>(0, 2),
        //                    new Edge<int>(0, 3),
        //                    new Edge<int>(0, 4),
        //                    new Edge<int>(0, 5),
        //                    new Edge<int>(0, 6),
        //                    new Edge<int>(1, 10),
        //                    new Edge<int>(1, 11),
        //                    new Edge<int>(4, 20),
        //                    new Edge<int>(4, 21),
        //                    new Edge<int>(4, 22),
        //                    new Edge<int>(5, 30),
        //                    new Edge<int>(5, 31),
        //                    new Edge<int>(5, 32),
        //                    new Edge<int>(5, 33),
        //                    new Edge<int>(30, 40),
        //                    new Edge<int>(30, 41),
        //                    new Edge<int>(30, 42),
        //                    new Edge<int>(30, 43),
        //                    new Edge<int>(30, 44),
        //                    new Edge<int>(30, 45),
        //                    new Edge<int>(10, 51),
        //                    new Edge<int>(10, 52),
        //                    new Edge<int>(10, 53),
        //                    new Edge<int>(10, 54),
        //                    new Edge<int>(10, 55),
        //                    new Edge<int>(10, 56),
        //                    new Edge<int>(6, 60),
        //                    new Edge<int>(6, 61),
        //                    new Edge<int>(6, 62),
        //                    new Edge<int>(6, 63),
        //                    new Edge<int>(20, 70),
        //                    new Edge<int>(20, 71),
        //                    new Edge<int>(20, 72),
        //                    new Edge<int>(20, 73),
        //                    new Edge<int>(62, 80),
        //                    new Edge<int>(62, 81),
        //                    new Edge<int>(62, 82),
        //                    new Edge<int>(62, 83),
        //                    new Edge<int>(63, 83)};
    }

    Edge<int>[] GraphFactory(int depth, int nChildren) {
        List<Edge<int>> edgeList = new List<Edge<int>>();
        int counter = 0;
        BuildGraphRecursive(edgeList, 0, depth, nChildren, ref counter);

        return edgeList.ToArray();
    }

    void BuildGraphRecursive(List<Edge<int>> edges, int parent, int depth, int nChildren, ref int counter) {
        int thisNode = counter;
        edges.Add(new Edge<int>(parent, thisNode));

        if (depth == 0) {
            return;
        }

        for (int i = 0; i < nChildren; i++) {
            counter++;
            BuildGraphRecursive(edges, thisNode, depth-1, nChildren, ref counter);
        }
    }

    void Start() {
        // Calculate display bounds
        displayBounds = parentDisplay.sizeDelta;

        GenerateGraph();

        DrawGraph();

        CenterGraph();
    }

    // Interface for switching layout / shape at run time notes...
    // If we already have a graph and want to change layout...
    // 1. we have existing node data, and generate new node data
    // 2. we can then match each original node to its new data with the nodeData dict
    // 3. each node is moved to new position, maybe with some animation

    // If we have new, added or removed graph data
    // 

    void GenerateGraph() {
        Edge<int>[] edges = GraphFactory(3, 3);

        if (graphType == GraphType.Tree) {
            graph = new TreeGraph<int, Edge<int>>(edges);
        } else if (graphType == GraphType.RadialTree) {
            graph = new RadialTreeGraph<int, Edge<int>>(edges);
        } else if (graphType == GraphType.Force) {
            graph  = new ForceGraph<int, Edge<int>>(edges);
        }

        nodeData = graph.GetNodeData();
    }

    void DrawGraph() {
        drawnVertices = new Dictionary<int, DrawnVertex>();

        // for each defined edge in the graph
        foreach (var edge in graph.graph.Edges) {
            // Draw the source vertex if not already drawn
            if (!drawnVertices.ContainsKey(edge.Source)) {
                Vector3 position = new Vector3(nodeData[edge.Source].x * widthSpacer, nodeData[edge.Source].y * depthSpacer, 0);
                position = RotatePointAroundPivot(position, Vector3.zero, new Vector3(0, 0, rotationDegrees));
                string displayText = nodeData[edge.Source].id.ToString();
                DrawnVertex newVertex = CreateVertex(edge.Source, position, displayText);
            }

            // Draw the target vertex if not already drawn
            // TODO - repeated code here from above
            if (!drawnVertices.ContainsKey(edge.Target)) {
                Vector3 position = new Vector3(nodeData[edge.Target].x * widthSpacer, nodeData[edge.Target].y * depthSpacer, 0);
                position = RotatePointAroundPivot(position, Vector3.zero, new Vector3(0, 0, rotationDegrees));
                string displayText = nodeData[edge.Target].id.ToString();
                DrawnVertex newVertex = CreateVertex(edge.Target, position, displayText);
            }

            // Draw the edge between them
            CreateEdge(drawnVertices[edge.Source], drawnVertices[edge.Target]);
        }
    }

    void UpdateLayout() {
        GenerateGraph();

        foreach (int id in drawnVertices.Keys) {
            Vector3 position = new Vector3(nodeData[id].x * widthSpacer, nodeData[id].y * depthSpacer, 0);
            position = RotatePointAroundPivot(position, Vector3.zero, new Vector3(0, 0, rotationDegrees));
            drawnVertices[id].vertexObject.SetPosition(position);
        }

        CenterGraph();
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        return dir + pivot;
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

    DrawnVertex CreateVertex(int id, Vector3 position, string displayText) {
        VertexObject newVertex = Instantiate(vertexObject, Vector3.zero, Quaternion.identity, parentDisplay);
        newVertex.SetPosition(position);
        newVertex.SetText(displayText);
        DrawnVertex drawnVertex = new DrawnVertex(newVertex.transform, newVertex);
        drawnVertices.Add(id, drawnVertex);

        return drawnVertex;
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

    void OnValidate() {
        if (Application.isPlaying) {
            UpdateLayout();
        }
    }
}
