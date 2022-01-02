using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceScript : MonoBehaviour
{
    private SphereScript Sphere;
    private Vector3[] positions;

    public FaceScript()
    {
    }
    public void SetPoints(Vector3 a, Vector3 b, Vector3 c)
    {
        positions = new Vector3[3];
        positions[0] = a;
        positions[1] = b;
        positions[2] = c;

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = new Vector3[] { a, b, c };
        mesh.triangles = new int[] {
            0, 1, 2,
            0, 2, 1,
        };
        //rend.enabled = true;
        Sphere = GameObject.Find("Sphere").GetComponent<SphereScript>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (positions == null) { return; }
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        for (int i=0;i<3;++i)
        {
            Vector3 p = positions[i];
            Vector3 p1 = getDisplayPosition(p);
            vertices[i] = p1;
        }
        mesh.vertices = vertices;
    }

    private Vector3 getDisplayPosition(Vector3 p)
    {
        Vector3 pole_x = Sphere.PoleX;
        Vector3 pole_y = Sphere.PoleY;
        Vector3 pole_z = Sphere.PoleZ;

        Vector3 p1;
        p1.x = Vector3.Dot(p, pole_x);
        p1.y = Vector3.Dot(p, pole_y);
        p1.z = Vector3.Dot(p, pole_z);
        return p1;
    }
}