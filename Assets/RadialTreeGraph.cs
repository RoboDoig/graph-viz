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

        // Add node data
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

        nodeData[rootNode.id].x = 0;
        nodeData[rootNode.id].y = 0;
        RadialPositions(nodeData[rootNode.id], 0f, Mathf.PI * 2);

        // foreach (NodeData node in nodeData.Values) {
        //     // Calculate radial position
        //     RadialPositions(node, 0f, Mathf.PI * 2);
        // }

        return nodeData;
    }

    protected override void CalculatePositioning()
    {
        int[] nexts = new int[maxDepth + 1];
        // MinimumWS(rootNode, rootNode.depth, nexts);
        ReingoldTilford(rootNode);
    }

    void RadialPositions(NodeData startNode, float alpha, float beta) {
        float d = (float)nodeGraph[startNode.id].depth;
        float theta = alpha;
        float radius = 1 + (d * Mathf.PI);
        
        int nTreeLeaves = CountLeavesInTree(startNode);
        foreach(NodeData childNode in startNode.childNodes) {
            int nLeaves = CountLeavesInTree(childNode);
            float mu = theta + ((float)nLeaves / (float)nTreeLeaves * (beta - alpha));

            float x = radius * Mathf.Cos((theta + mu) / 2f);
            float y = radius * Mathf.Sin((theta + mu) / 2f);

            childNode.x = x;
            childNode.y = y;

            Debug.Log(childNode.x + ", " + childNode.y);

            if (childNode.childNodes.Count > 0) {
                RadialPositions(childNode, theta, mu);
            }

            theta = mu;
        }
    }

    void ReingoldTilfordRadial(Node startNode) {
        
    }

    int CountLeavesInTree(NodeData node) {
        if (node.childNodes.Count == 0) {
            return 1;
        }

        int total = 0;
        foreach(NodeData childNode in node.childNodes) {
            total += CountLeavesInTree(childNode);
        }

        return total;
    }
}
