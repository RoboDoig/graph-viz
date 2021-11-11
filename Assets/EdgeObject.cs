using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObject : MonoBehaviour
{
    public int renderIndex;
    private UILineRenderer lineRenderer;
    public Transform start; // doesn't need to be public (?)
    public Transform end;
    private RectTransform startRect;
    private RectTransform endRect;
    private Vector3 lastStartPosition;
    private Vector3 lastEndPosition;

    void Awake() {
        lineRenderer = GetComponent<UILineRenderer>();
        transform.SetSiblingIndex(renderIndex);
    }

    void Start() {
        GetRects();
    }

    void GetRects() {
        startRect = start.GetComponent<RectTransform>();
        endRect = end.GetComponent<RectTransform>();
    }

    public void DefinePoints(Transform start, Transform end) {
        this.start = start;
        this.end = end;
        GetRects();
        DrawStraightLine();
    }

    // TODO - most of the time edges are static and don't need to update
    private void Update() {
        // We only need to draw if target positions changed
        if (startRect.anchoredPosition3D != lastStartPosition || endRect.anchoredPosition3D != lastEndPosition) {
            // DrawStraightLine();
            // DrawSineWave(10f, 0.5f, 100);
            DrawStepLine();
        }
        lastStartPosition = startRect.anchoredPosition3D;
        lastEndPosition = endRect.anchoredPosition3D;
    }

    private void DrawStraightLine() {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPointPosition(0, startRect.anchoredPosition3D);
        lineRenderer.SetPointPosition(1, endRect.anchoredPosition3D);
    }

    private void DrawStepLine() {
        if (start == end) {
            // If this is a loop back we need to do something different
        } else {
            lineRenderer.positionCount = 4;

            Vector3 h = end.position - start.position;
            Vector3 graphAxis = Vector3.up; // TODO - vector up only accounts for standard rotation
            float a = Mathf.Deg2Rad * (Vector3.Angle(h, graphAxis));

            Vector3 b = graphAxis * (h.magnitude * Mathf.Cos(a));
            Vector3 m = b/2; // m = midpoint straight line

            Vector3 crossingLine = h - m;

            lineRenderer.SetPointPosition(0, startRect.anchoredPosition3D);
            lineRenderer.SetPointPosition(1, m + startRect.anchoredPosition3D);
            lineRenderer.SetPointPosition(2, crossingLine + startRect.anchoredPosition3D);
            lineRenderer.SetPointPosition(3, endRect.anchoredPosition3D);
        }
    }

    // https://stackoverflow.com/questions/44692895/sinusoidal-or-other-custom-move-type-between-two-points
    private void DrawSineWave(float amplitude, float wavelength, int nPoints) {
        lineRenderer.positionCount = nPoints;
        Vector3 p1 = startRect.anchoredPosition3D;
        Vector3 p2 = endRect.anchoredPosition3D;
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

            lineRenderer.SetPointPosition(i, newPosition);
        }
    }
}
