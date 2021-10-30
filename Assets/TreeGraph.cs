using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using QuikGraph.Algorithms.Search;

public class TreeGraph<TVertex, TEdge> : VisualizableGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
{
    private Node rootNode;
    private int maxDepth = int.MinValue;

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

    protected override void CalculatePositioning()
    {
        base.CalculatePositioning();

        int[] nexts = new int[maxDepth+1];
        MinimumWS(rootNode, rootNode.depth, nexts);
    }

    void MinimumWS(Node startNode, int depth, int[] nexts) {
        startNode.depthRank = nexts[startNode.depth];
        startNode.depth = depth;
        nexts[depth] += 1;
        foreach (Node childNode in startNode.children) {
            MinimumWS(childNode, childNode.depth, nexts);
        }
    }
}
