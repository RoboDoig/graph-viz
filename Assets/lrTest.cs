using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lrTest : MonoBehaviour
{
    LineRenderer lr;
    Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        lr = GetComponent<LineRenderer>();
        lr.BakeMesh(mesh);
    }

    // Update is called once per frame
    void Update()
    {
        // foreach (Vector3 vertex in mesh.vertices) {
        //     Debug.Log(vertex);
        // }
    }

    void OnValidate() {

    }
}
