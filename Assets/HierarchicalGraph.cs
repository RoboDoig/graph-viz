using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;

public class HierarchicalGraph<TVertex, TEdge> : VisualizableGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
{
    public HierarchicalGraph(TEdge[] edges) : base(edges)
    {
        
    }

    void CalculatePositioning() {
        
    }

    class Node {
        int id;
        int depth;
        int depthRank;

        public Node(int id, int depth, int depthRank) {
            this.id = id;
            this.depth = depth;
            this.depthRank = depthRank;
        }
    }
}
