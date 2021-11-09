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

                // end of corner positions
                adjustVector = Quaternion.Euler(0, 0, -90) * (v2 - v1).normalized * Mathf.Sign(GetAngle(v2, v1));
                v1 -= adjustVector * meetLength;
                v2 -= adjustVector * meetLength;

                vertex.position = c1; 
                vh.AddVert(vertex);
                vertex.position = c2; 
                vh.AddVert(vertex);

                // corner lines

                // float kinkAngle = (GetAngle(v2, v1) - 180 - GetAngle(c2, c1));
                float kinkAngle = Mathf.DeltaAngle(GetAngle(v2, v1), GetAngle(c2, c1)) / 4;
                Vector3 lim1 = Quaternion.Euler(0, 0, 180 - kinkAngle) * (c2 - c1) + c2;
                Vector3 lim2 = c2;

                vertex.position = lim1; 
                vh.AddVert(vertex);
                vertex.position = lim2; 
                vh.AddVert(vertex);

                Vector3 lim3 = Quaternion.Euler(0, 0, 180 - kinkAngle*2) * (c2 - c1) + c2;
                Vector3 lim4 = c2;

                vertex.position = lim3; 
                vh.AddVert(vertex);
                vertex.position = lim4; 
                vh.AddVert(vertex);
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
        vh.AddTriangle(0, 3, 1);
        vh.AddTriangle(0, 2, 3);

        vh.AddTriangle(2, 4, 5);
        vh.AddTriangle(5, 4, 6);
        vh.AddTriangle(7, 6, 8);

        // Add triangles
        // for (int i = 0; i < points.Count-1; i++) {
        //     int index = i * 2;
        //     vh.AddTriangle(index + 0, index + 3, index + 1);
        //     vh.AddTriangle(index + 0, index + 2, index + 3);
        // }
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
