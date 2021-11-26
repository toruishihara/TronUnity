using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TronScript : MonoBehaviour
{
    public int TronID;
    public Vector3 Position;
    public Vector3 Coulomb;
    public Vector3 Rot;
    private Rigidbody rb;
    //float v = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(string.Format("Tron start id={0}", TronID));
        if (TronID < 10)
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
        } else
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        // new Color(TronID & 1, (TronID & 2) >> 1, (TronID & 4) >> 2, 1);

        //rb = GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(Random.Range(-1 * v, v), Random.Range(-1 * v, v), Random.Range(-1 * v, v));
    }

    // Update is called once per frame
    void Update()
    {
        Position += Coulomb;
        this.transform.position = GetDisplayPosition();
    }

    public Vector3 GetDisplayPosition()
    {
        GameObject sph = GameObject.Find("Sphere");
        Vector3 pole_x = sph.GetComponent<SphereScript>().PoleX;
        Vector3 pole_y = sph.GetComponent<SphereScript>().PoleY;
        Vector3 pole_z = sph.GetComponent<SphereScript>().PoleZ;

        TupleSph t = new TupleSph(Position);
        t.Unify();
        Vector3 p = t.GetVector3();
        Vector3 p1;
        p1.x = Vector3.Dot(p, pole_x);
        p1.y = Vector3.Dot(p, pole_y);
        p1.z = Vector3.Dot(p, pole_z);
        return p1;
    }

    
}
