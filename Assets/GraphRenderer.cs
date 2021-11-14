using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;

// TODO Require Components
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
public class GraphRenderer : MonoBehaviour
{
    // Display parameters
    private RectTransform parentDisplay;
    private Vector2 displayBounds;

    [Header("Render Ojects")]
    public VertexObject vertexObject;
    public EdgeObject edgeObject;

    public enum GraphType {Tree, RadialTree, Force}
    [Header("Attributes")]
    public GraphType graphType;
    public float depthSpacer;
    public float widthSpacer;
    public float rotationDegrees;

    // graph definition and data cache
    private Edge<int>[] edges;
    private VisualizableGraph<int, Edge<int>> graph;
    private Dictionary<int, VisualizableGraph<int, Edge<int>>.NodeData> nodeData;
    private Dictionary<int, DrawnVertex> drawnVertices = new Dictionary<int, DrawnVertex>();
    private List<DrawnEdge> drawnLines = new List<DrawnEdge>();

    // events
    public event Action<DrawnVertex> vertexAdded;
    public event Action<DrawnVertex> vertexRemoved;
    public event Action<DrawnVertex> vertexChanged;
    public event Action<DrawnEdge> edgeAdded;
    public event Action<DrawnEdge> edgeRemoved;
    public event Action<DrawnEdge> edgeChanged;

    void Awake() {
        parentDisplay = GetComponent<RectTransform>();
    }

    public void InitializeGraph(Edge<int>[] edges) {
        this.edges = edges;
    }    

    public void DisplayGraph() {
        GenerateLayout();

        DrawGraph();

        CenterGraph();
    }

    // TODO
    public void AddEdges(Edge<int>[] edges) {

    }

    // TODO
    public void RemoveEdges(Edge<int>[] edges) {

    }

    void GenerateLayout() {
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

    void CenterGraph() {
        DrawnVertex firstVertex = drawnVertices[graph.graph.Edges.First().Source];
        Vector3 vertexPosition = firstVertex.vertexObject.GetPosition();
        vertexPosition.y += displayBounds.y;

        foreach (DrawnVertex drawnVertex in drawnVertices.Values) {
            drawnVertex.vertexObject.SetPosition(drawnVertex.vertexObject.GetPosition() - vertexPosition);
        }
    }

    void UpdateLayout() {
        GenerateLayout();

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

    DrawnVertex CreateVertex(int id, Vector3 position, string displayText) {
        VertexObject newVertex = Instantiate(vertexObject, Vector3.zero, Quaternion.identity, parentDisplay);
        newVertex.SetPosition(position);
        newVertex.SetText(displayText);
        DrawnVertex drawnVertex = new DrawnVertex(newVertex.transform, newVertex);
        drawnVertices.Add(id, drawnVertex);

        // invoke vertexAdded event
        vertexAdded(drawnVertex);

        return drawnVertex;
    }

    DrawnEdge CreateEdge(DrawnVertex start, DrawnVertex end) {
        EdgeObject newEdge = Instantiate(edgeObject, Vector3.zero, Quaternion.identity, parentDisplay);
        newEdge.transform.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        newEdge.DefinePoints(start.transform, end.transform);

        DrawnEdge drawnEdge = new DrawnEdge(newEdge.transform, start, end);

        // invoke edgeAdded event
        edgeAdded(drawnEdge);

        return drawnEdge;
    }

    public class DrawnVertex {
        public Transform transform;
        public VertexObject vertexObject;

        public DrawnVertex (Transform _transform, VertexObject _vertexObject) {
            transform = _transform;
            vertexObject = _vertexObject;
        }
    }

    public class DrawnEdge {
        public Transform transform;
        public DrawnVertex source;
        public DrawnVertex target;

        public DrawnEdge (Transform _transform, DrawnVertex _source, DrawnVertex _target) {
            transform = _transform;
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
