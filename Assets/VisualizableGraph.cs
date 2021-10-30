using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using QuikGraph.Utils;
using QuikGraph.Predicates;

public class VisualizableGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
{
    public AdjacencyGraph<TVertex, TEdge> graph {get; protected set;}
    public Dictionary<TVertex, Node> nodeGraph {get; protected set;}

    public VisualizableGraph(TEdge[] edges) {
        graph = edges.ToAdjacencyGraph<TVertex, TEdge>();

        InitializeGraph();
        CalculatePositioning();
    }

    protected virtual void InitializeGraph() {

    }

    protected virtual void CalculatePositioning() {

    }

    public virtual void Draw() {
        
    }

    public class Node {
        public TVertex id;
        public int depth;
        public int depthRank;

        public Node(TVertex id, int depth, int depthRank) {
            this.id = id;
            this.depth = depth;
            this.depthRank = depthRank;
        }
    }
}
