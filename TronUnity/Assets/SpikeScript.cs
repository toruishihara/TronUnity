using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    private SphereScript Sphere;
    private TronScript tron;
    public int num = 8;
    public float radius = 0.2f;
    public float radius2 = 0.1f;
    public float height = 1f;
    public float height2 = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Set(TronScript tr, int n, float r, float h)
    {
        Sphere = GameObject.Find("Sphere").GetComponent<SphereScript>();
        tron = tr;
        num = n;
        radius = 3 * r;
        height = h;
        height2 = r;
        radius2 = r;
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        Vector3[] ps = new Vector3[3 + 2 * num];
        ps[1].y = h;
        ps[2].y = height - height2;
        // large circle
        for (int i = 0; i < num; ++i)
        {
            float angle = 2 * Mathf.PI * i / (float)num;
            ps[3 + i].y = height;
            ps[3 + i].x = radius2 * Mathf.Cos(angle);
            ps[3 + i].z = radius2 * Mathf.Sin(angle);
        }
        // small circle
        for (int i = 0; i < num; ++i)
        {
            float angle = 2 * Mathf.PI * i / (float)num;
            ps[3 + num + i].y = height - height2;
            ps[3 + num + i].x = radius * Mathf.Cos(angle);
            ps[3 + num + i].z = radius * Mathf.Sin(angle);
        }

        mesh.vertices = ps;

        int[] ints = new int[3 * 3 * num];
        // long cone
        for (int i = 0; i < num; ++i)
        {
            int j = i + 1;
            if (j >= num) { j = 0; }
            ints[3 * i] = 0;
            ints[3 * i + 1] = 3 + i;
            ints[3 * i + 2] = 3 + j;
        }
        // top cap
        int offset = 3 * num;
        for (int i = 0; i < num; ++i)
        {
            int j = i + 1;
            if (j >= num) { j = 0; }
            ints[offset + 3 * i] = 1;
            ints[offset + 3 * i + 1] = 3 + num + j;
            ints[offset + 3 * i + 2] = 3 + num + i;
        }
        // top cone
        offset = 2 * 3 * num;
        for (int i = 0; i < num; ++i)
        {
            int j = i + 1;
            if (j >= num) { j = 0; }
            ints[offset + 3 * i] = 2;
            ints[offset + 3 * i + 1] = 3 + num + i;
            ints[offset + 3 * i + 2] = 3 + num + j;
        }
        mesh.triangles = ints;

        Vector3 p = tron.Position;
        this.transform.up = p;
        this.transform.position = p;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 p2 = getDisplayPosition(tron.Position);
        this.transform.up = p2;
        this.transform.position = p2;
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