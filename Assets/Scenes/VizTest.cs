using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VizTest : MonoBehaviour
{
    public RectTransform parentDisplay;

    public VertexObject vertexObjectStart;
    public VertexObject vertexObjectEnd;
    public EdgeObject edgeObject;

    void Start() {
        CreateEdge();
    }

    void CreateEdge() {
        EdgeObject newEdge = Instantiate(edgeObject, Vector3.zero, Quaternion.identity, parentDisplay);
        newEdge.transform.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        newEdge.DefinePoints(vertexObjectStart.transform, vertexObjectEnd.transform);
    }
}
