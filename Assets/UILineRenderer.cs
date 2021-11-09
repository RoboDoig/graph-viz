using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    public List<Vector2> points;

    float width;
    float height;
    float unitWidth;
    float unitHeight;

    public float thickness = 10f;
    public int cornerVertices = 10;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        if (points.Count < 2) {
            return;
        }

        float angle = 0;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        float previousAngle = 0;

        // Construct vertices
        for (int i = 0; i < points.Count; i++) {
            // Get angle between this point and the next
            if (i < points.Count - 1) {
                angle = GetAngle(points[i], points[i + 1]) - 90f; // Use mathf delta angle instead here?
            }

            // Initial vertex positions
            Vector3 v1 = (Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);
            Vector3 v2 = (Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);

            // If there were previous steps - no corner segments
            // if (i > 0) {
            //     // Kite calculation
            //     float adjustAngle = Mathf.DeltaAngle(angle, previousAngle) / 2f;
            //     float cornerAngle = 90 - adjustAngle;
            //     float oLength = (v1 - new Vector3(points[i].x, points[i].y)).magnitude;
            //     float tAngle = Mathf.Tan(cornerAngle*Mathf.Deg2Rad);
            //     float meetLength = 0f;
            //     if (tAngle != 0) {
            //         meetLength = oLength / tAngle;
            //     }
            //     Vector3 adjustVector = Quaternion.Euler(0, 0, -90) * (v2 - v1).normalized;

            //     v1 += adjustVector * meetLength;
            //     v2 -= adjustVector * meetLength;
            // }

            // If there were previous steps - corner segments
            if (i > 0) {
                float adjustAngle = Mathf.DeltaAngle(angle, previousAngle) / 2f;
                float cornerAngle = 90 - adjustAngle;
                float oLength = (v1 - new Vector3(points[i].x, points[i].y)).magnitude;
                float tAngle = Mathf.Tan(cornerAngle*Mathf.Deg2Rad);
                float meetLength = 0f;
                if (tAngle != 0) {
                    meetLength = oLength / tAngle;
                }

                // start of corner positions
                Vector3 c1 = (Quaternion.Euler(0, 0, previousAngle) * new Vector3(-thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);
                Vector3 c2 = (Quaternion.Euler(0, 0, previousAngle) * new Vector3(thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);
                Vector3 adjustVector = Quaternion.Euler(0, 0, -90) * (c2 - c1).normalized * Mathf.Sign(GetAngle(v2, v1));
                c1 += adjustVector * meetLength;
                c2 += adjustVector * meetLength;
                Vector3 cmid = (c2 - c1) / 2f;

                // end of corner positions
                adjustVector = Quaternion.Euler(0, 0, -90) * (v2 - v1).normalized * Mathf.Sign(GetAngle(v2, v1));
                v1 -= adjustVector * meetLength;
                v2 -= adjustVector * meetLength;

                vertex.position = c1; 
                vh.AddVert(vertex);
                vertex.position = c2; 
                vh.AddVert(vertex);

                // corner lines
                float kinkAngle = 0f;
                Debug.Log((v1-c1).magnitude);
                Debug.Log( Mathf.Clamp ( Mathf.Round( (v1-c1).magnitude ), 0, 1) );
                for (int x = 1; x <= cornerVertices; x++) {
                    kinkAngle = (Mathf.DeltaAngle(GetAngle(v2, v1), GetAngle(c2, c1)) / cornerVertices) * x;
                    float rClamp = Mathf.Clamp ( Mathf.Round( (v1-c1).magnitude ), 0, 1); // These clamps serve as bool multipliers, whether the kinks should angle from c1 or c2
                    float lClamp = Mathf.Clamp ( Mathf.Round( (v2-c2).magnitude ), 0, 1);
                    Vector3 rPos = (Quaternion.Euler(0, 0, 180 - kinkAngle) * (c2 - c1) + c2) * rClamp; // If kink is going right away from previous line
                    Vector3 lPos = (Quaternion.Euler(0, 0, 180 - kinkAngle) * (c1 - c2) + c1) * lClamp; // If going left
                    Vector3 k1 = (rPos + lPos) * rClamp;
                    Vector3 k2 = c2 * rClamp + c1 * lClamp;

                    vertex.position = k1; 
                    vh.AddVert(vertex);
                    vertex.position = k2; 
                    vh.AddVert(vertex);
                }
            }

            // Add vertices
            vertex.position = v1; 
            vh.AddVert(vertex);
            vertex.position = v2; 
            vh.AddVert(vertex);

            // Previous angle
            if (i < points.Count - 1) {
                previousAngle = angle;
            }
        }

        // Add Triangles
        // // segment block
        // vh.AddTriangle(0, 3, 1);
        // vh.AddTriangle(0, 2, 3);

        // // kink triangles
        // vh.AddTriangle(2, 3, 4);
        // vh.AddTriangle(4, 5, 6);
        // vh.AddTriangle(6, 7, 8);

        // vh.AddTriangle(10, 13, 11);
        // vh.AddTriangle(10, 12, 13);

        // // kink triangles
        // vh.AddTriangle(12, 13, 14);
        // vh.AddTriangle(14, 15, 16);
        // vh.AddTriangle(16, 17, 18);

        // Add triangles
        for (int i = 0; i < points.Count-1; i++) {
            int index = i * (4 + cornerVertices * 2);
            vh.AddTriangle(index + 0, index + 3, index + 1);
            vh.AddTriangle(index + 0, index + 2, index + 3);
            int kStart = index + 2;
            for (int j = 0; j < cornerVertices; j++) {
                vh.AddTriangle(kStart, kStart + 1, kStart + 2);
                kStart = kStart + 2;
            }
        }
    }

    void DrawVerticesForSegment(Vector2 startPoint, Vector2 endPoint, float angle, VertexHelper vh) {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
            vertex.position += new Vector3(startPoint.x, startPoint.y);
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 1);
            vertex.position += new Vector3(endPoint.x, endPoint.y);
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 1);
            vertex.position += new Vector3(endPoint.x, endPoint.y);
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
            vertex.position += new Vector3(startPoint.x, startPoint.y);
            vh.AddVert(vertex);
    }

    public float GetAngle(Vector2 me, Vector2 target) {
        return (float)(Mathf.Atan2(target.y - me.y, target.x - me.x) * (180/Mathf.PI));
    }
}
