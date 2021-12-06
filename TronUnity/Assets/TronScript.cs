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
        GetComponent<MeshRenderer>().material.color = Color.red;
        //GetComponent<MeshRenderer>().material.color = new Color(TronID & 1, (TronID & 2) >> 1, (TronID & 4) >> 2, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInside)
        {
            Position += Coulomb;
            Position.Normalize();
            Vector3 dp = GetDisplayPosition();
            this.transform.position = dp;
        }
        else
        {
            Vector3 old0 = new Vector3(Position.x, Position.y, Position.z);
            Position = new Vector3(0.99f*Position.x , 0.99f * Position.y, 0.99f * Position.z);
            if (Vector3.Distance(Vector3.zero, Position) <= 1.0f)
            {
                isInside = true;
                GameObject sph = GameObject.Find("Sphere");
                Vector3 pole_x = sph.GetComponent<SphereScript>().PoleX;
                Vector3 d = Vector3.right - pole_x;
                Position += d;
                Position.Normalize();
            }
            Vector3 old = this.transform.position;
            if (Mathf.Abs(old.x - Position.x) > 0.2f && Mathf.Abs(old.x - Position.x) < 2f && isInside == false) {
                Debug.Log("Huge move");
            }
            this.transform.position = Position;
        }
        /*
        SphereScript sph2 = GameObject.Find("Sphere").GetComponent<SphereScript>();
        if (sph2.cnt > 200 && TronID == 0 && Mathf.Abs(this.transform.position.x - dp.x) > 0.1f)
        {
            //GameObject sph = GameObject.Find("Sphere");
            Debug.Log("TronScript Huge update old=" + this.transform.position + " new=" + dp);
            Debug.Log("PoleX=" + sph2.PoleX);
            Debug.Log("PoleY=" + sph2.PoleY);
            Debug.Log("PoleZ=" + sph2.PoleZ);
            Debug.Log("break");
        }
        */
    }

    public Vector3 GetDisplayPosition()
    {
        if (isInside)
        {
            GameObject sph = GameObject.Find("Sphere");
            Vector3 pole_x = sph.GetComponent<SphereScript>().PoleX;
            Vector3 pole_y = sph.GetComponent<SphereScript>().PoleY;
            Vector3 pole_z = sph.GetComponent<SphereScript>().PoleZ;
           
            Vector3 p1;
            p1.x = Vector3.Dot(Position, pole_x);
            p1.y = Vector3.Dot(Position, pole_y);
            p1.z = Vector3.Dot(Position, pole_z);
            return p1;
        }
        else
        {
            return Position;
        }
    }
}
