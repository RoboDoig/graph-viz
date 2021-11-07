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
        List<Vector3> previousPositions = new List<Vector3>();
        float previousAngle = 0;

        // Construct vertices
        for (int i = 0; i < points.Count; i++) {
            // Get angle between this point and the next
            if (i < points.Count - 1) {
                angle = GetAngle(points[i], points[i + 1]) - 90f;
            }

            // Initial vertex positions
            Vector3 v1 = (Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);
            Vector3 v2 = (Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);

            // If there were previous predictions
            if (previousPositions.Count > 0) {
                float adjustAngle = Mathf.DeltaAngle(angle, previousAngle) / 2f;
                float cornerAngle = 90 - adjustAngle;
                float oLength = (v1 - new Vector3(points[i].x, points[i].y)).magnitude;
                float tAngle = Mathf.Tan(cornerAngle*Mathf.Deg2Rad);
                float meetLength = 0f;
                if (tAngle != 0) {
                    meetLength = oLength / tAngle;
                }
                Vector3 adjustVector = Quaternion.Euler(0, 0, -90) * (v2 - v1).normalized;

                v1 += adjustVector * meetLength;
                v2 -= adjustVector * meetLength;
            }

            // Add vertices
            vertex.position = v1; 
            vh.AddVert(vertex);
            vertex.position = v2; 
            vh.AddVert(vertex);

            // Predicted next positions
            if (i < points.Count - 1) {
                previousPositions.Clear();
                previousPositions.Add((Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0)) + new Vector3(points[i+1].x, points[i+1].y));
                previousPositions.Add((Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0)) + new Vector3(points[i+1].x, points[i+1].y));
                previousAngle = angle;
            }
        }

        // Add triangles
        for (int i = 0; i < points.Count-1; i++) {
            int index = i * 2;
            vh.AddTriangle(index + 0, index + 3, index + 1);
            vh.AddTriangle(index + 0, index + 2, index + 3);
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
