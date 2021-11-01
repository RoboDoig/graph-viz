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
        DrawSineWave(1f, 0.5f, 1000);
    }

    private void DrawStraightLine() {
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++) {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }

    // https://stackoverflow.com/questions/44692895/sinusoidal-or-other-custom-move-type-between-two-points
    private void DrawSineWave(float amplitude, float wavelength, int nPoints) {
        lineRenderer.positionCount = nPoints;
        Vector3 p1 = points[0].position;
        Vector3 p2 = points[points.Length - 1].position;
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
