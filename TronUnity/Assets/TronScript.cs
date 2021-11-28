using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TronScript : MonoBehaviour
{
    public int TronID = 0;
    public Vector3 Position = Vector3.zero;
    public Vector3 Coulomb = Vector3.zero;
    public Vector3 LaunchForce = Vector3.zero;
    public bool isInside = false;

    private Rigidbody rb;
    //float v = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(string.Format("Tron start id={0}", TronID));
        //if (TronID < 10)
        //{
        //    GetComponent<MeshRenderer>().material.color = Color.blue;
        //} else
        //{
        //    GetComponent<MeshRenderer>().material.color = Color.red;
        //}
        GetComponent<MeshRenderer>().material.color = new Color(TronID & 1, (TronID & 2) >> 1, (TronID & 4) >> 2, 1);

        //rb = GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(Random.Range(-1 * v, v), Random.Range(-1 * v, v), Random.Range(-1 * v, v));
    }

    // Update is called once per frame
    void Update()
    {
        if (isInside)
        {
            //Position += Coulomb;
            Position.Normalize();
        }
        else
        {
            Position += LaunchForce;
            if (Vector3.Distance(Vector3.zero, Position) <= 1.0f)
            {
                isInside = true;
                LaunchForce = Vector3.zero;
            }
        }

        this.transform.position = GetDisplayPosition();
    }

    public Vector3 GetDisplayPosition()
    {
        GameObject sph = GameObject.Find("Sphere");
        Vector3 pole_x = sph.GetComponent<SphereScript>().PoleX;
        Vector3 pole_y = sph.GetComponent<SphereScript>().PoleY;
        Vector3 pole_z = sph.GetComponent<SphereScript>().PoleZ;

        //TupleSph t = new TupleSph(Position);
        //t.Unify();
        //Vector3 p = t.GetVector3();
        Vector3 p1;
        p1.x = Vector3.Dot(Position, pole_x);
        p1.y = Vector3.Dot(Position, pole_y);
        p1.z = Vector3.Dot(Position, pole_z);
        return p1;
    }

    
}
