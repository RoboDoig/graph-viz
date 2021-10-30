using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using QuikGraph.Algorithms.Search;

public class HierarchicalGraph<TVertex, TEdge> : VisualizableGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
{
    public HierarchicalGraph(TEdge[] edges) : base(edges)
    {

    }

    // Calculate depth and depth ranks of vertices and store them in the node graph
    protected override void InitializeGraph()
    {
        base.InitializeGraph();

        nodeGraph = new Dictionary<TVertex, Node>();

        // Depth calculation
        DepthFirstSearchAlgorithm<TVertex, TEdge> dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(graph);
        dfs.TreeEdge += (TEdge edge) => {
            if (!nodeGraph.ContainsKey(edge.Source)) {
                nodeGraph.Add(edge.Source, new Node(edge.Source, 0, 0));
            }

            if (!nodeGraph.ContainsKey(edge.Target)) {
                nodeGraph.Add(edge.Target, new Node(edge.Target, 0, 0));
            }

            nodeGraph[edge.Target].depth = nodeGraph[edge.Source].depth + 1;
        };
        dfs.Compute(); 

        // Depth rank calculation
        Dictionary<int, int> depthRanks = new Dictionary<int, int>();
        dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(graph);
        dfs.DiscoverVertex += (TVertex vertex) => {
            if (!depthRanks.ContainsKey(nodeGraph[vertex].depth)) {
                depthRanks.Add(nodeGraph[vertex].depth, 0);
            }

            nodeGraph[vertex].depthRank = depthRanks[nodeGraph[vertex].depth];
            depthRanks[nodeGraph[vertex].depth] += 1;
        };
        dfs.Compute();
    }

    protected override void CalculatePositioning() {

    }
}
