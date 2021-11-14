using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;

public class TestController : MonoBehaviour
{
    public GraphRenderer graphRenderer;

    void Start() {
        Edge<int>[] edges = GraphFactory(3, 3);

        graphRenderer.InitializeGraph(edges);

        // graphRenderer.vertexAdded += (GraphRenderer.DrawnVertex v) => {
        //     Debug.Log(v.transform);
        // };

        graphRenderer.edgeAdded += (GraphRenderer.DrawnEdge e) => {
            Debug.Log(e.transform);
        };

        graphRenderer.DisplayGraph();
    }

    Edge<int>[] GraphFactory(int depth, int nChildren) {
        List<Edge<int>> edgeList = new List<Edge<int>>();
        int counter = 0;
        BuildGraphRecursive(edgeList, 0, depth, nChildren, ref counter);

        return edgeList.ToArray();
    }

    void BuildGraphRecursive(List<Edge<int>> edges, int parent, int depth, int nChildren, ref int counter) {
        int thisNode = counter;
        edges.Add(new Edge<int>(parent, thisNode));

        if (depth == 0) {
            return;
        }

        for (int i = 0; i < nChildren; i++) {
            counter++;
            BuildGraphRecursive(edges, thisNode, depth-1, nChildren, ref counter);
        }
    }
}
