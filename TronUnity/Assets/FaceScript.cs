using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceScript : MonoBehaviour
{
    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;

    public FaceScript()
    {
    }
    public void SetPoints(Vector3 a, Vector3 b, Vector3 c)
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = new Vector3[] { a, b, c };
        mesh.triangles = new int[] {
            0, 1, 2,
            0, 2, 1,
        };
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
