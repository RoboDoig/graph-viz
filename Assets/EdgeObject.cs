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
        this.points = points;
    }

    private void Update() {
        // DrawStraightLine();
        DrawSineWave(0.05f, 0.1f, 200);
    }

    private void DrawStraightLine() {
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++) {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }

    // TODO - only sort of works
    private void DrawSineWave(float amplitude, float wavelength, int nPoints) {
        lineRenderer.positionCount = nPoints;
        Vector3 p1 = points[0].position;
        Vector3 p2 = points[points.Length - 1].position;
        Vector3 moveVec = (p2 - p1);
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);
        float a = Mathf.Atan2(dy, dx);
        // float cosa = Mathf.Cos(a);
        // float sina = Mathf.Sin(a);

        // Vector3 pointPos = Vector3.zero;
        // for (int i = 0; i < lineRenderer.positionCount; i++) {
        //     // untransformed wave
        //     float tx = (float)i / (float)nPoints;
        //     float ty = amplitude * Mathf.Sin(tx * 2 * Mathf.PI) / wavelength;

        //     // apply transform
        //     float x = p1.x + distance * (tx * cosa - ty * sina);
        //     float y = p1.y + distance * (tx * sina + ty * cosa);

        //     lineRenderer.SetPosition(i, new Vector3(x, y, p1.z));
        // }

        for (int i = 0; i < lineRenderer.positionCount; i++) {
            float tx = (float)i / (float)nPoints; // proportion along line
            float ty = moveVec.y * tx;

            float x = p1.x + (moveVec * tx).x;
            float y = p1.y + (moveVec * tx).y;

            lineRenderer.SetPosition(i, new Vector3(x, y, p1.z));
        }
    }
}
