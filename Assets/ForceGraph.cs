using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using QuikGraph.Algorithms.Search;

public class ForceGraph<TVertex, TEdge> : VisualizableGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
{
    public Dictionary<TVertex, Node> nodeGraph {get; protected set;}

    public ForceGraph(TEdge[] edges) : base(edges)
    {

    }

    protected override void InitializeGraph()
    {
        base.InitializeGraph();

        nodeGraph = new Dictionary<TVertex, Node>();

        // Build tree
        DepthFirstSearchAlgorithm<TVertex, TEdge> dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(graph);
        dfs.TreeEdge += (TEdge edge) => {
            if (!nodeGraph.ContainsKey(edge.Source)) {
                nodeGraph.Add(edge.Source, new Node(edge.Source, Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            }

            if (!nodeGraph.ContainsKey(edge.Target)) {
                nodeGraph.Add(edge.Target, new Node(edge.Target, Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            }

            Node sourceNode = nodeGraph[edge.Source];
            Node targetNode = nodeGraph[edge.Target];

            sourceNode.children.Add(targetNode);
            targetNode.parents.Add(sourceNode);
        };
        dfs.Compute();
    }

    protected override void CalculatePositioning()
    {
        base.CalculatePositioning();

        Eades();
        // Barycenter(10);
    }

    void Eades() {
        float c1 = 2f;
        float c2 = 1f;
        float c3 = 1f;
        float c4 = 0.1f;

        // force calculation iterations - TODO, hard coded 100 bad for larger graphs!
        for (int i = 0; i < 100; i++) {

            // for each vertex, calculate its force
            foreach (Node node in nodeGraph.Values) {
                Vector2 nodePosition = node.NodePosition();
                Vector2 moveVector = Vector2.zero;

                // parents pull children with spring force
                foreach (Node parentNode in node.parents) {
                    Vector2 parentNodePosition = parentNode.NodePosition();
                    Vector2 moveDirection = (parentNodePosition - nodePosition);
                    float distance = moveDirection.magnitude;
                    float force = c1 * Mathf.Log(distance / c2);
                    moveVector += (moveDirection * force);
                }

                // non-adjacent nodes repel each other
                foreach (Node otherNode in nodeGraph.Values) {
                    if (otherNode == node || node.parents.Contains(otherNode) || node.children.Contains(otherNode)) {
                        continue;
                    }

                    // TODO - repeated code from above, make some kind of force calculation / move vector method
                    Vector2 otherNodePosition = otherNode.NodePosition();
                    Vector2 moveDirection = (nodePosition - otherNodePosition);
                    float distance = moveDirection.magnitude;
                    float force = c3 / Mathf.Pow(distance, 2);
                    moveVector += (moveDirection * force);
                }

                node.MoveBy(moveVector * c4);
            }
        }
    }

    void FruchtermanReingold() {

    }

    void Barycenter(int polygonSize) {
        // Select fixed vertices (in this case 4), and the remaining free vertices
        int nNodes = nodeGraph.Values.Count;
        List<Node> fixedNodes = nodeGraph.Values.Take(polygonSize).ToList();
        List<Node> freeNodes = nodeGraph.Values.Skip(polygonSize).Take(nNodes - polygonSize).ToList();

        // Polygon points arrangement for fixed nodes
        float xCenter = 0f;
        float yCenter = 0f;
        float angle = 0f;
        float angleIncrement = 2 * Mathf.PI / polygonSize;
        for (int i = 0; i < polygonSize; i++) {
            fixedNodes[i].x = xCenter + 1f * Mathf.Cos(angle);
            fixedNodes[i].y = yCenter + 1f * Mathf.Sin(angle);
            angle += angleIncrement;
        }

        for (int i = 0; i < 100; i++) {
            foreach (Node node in freeNodes) {
                int nodeDegree = node.children.Count + node.parents.Count;

                float edgeX = 0;
                float edgeY = 0;
                foreach(Node adjNode in node.children.Concat(node.parents)) {
                    edgeX += adjNode.x;
                    edgeY += adjNode.y;
                }
                node.x = (1/nodeDegree) * edgeX;
                node.y = (1/nodeDegree) * edgeY;
            }
        }
    }

    public override Dictionary<TVertex, NodeData> GetNodeData()
    {
        Dictionary<TVertex, NodeData> nodeData = new Dictionary<TVertex, NodeData>();

        foreach (Node node in nodeGraph.Values) {
            NodeData newNodeData = new NodeData (
                node.id,
                node.x,
                node.y
            );

            nodeData.Add(newNodeData.id, newNodeData);
        }

        foreach (Node node in nodeGraph.Values) {
            if (node.parents != null) {
                foreach (Node parentNode in node.parents) {
                    nodeData[node.id].parentNodes.Add(nodeData[parentNode.id]);
                }
            }

            if (node.children != null) {
                foreach (Node childNode in node.children) {
                    nodeData[node.id].childNodes.Add(nodeData[childNode.id]);
                }
            }
        }

        return nodeData;
    }

    public class Node {
        public TVertex id;
        public float x;
        public float y;

        public List<Node> children = new List<Node>();
        public List<Node> parents = new List<Node>();

        public Node(TVertex id, float x, float y) {
            this.id = id;
            this.x = x;
            this.y = y;
        }

        public Vector2 NodePosition() {
            return new Vector2(x, y);
        }

        public void SetPosition(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public void MoveBy(Vector2 moveBy) {
            x += moveBy.x;
            y += moveBy.y;
        }
    }
}
