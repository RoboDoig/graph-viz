using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;

public class RadialTreeGraph<TVertex, TEdge> : TreeGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
{
    public RadialTreeGraph(TEdge[] edges) : base(edges)
    {

    }

    public override Dictionary<TVertex, NodeData> GetNodeData() {
        Dictionary<TVertex, NodeData> nodeData = new Dictionary<TVertex, NodeData>();
        Dictionary<int, float> maxDepthRank = new Dictionary<int, float>();

        // Add node data
        foreach (Node node in nodeGraph.Values) {
            NodeData newNodeData = new NodeData (
                node.id,
                node.depthRank,
                node.depth
            );

            Debug.Log(node.depthRank);

            nodeData.Add(newNodeData.id, newNodeData);

            if (!maxDepthRank.ContainsKey(node.depth)) {
                maxDepthRank.Add(node.depth, 0);
            }
            if (node.depthRank > maxDepthRank[node.depth]) {
                maxDepthRank[node.depth] = node.depthRank;
            }
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

        // Calculate radial position
        foreach (Node node in nodeGraph.Values) {
            float theta;
            if (maxDepthRank[node.depth] == 0) {
                theta = 0f;
            } else {
                theta = (Mathf.PI * 2) / maxDepthRank[node.depth];
            }
            float angle = theta * node.depthRank;

            nodeData[node.id].x = node.depth * Mathf.Cos(angle);
            nodeData[node.id].y = node.depth * Mathf.Sin(angle);
        }

        return nodeData;
    }

    protected override void CalculatePositioning()
    {
        int[] nexts = new int[maxDepth + 1];
        // MinimumWS(rootNode, rootNode.depth, nexts);
        ReingoldTilford(rootNode);
    }

    void ReingoldTilfordRadial(Node startNode) {
        
    }
}
