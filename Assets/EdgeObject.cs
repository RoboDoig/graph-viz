using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObject : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform start; // TODO - user can supply more than 2 points
    private Transform end;

    void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DefinePoints(Transform start, Transform end) {
        this.start = start;
        this.end = end;
        DrawStraightLine();
    }

    // TODO - most of the time edges are static and don't need to update
    private void Update() {
        // DrawStraightLine();
        DrawSineWave(1f, 0.5f, 1000);
        // DrawStepLine();
    }

    private void DrawStraightLine() {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, end.position);
        // for (int i = 0; i < points.Length; i++) {
        //     lineRenderer.SetPosition(i, points[i].position);
        // }
    }

    private void DrawStepLine() {
        lineRenderer.positionCount = 4;

        Vector3 h = end.position - start.position;
        Vector3 graphAxis = Vector3.up; // TODO - vector up only accounts for standard rotation
        float a = Mathf.Deg2Rad * (Vector3.Angle(h, graphAxis));

        Vector3 b = graphAxis * (h.magnitude * Mathf.Cos(a));
        Vector3 m = b/2; // m = midpoint straight line

        Vector3 crossingLine = h - m;

        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, m + start.position);
        lineRenderer.SetPosition(2, crossingLine + start.position);
        lineRenderer.SetPosition(3, end.position);
    }

    // https://stackoverflow.com/questions/44692895/sinusoidal-or-other-custom-move-type-between-two-points
    private void DrawSineWave(float amplitude, float wavelength, int nPoints) {
        lineRenderer.positionCount = nPoints;
        Vector3 p1 = start.position;
        Vector3 p2 = end.position;
        Vector3 moveVec = (p2 - p1);
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);
        float a = Mathf.Atan2(dy, dx);

        Vector3 newPosition = new Vector3(0, 0, p1.z);
        for (int i = 0; i < lineRenderer.positionCount; i++) {
            float tx = (float)i / (float)nPoints * distance; // proportion/distance along line
            
            newPosition.x = p1.x + Mathf.Cos(a) * tx;
            newPosition.y = p1.y + Mathf.Sin(a) * tx;

            float deviation = Mathf.Sin(tx * Mathf.PI / wavelength) * amplitude;

            newPosition.x += Mathf.Sin(a) * deviation;
            newPosition.y -= Mathf.Cos(a) * deviation;

            lineRenderer.SetPosition(i, newPosition);
        }
    }
}
