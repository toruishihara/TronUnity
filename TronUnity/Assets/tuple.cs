using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TupleSph 
{
    public float r;
    public float th;
    public float ph;

    public const float diff = 0.0000001f;

    public TupleSph(float x, float y, float z)
    {
        r = Mathf.Sqrt(x * x + y * y + z * z);
        th = Mathf.Atan2(y, x);
        if (r < diff)
        {
            ph = 0f;
        }
        else
        {
            ph = Mathf.Acos(z / r);
        }
    }

    public TupleSph(Vector3 v)
    {
        r = Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        th = Mathf.Atan2(v.y, v.x);
        if (r < diff)
        {
            ph = 0.0f;
        }
        else
        {
            ph = Mathf.Acos(v.z / r);
        }
        Debug.Log(string.Format("TupleSph xyz={0} {1} {2} sph={3} {4} {5}", v.x, v.y, v.z, r, th, ph));
    }

    public void SetXYZ(float x, float y, float z)
    {
        r = Mathf.Sqrt(x * x + y * y + z * z);
        th = Mathf.Atan2(y, x);
        if (r < diff)
        {
            ph = 0.0f;
        }
        else
        {
            ph = Mathf.Acos(z / r);
        }
    }

    public void SetVector3(Vector3 v)
    {
        SetXYZ(v.x, v.y, v.z);
    }

    public Vector3 GetVector3()
    {
        float x = r * Mathf.Cos(th) * Mathf.Sin(ph);
        float y = r * Mathf.Sin(th) * Mathf.Sin(ph);
        float z = r * Mathf.Cos(ph);
        Debug.Log(string.Format("GetVector3 xyz={0} {1} {2} sph={3} {4} {5}", x, y, z, r, th, ph));
        return new Vector3(x, y, z);
    }

    public void Unify()
    {
        r = 1.0f;
    }
}

/*
function tuple3d_xy2sp()
{
    var r = Math.sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
    var th = Math.atan2(this.y, this.x);
    var ph;
    if (r < 0.0000001)
    {
        ph = 0;
    }
    else
    {
        ph = Math.acos(this.z / r);
    }
    this.x = r;
    this.y = th;
    this.z = ph;
}

function tuple3d_sp2xy()
{
    var r = this.x;
    var th = this.y;
    var ph = this.z;
    this.x = r * Math.cos(th) * Math.sin(ph);
    this.y = r * Math.sin(th) * Math.sin(ph);
    this.z = r * Math.cos(ph);
}
*/
