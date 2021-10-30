using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using QuikGraph.Utils;
using QuikGraph.Predicates;

public class VisualizableGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
{
    protected AdjacencyGraph<TVertex, TEdge> graph;

    public VisualizableGraph(TEdge[] edges) {
        graph = edges.ToAdjacencyGraph<TVertex, TEdge>();
    }
}
