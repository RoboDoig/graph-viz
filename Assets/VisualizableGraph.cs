using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;
using QuikGraph.Utils;
using QuikGraph.Predicates;

public class VisualizableGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
{
    public AdjacencyGraph<TVertex, TEdge> graph {get; protected set;}

    public VisualizableGraph(TEdge[] edges) {
        graph = edges.ToAdjacencyGraph<TVertex, TEdge>();

        InitializeGraph();
        CalculatePositioning();
    }

    protected virtual void InitializeGraph() {

    }

    protected virtual void CalculatePositioning() {

    }

    public virtual Dictionary<TVertex, NodeData> GetNodeData() {
        Dictionary<TVertex, NodeData> nodeData = new Dictionary<TVertex, NodeData>();

        return nodeData;
    }

    public class NodeData {
        public TVertex id;
        public float x;
        public float y;
        public List<NodeData> parentNodes;
        public List<NodeData> childNodes;
        public bool isLeaf = false;

        public NodeData(TVertex id, float x, float y) {
            this.id = id;
            this.x = x;
            this.y = y;
            parentNodes = new List<NodeData>();
            childNodes = new List<NodeData>();
        }
    }
}
