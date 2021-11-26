using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScript : MonoBehaviour
{
    public float alpha = .1f;
    public GameObject TronPrefab;
    public List<GameObject> TronList = new List<GameObject>();
    public const float CoulombK = 0.001f;
    public Vector3 PoleX = new Vector3(1f, 0, 0);
    public Vector3 PoleY = new Vector3(0, 1f, 0);
    public Vector3 PoleZ = new Vector3(0, 0, 1f);
    private int cnt;
    private double lastCreation = 0;
    private bool step1 = false;
    private bool step2 = false;
    private bool step3 = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Sph start");
        Debug.DrawLine(Vector3.zero, PoleX, Color.red, 10f);
        Debug.DrawLine(Vector3.zero, PoleY, Color.green, 10f);
        Debug.DrawLine(Vector3.zero, PoleZ, Color.blue, 10f);

        Create8(true);
    }

    // Update is called once per frame
    void Update()
    {
        double t = Time.realtimeSinceStartup;
        if (t - lastCreation > 2 && cnt < 0)
        {
            Debug.Log(string.Format("Sph update t={0}", t));

            Vector3 pos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            TupleSph tuple = new TupleSph(pos);
            tuple.Unify();

            //float angleDegrees = -angle * Mathf.Rad2Deg;
            //Quaternion rot = Quaternion.Euler(0, angleDegrees, 0);
            GameObject obj = Instantiate(TronPrefab, tuple.GetVector3(), Quaternion.identity);
            obj.GetComponent<TronScript>().TronID = cnt;
            TronList.Add(obj);
            lastCreation = t;
            cnt = cnt + 1;
        }
        //RotateSphere();
        if (t < 30.0f)
        {
            UpdateCoulomb();
        }

        if (t > 30.0f && step1 == false)
        {
            step1 = true;

            drawTronLine();
        }
        if (t > 35.0f && step2 == false)
        {
            step2 = true;
            Vector3 pos2 = new Vector3(0.1f, 1, 0.1f);
            TupleSph tuple2 = new TupleSph(pos2);
            tuple2.Unify();
            Vector3 pos3 = tuple2.GetVector3();
            MovePoleTo(pos3);
        }
        if (t > 40.0f && step3 == false)
        {
            step3 = true;
            drawTronLine();
        }
    }

    void Create8(bool isRandom)
    {
        for (int i = 0; i < 8; ++i)
        {
            Vector3 pos;
            if (isRandom) {
                pos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
            else {
                pos = new Vector3(-1 + 2 * (i & 1), -1 + 2 * ((i & 2) >> 1), -1 + 2 * ((i & 4) >> 2));
            } 
            TupleSph tuple = new TupleSph(pos);
            tuple.Unify();

            GameObject obj = Instantiate(TronPrefab, Vector3.zero, Quaternion.identity);
            obj.GetComponent<TronScript>().TronID = i;
            obj.GetComponent<TronScript>().Position = tuple.GetVector3();
            TronList.Add(obj);
        }
    }

    void drawTronLine()
    {
        foreach (GameObject obj0 in TronList)
        {
            TronScript tron0 = obj0.GetComponent<TronScript>();
            int i = tron0.TronID;
            foreach (GameObject obj1 in TronList)
            {
                TronScript tron1 = obj1.GetComponent<TronScript>();
                int j = obj1.GetComponent<TronScript>().TronID;
                if (j <= i)
                {
                    continue;
                }
                Debug.DrawLine(tron0.GetDisplayPosition(), tron1.GetDisplayPosition(), Color.blue, 10f);
            }
        }
    }

    void UpdateCoulomb()
    {
        foreach (GameObject obj0 in TronList)
        {
            TronScript tron0 = obj0.GetComponent<TronScript>();
            int i = tron0.TronID;
            Vector3 newCoulonb = new Vector3();
            foreach (GameObject obj1 in TronList)
            {
                TronScript tron1 = obj1.GetComponent<TronScript>();
                int j = tron1.TronID;
                if (i == j)
                {
                    continue;
                }
                float dis2 = CalcDistance2(tron0.Position, tron1.Position);
                if (dis2 < TupleSph.diff)
                {
                    continue;
                }
                Vector3 d = tron0.Position - tron1.Position;
                d /= dis2;
                d *= CoulombK;
                newCoulonb += d;
            }
            tron0.Coulomb = newCoulonb;
        }
    }

    private float CalcDistance2(Vector3 a, Vector3 b)
    {
        return (a.x - b.x)*(a.x - b.x) + (a.y - b.y)*(a.y - b.y) + (a.z - b.z)*(a.z - b.z);
    }

    private void MovePoleTo(Vector3 p)
    {
        Debug.DrawLine(Vector3.zero, p, Color.black, 100f);
        Vector3 newPoleY = p;
        Vector3 newPoleZ;
        Vector3 newPoleX;
        float dotX = Vector3.Dot(p, PoleX);
        float dotZ = Vector3.Dot(p, PoleZ);
        if (Mathf.Abs(dotX) < Mathf.Abs(dotZ))
        {
            newPoleZ = Vector3.Cross(p, PoleX).normalized;
        }
        else
        {
            newPoleZ = Vector3.Cross(p, PoleZ).normalized;
        }
        newPoleX = Vector3.Cross(newPoleY, newPoleZ).normalized;

        PoleX = newPoleX;
        PoleY = newPoleY;
        PoleZ = newPoleZ;

        Debug.DrawLine(Vector3.zero, newPoleX, Color.red, 10f);
        Debug.DrawLine(Vector3.zero, newPoleY, Color.green, 10f);
        Debug.DrawLine(Vector3.zero, newPoleZ, Color.blue, 10f);
    }
}
