using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using QuikGraph.Algorithms.Search;

public class TreeGraph<TVertex, TEdge> : VisualizableGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
{
    private Node rootNode;
    private int maxDepth = int.MinValue;
    
    public Dictionary<TVertex, Node> nodeGraph {get; protected set;}

    public TreeGraph(TEdge[] edges) : base(edges)
    {

    }

    protected override void InitializeGraph()
    {
        base.InitializeGraph();

        nodeGraph = new Dictionary<TVertex, Node>();

        // Build tree + calculate depth
        int count = 0;
        DepthFirstSearchAlgorithm<TVertex, TEdge> dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(graph);
        dfs.TreeEdge += (TEdge edge) => {
            if (!nodeGraph.ContainsKey(edge.Source)) {
                nodeGraph.Add(edge.Source, new Node(edge.Source, 0, 0));
            }

            if (!nodeGraph.ContainsKey(edge.Target)) {
                nodeGraph.Add(edge.Target, new Node(edge.Target, 0, 0));
            }

            Node sourceNode = nodeGraph[edge.Source];
            Node targetNode = nodeGraph[edge.Target];
            targetNode.depth = nodeGraph[edge.Source].depth + 1;

            if (targetNode.depth > maxDepth) {
                maxDepth = targetNode.depth;
            }

            if (count == 0) {
                rootNode = sourceNode;
            }

            sourceNode.children.Add(targetNode);
            targetNode.parents.Add(sourceNode);

            count += 1;
        };
        dfs.Compute();
    }

    public override Dictionary<TVertex, NodeData> GetNodeData()
    {
        Dictionary<TVertex, NodeData> nodeData = new Dictionary<TVertex, NodeData>();

        foreach (Node node in nodeGraph.Values) {
            NodeData newNodeData = new NodeData (
                node.id,
                node.depthRank,
                node.depth
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

    protected override void CalculatePositioning()
    {
        base.CalculatePositioning();

        int[] nexts = new int[maxDepth+1];
        MinimumWS(rootNode, rootNode.depth, nexts);
        // ReingoldTilford(rootNode);
    }

    void MinimumWS(Node startNode, int depth, int[] nexts) {
        startNode.depthRank = nexts[startNode.depth];
        startNode.depth = depth;
        nexts[depth] += 1;
        foreach (Node childNode in startNode.children) {
            MinimumWS(childNode, childNode.depth, nexts);
        }
    }

    void ReingoldTilford(Node startNode) {

    }

    public class Node {
        public TVertex id;
        public int depth = 0;
        public float depthRank = 0f;
        public List<Node> children;
        public List<Node> parents;
        public int offset = 0;
        public Node ancestor;
        public int change = 0;
        public int shift = 0;
        public Node lmostSibling = null;
        public int number = 0;

        public Node(TVertex id) {
            this.id = id;
        }

        public Node(TVertex id, int depth, int depthRank) {
            this.id = id;
            this.depth = depth;
            this.depthRank = depthRank;

            children = new List<Node>();
            parents = new List<Node>();

            ancestor = this;
        }

        public Node LeftBrother() {
            Node n = null;
            if (parents != null) {
                foreach (Node node in parents[0].children) {
                    if (node == this) {
                        return n;
                    } else {
                        n = node;
                    }
                }
            }
            return n;
        }
    }
}
