using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderScript : MonoBehaviour
{
    private Vector3 pos;
    private SphereScript Sphere;

    // Start is called before the first frame update
    void Start()
    {
        Sphere = GameObject.Find("Sphere").GetComponent<SphereScript>();
        pos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = getDisplayPosition(pos);
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
