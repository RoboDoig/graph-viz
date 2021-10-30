using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObject : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform[] points;

    void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DefinePoints(Transform[] points) {
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    private void LateUpdate() {
        for (int i = 0; i < points.Length; i++) {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }
}
