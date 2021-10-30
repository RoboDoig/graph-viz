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
                           new Edge<int>(1, 3),
                           new Edge<int>(3, 4),
                           new Edge<int>(2, 3),
                           new Edge<int>(0, 5),
                           new Edge<int>(2, 6),
                           new Edge<int>(2, 7),
                           new Edge<int>(2, 8),
                           new Edge<int>(5, 9)};
        graph = edges.ToAdjacencyGraph<int, Edge<int>>();
    }

    void Start() {
        // Calculate display bounds
        displayBounds = parentDisplay.sizeDelta;

        InitializeGraph(graph);
        ArrangeGraph(graph);
    }

    void InitializeGraph(AdjacencyGraph<int, Edge<int>> graph) {
        vizGraph = new Dictionary<int, DrawnVertex>();

        foreach (var edge in graph.Edges) {
            // if source vertex hasn't been instantiated, create it
            if (!vizGraph.ContainsKey(edge.Source)) {
                DrawnVertex newVertex = new DrawnVertex(edge.Source, 0, 0, CreateVertex());
                vizGraph.Add(edge.Source, newVertex);
            }

            // if the target vertex hasn't been isntantiated, instantiate it
            if (!vizGraph.ContainsKey(edge.Target)) {
                DrawnVertex newVertex = new DrawnVertex(edge.Target, 0, 0, CreateVertex());
                vizGraph.Add(edge.Target, newVertex);
            }

            CreateLine(vizGraph[edge.Source].transform, vizGraph[edge.Target].transform);
        }
    }

    void ArrangeGraph(AdjacencyGraph<int, Edge<int>> graph) {
        // Calculate vertex depth
        var dfs = new DepthFirstSearchAlgorithm<int, Edge<int>>(graph);
        dfs.TreeEdge += (Edge<int> edge) => {
            vizGraph[edge.Target].depth += vizGraph[edge.Source].depth + 1;
        };
        dfs.Compute();

        Dictionary<int, int> depthRanks = new Dictionary<int, int>();
        // Calculate depth rank
        var bfs = new DepthFirstSearchAlgorithm<int, Edge<int>>(graph);
        bfs.DiscoverVertex += (int vertex) => {
            if (!depthRanks.ContainsKey(vizGraph[vertex].depth)) {
                depthRanks.Add(vizGraph[vertex].depth, 0);
            }

            vizGraph[vertex].depthRank = depthRanks[vizGraph[vertex].depth];
            depthRanks[vizGraph[vertex].depth] += 1;
        };
        bfs.Compute();

        foreach(var vertex in graph.Vertices) {
            vizGraph[vertex].SetPosition(new Vector2(vizGraph[vertex].depthRank * 100, vizGraph[vertex].depth * 100));
            vizGraph[vertex].SetText(vertex.ToString());
        }
    }

    // Get the maximum depth of the graph
    int GetMaxDepth(AdjacencyGraph<int, Edge<int>> graph) {
        int maxDepth = int.MinValue;

        var dfs = new DepthFirstSearchAlgorithm<int, Edge<int>>(graph);
        Dictionary<int, int> vertexParents = new Dictionary<int, int>{{0, 0}};
        dfs.TreeEdge += (Edge<int> edge) => {
            if (!vertexParents.ContainsKey(edge.Target)) {
                vertexParents.Add(edge.Target, 0);
            }

            vertexParents[edge.Target] = 1 + vertexParents[edge.Source];

            if (vertexParents[edge.Target] > maxDepth) {
                maxDepth = vertexParents[edge.Target];
            }
        };
        dfs.Compute();

        return maxDepth;
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
        public int depthRank;
        public Transform transform;

        // UI components
        private RectTransform rectTransform;
        private Button button;
        private Text text;

        public DrawnVertex(int _id, int _depth, int _depthRank, Transform _transform) {
            id = _id;
            depth = _depth;
            depthRank = _depthRank;
            transform = _transform;

            rectTransform = transform.GetComponent<RectTransform>();
            button = transform.GetComponent<Button>();
            text = transform.GetComponentInChildren<Text>();

            button.onClick.AddListener(() => {Debug.Log("Click");});
        }

        public void SetPosition(Vector2 position) {
            rectTransform.anchoredPosition3D = new Vector3(position.x, position.y, 0);
            text.text = "test";
        }

        public void SetText(string displayText) {
            text.text = displayText;
        }
    }

    // class DrawnLine {

    // }
}
