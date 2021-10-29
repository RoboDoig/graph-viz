using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    public Transform[] points;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 10f;
        lr.endWidth = 10f;
    }

    public void SetUpLine(Transform[] points) {
        lr.positionCount = points.Length;
        this.points = points;
    }

    private void Update() {
        for (int i = 0; i < points.Length; i++) {
            lr.SetPosition(i, points[i].position);
        }
    }
}
