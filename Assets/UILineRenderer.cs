using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    public Vector2[] points;

    public int positionCount {
        get {
            return points.Length;
        }
        set {
            points = new Vector2[value];
        }
    }

    public enum DrawType {Straight, Corner};
    public DrawType drawType;

    float width;
    float height;
    float unitWidth;
    float unitHeight;

    public float thickness = 10f;
    public int cornerVertices = 10;

    public void SetPointPosition(int index, Vector3 position) {
        points[index] = new Vector2(position.x, position.y);
        DrawMesh();
    }

    public void DrawMesh() {
        SetAllDirty();
    }

    void DrawMeshStraight(VertexHelper vh) {
        vh.Clear();

        if (points.Length < 2) {
            return;
        }

        float angle = 0;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        float previousAngle = 0;

        // Construct vertices
        for (int i = 0; i < points.Length; i++) {
            // Get angle between this point and the next
            if (i < points.Length - 1) {
                angle = GetAngle(points[i], points[i + 1]) - 90f; // Use mathf delta angle instead here?
            }

            // Initial vertex positions
            Vector3 v1 = (Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);
            Vector3 v2 = (Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);

            // If there were previous steps - no corner segments
            if (i > 0) {
                // Kite calculation
                float adjustAngle = Mathf.DeltaAngle(angle, previousAngle);
                float cornerAngle = 90 - adjustAngle / 2f;
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

            // Previous angle
            if (i < points.Length - 1) {
                previousAngle = angle;
            }
        }

        // Add triangles
        for (int i = 0; i < points.Length-1; i++) {
            int index = i * 2;
            vh.AddTriangle(index + 0, index + 3, index + 1);
            vh.AddTriangle(index + 0, index + 2, index + 3);
        }
    }

    void DrawMeshCorners(VertexHelper vh) {
        vh.Clear();

        if (points.Length < 2) {
            return;
        }

        float angle = 0;
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        float previousAngle = 0;
        Vector2 previousPosition = Vector2.zero;

        // Construct vertices
        for (int i = 0; i < points.Length; i++) {
            // Get angle between this point and the next
            if (i < points.Length - 1) {
                angle = GetAngle(points[i], points[i + 1]) - 90f; // Use mathf delta angle instead here?
            }

            // Initial vertex positions
            Vector3 v1 = (Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);
            Vector3 v2 = (Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);

            // If there were previous steps - corner segments
            if (i > 0) {
                float adjustAngle = Mathf.DeltaAngle(angle, previousAngle);

                Vector3 c1;
                Vector3 c2;
                if (Mathf.Abs(adjustAngle) <= 90f) {
                    // Kite calculation
                    float cornerAngle = 90 - adjustAngle / 2f;
                    float oLength = (v1 - new Vector3(points[i].x, points[i].y)).magnitude;
                    float tAngle = Mathf.Tan(cornerAngle*Mathf.Deg2Rad);
                    float meetLength = 0f;
                    if (tAngle != 0) {
                        meetLength = Mathf.Abs(oLength / tAngle);
                    }

                    // start of corner positions
                    c1 = (Quaternion.Euler(0, 0, previousAngle) * new Vector3(-thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);
                    c2 = (Quaternion.Euler(0, 0, previousAngle) * new Vector3(thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);
                    Vector3 adjustVector = Quaternion.Euler(0, 0, -90) * (c2 - c1).normalized;
                    c1 += adjustVector * meetLength;
                    c2 += adjustVector * meetLength;

                    // end of corner positions
                    adjustVector = Quaternion.Euler(0, 0, -90) * (v2 - v1).normalized;
                    v1 -= adjustVector * meetLength;
                    v2 -= adjustVector * meetLength;
                } else {
                    float cornerAngle = 180 - adjustAngle;
                    
                    // start of corner positions
                    c1 = (Quaternion.Euler(0, 0, previousAngle) * new Vector3(-thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);
                    c2 = (Quaternion.Euler(0, 0, previousAngle) * new Vector3(thickness / 2, 0)) + new Vector3(points[i].x, points[i].y);

                    // end of corner positions
                    Vector3 adjustVector = (c2 - c1)/2 + (v1-v2)/2;
                    v1 += adjustVector * Mathf.Sign(adjustAngle);
                    v2 += adjustVector * Mathf.Sign(adjustAngle);
                }

                vertex.position = c1; 
                vh.AddVert(vertex);
                vertex.position = c2; 
                vh.AddVert(vertex);

                // corner tri vertices
                float kinkAngle = 0f;
                float angleSign = Mathf.Sign(Vector3.SignedAngle((v2-v1), (c2-c1), Vector3.forward));
                for (int x = 1; x <= cornerVertices; x++) {
                    kinkAngle = (Mathf.DeltaAngle(GetAngle(v2, v1), GetAngle(c2, c1)) / cornerVertices) * x;

                    // TODO - if kink angles are very small, both v2-v1 and c2-c1 will be close to 0, so both rClamp and lClamp will be 0, k1 and k2 will end up being 0
                    // These clamps serve as bool multipliers, whether the kinks should angle from c1 or c2
                    float rClamp;
                    float lClamp;
                    if (angleSign >= 0) {
                        rClamp = 1f;
                        lClamp = 0f;
                    } else {
                        rClamp = 0f;
                        lClamp = 1f;
                    }

                    Vector3 rPos = (Quaternion.Euler(0, 0, 180 - kinkAngle) * (c2 - c1) + c2) * rClamp; // If kink is going right away from previous line
                    Vector3 lPos = (Quaternion.Euler(0, 0, 180 - kinkAngle) * (c1 - c2) + c1) * lClamp; // If going left
                    Vector3 k1 = rPos + lPos;
                    Vector3 k2 = (c2 * rClamp + c1 * lClamp);

                    // if (points)
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
            if (i < points.Length - 1) {
                previousAngle = angle;
                previousPosition = points[i];
            }
        }

        // Add triangles
        for (int i = 0; i < points.Length-1; i++) {
            int index = i * (4 + cornerVertices * 2);
            vh.AddTriangle(index + 0, index + 3, index + 1);
            vh.AddTriangle(index + 0, index + 2, index + 3);
            int kStart = index + 2;
            if (i < points.Length - 2) {
                for (int j = 0; j < cornerVertices; j++) {
                    vh.AddTriangle(kStart, kStart + 1, kStart + 2);
                    kStart = kStart + 2;
                }
            }
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (drawType == DrawType.Straight) {
            DrawMeshStraight(vh);
        } else if (drawType == DrawType.Corner) {
            DrawMeshCorners(vh);
        } else {
            DrawMeshCorners(vh);
        }
    }

    public float GetAngle(Vector2 me, Vector2 target) {
        return (float)(Mathf.Atan2(target.y - me.y, target.x - me.x) * (180/Mathf.PI));
    }
}
