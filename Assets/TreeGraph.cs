using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using QuikGraph.Algorithms.Search;

public class TreeGraph<TVertex, TEdge> : VisualizableGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
{
    protected Node rootNode;
    protected int maxDepth = int.MinValue;
    
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

        // Add each node 
        foreach (Node node in nodeGraph.Values) {
            NodeData newNodeData = new NodeData (
                node.id,
                node.depthRank,
                node.depth
            );

            nodeData.Add(newNodeData.id, newNodeData);
        }

        // Determine each node's parents and children
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
        int[] nexts = new int[maxDepth+1];
        // MinimumWS(rootNode, rootNode.depth, nexts); // TODO - this can't draw separate graphs rn because it runs out of children!
        ReingoldTilford(rootNode); // TODO - same here
    }

    protected void MinimumWS(Node startNode, int depth, int[] nexts) {
        startNode.depthRank = nexts[startNode.depth];
        startNode.depth = depth;
        nexts[depth] += 1;
        foreach (Node childNode in startNode.children) {
            MinimumWS(childNode, childNode.depth, nexts);
        }
    }

    protected void ReingoldTilford(Node startNode) {
        CalculateInitialX(startNode);

        CalculateFinalPositions(startNode, 0);
    }

    void CalculateInitialX(Node startNode) {
        foreach (Node childNode in startNode.children) {
            CalculateInitialX(childNode);
        }

        // if no children
        if (startNode.IsLeaf()) {
            // if there is a previous sibling, iterate on previous sibling rank
            if (!startNode.IsLeftMost()) {
                startNode.depthRank = startNode.GetPreviousSibling().depthRank + 1;
            } else {
                startNode.depthRank = 0;
            }
        }
        // if there is only one child
        else if (startNode.children.Count == 1) {
            // if there is no previous sibling, center its rank on child rank
            if (startNode.IsLeftMost()) {
                startNode.depthRank = startNode.children[0].depthRank;
            } else {
                startNode.depthRank = startNode.GetPreviousSibling().depthRank + 1;
                startNode.mod = startNode.depthRank - startNode.children[0].depthRank;
            }
        }
        else {
            Node leftChild = startNode.GetLeftMostChild();
            Node rightChild = startNode.GetRightMostChild();
            float mid = (leftChild.depthRank + rightChild.depthRank) / 2f;

            if (startNode.IsLeftMost()) {
                startNode.depthRank = mid;
            } else {
                startNode.depthRank = startNode.GetPreviousSibling().depthRank + 1;
                startNode.mod = startNode.depthRank - mid;
            }
        }

        if (startNode.children.Count > 0 && !startNode.IsLeftMost()) {
            CheckForConflicts(startNode);
        }
    }

    void CheckForConflicts(Node startNode) {
        float minDistance = 1f;
        float shiftValue = 0f;

        Dictionary<int, float> nodeContour = new Dictionary<int, float>();
        GetLeftContour(startNode, 0, ref nodeContour);

        Node sibling = startNode.GetLeftMostSibling();
        while (sibling != null && sibling != startNode) {
            Dictionary<int, float> siblingContour = new Dictionary<int, float>();
            GetRightContour(sibling, 0, ref siblingContour);

            for (int level = startNode.depth; level <= Mathf.Min(siblingContour.Keys.Max(), nodeContour.Keys.Max()); level++) {
                float distance = nodeContour[level] - siblingContour[level];
                if (distance + shiftValue < minDistance) {
                    shiftValue = minDistance - distance;
                }
            }

            if (shiftValue > 0) {
                startNode.depthRank += shiftValue;
                startNode.mod += shiftValue;

                CenterNodesBetween(startNode, sibling);

                shiftValue = 0;
            }

            sibling = sibling.GetNextSibling();
        }
    }

    void CalculateFinalPositions(Node node, float modSum) {
        node.depthRank += modSum;
        modSum += node.mod;

        foreach (Node child in node.children) {
            CalculateFinalPositions(child, modSum);
        }
    }

    void CenterNodesBetween(Node leftNode, Node rightNode) {
        int leftIndex = leftNode.parents[0].children.IndexOf(rightNode);
        int rightIndex= leftNode.parents[0].children.IndexOf(leftNode);

        int numNodesBetween = (rightIndex - leftIndex) - 1;

        if (numNodesBetween > 0) {
            float distanceBetweenNodes = (leftNode.depthRank - rightNode.depthRank) / (numNodesBetween + 1);

            int count = 1;
            for (int i = leftIndex + 1; i < rightIndex; i++) {
                Node middleNode = leftNode.parents[0].children[i];

                float desiredX = rightNode.depthRank + (distanceBetweenNodes * count);
                float offset = desiredX - middleNode.depthRank;
                middleNode.depthRank += offset;
                middleNode.mod += offset;

                count++;
            }

            CheckForConflicts(leftNode);
        }
    }

    void GetLeftContour(Node node, float modSum, ref Dictionary<int, float> values) {
        if (!values.ContainsKey(node.depth)) {
            values.Add(node.depth, node.depthRank + modSum);
        } else {
            values[node.depth] = Mathf.Min(values[node.depth], node.depthRank + modSum);
        }

        modSum += node.mod;
        foreach (Node child in node.children) {
            GetLeftContour(child, modSum, ref values);
        }
    }

    void GetRightContour(Node node, float modSum, ref Dictionary<int, float> values) {
        if (!values.ContainsKey(node.depth)) {
            values.Add(node.depth, node.depthRank + modSum);
        } else {
            values[node.depth] = Mathf.Max(values[node.depth], node.depthRank + modSum);
        }

        modSum += node.mod;
        foreach (Node child in node.children) {
            GetRightContour(child, modSum, ref values);
        }
    }

    public class Node {
        public TVertex id;
        public int depth = 0;
        public float depthRank = 0f;
        public float mod = 0f;
        public List<Node> children;
        public List<Node> parents;

        public Node(TVertex id) {
            this.id = id;
        }

        public Node(TVertex id, int depth, int depthRank) {
            this.id = id;
            this.depth = depth;
            this.depthRank = depthRank;

            children = new List<Node>();
            parents = new List<Node>();
        }

        public bool IsLeaf() {
            return this.children.Count == 0;
        }

        public bool IsLeftMost() {
            if (this.parents.Count == 0) {
                return true;
            }

            return this.parents[0].children[0] == this;
        }

        public bool IsRightMost() {
            if (this.parents.Count == 0) {
                return true;
            }

            return this.parents[0].children[this.parents[0].children.Count - 1] == this;
        }

        public Node GetPreviousSibling() {
            if (this.parents.Count == 0 || this.IsLeftMost()) {
                return null;
            }

            return this.parents[0].children[this.parents[0].children.IndexOf(this) - 1];
        }

        public Node GetNextSibling() {
            if (this.parents.Count == 0 || this.IsRightMost()) {
                return null;
            }

            return this.parents[0].children[this.parents[0].children.IndexOf(this) + 1];
        }

        public Node GetLeftMostChild() {
            if (this.children.Count == 0) {
                return null;
            }

            return this.children[0];
        }

        public Node GetRightMostChild() {
            if (this.children.Count == 0) {
                return null;
            }

            return this.children[children.Count - 1];
        }

        public Node GetLeftMostSibling() {
            if (this.parents.Count == 0) {
                return null;
            }

            if (this.IsLeftMost()) {
                return this;
            }

            return this.parents[0].children[0];
        }
    }
}
