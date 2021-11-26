using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TronScript : MonoBehaviour
{
    public int TronID;
    public Vector3 Coulomb;
    private Rigidbody rb;
    //float v = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(string.Format("Tron start id={0}", TronID));

        GetComponent<MeshRenderer>().material.color = new Color(TronID & 1, (TronID & 2) >> 1, (TronID & 4) >> 2, 1);

        //rb = GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(Random.Range(-1 * v, v), Random.Range(-1 * v, v), Random.Range(-1 * v, v));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;
        pos += Coulomb;
        TupleSph t = new TupleSph(pos);
        t.Unify();
        this.transform.position = t.GetVector3();
    }

    
}
