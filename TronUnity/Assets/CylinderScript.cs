using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderScript : MonoBehaviour
{
    private Vector3 start, end;
    private SphereScript Sphere;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetStartEnd(Vector3 s, Vector3 e)
    {
        Sphere = GameObject.Find("Sphere").GetComponent<SphereScript>();
        start = s;
        end = e;
        //MeshRenderer mesh = this.GetComponent<MeshRenderer>();
        //mesh.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        float w = 0.01f;
        Vector3 s = getDisplayPosition(start);
        Vector3 e = getDisplayPosition(end);

        float len = Vector3.Distance(s, e);
        Vector3 offset = new Vector3(e.x - s.x, e.y - s.y, e.z - s.z);
        Vector3 mid = new Vector3(0.5f * (s.x + e.x), 0.5f * (s.y + e.y), 0.5f * (s.z + e.z));

        this.transform.position = mid;
        this.transform.up = offset;
        this.transform.localScale = new Vector3(w, len / 2.0f, w);
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
