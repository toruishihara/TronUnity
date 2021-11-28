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
    public List<Vector3> PoleMoveList;// = new List<Vector3>();
    public float FPS = 130f;
    private int poleMoveSteps;
    private int cnt;
    private int lastCnt;
    private int lastSec = 0;
    private int tronCnt = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Sph start");
        //Debug.DrawLine(Vector3.zero, PoleX, Color.red, 10f);
        Debug.DrawLine(Vector3.zero, new Vector3(2f, 0, 0), Color.red, 10f);
        Debug.DrawLine(Vector3.zero, PoleY, Color.green, 10f);
        Debug.DrawLine(Vector3.zero, PoleZ, Color.blue, 10f);

        //Create4(true);
    }

    // Update is called once per frame
    void Update()
    {
        cnt++;
        float t = Time.realtimeSinceStartup;
        int sec = (int)t;
        if (poleMoveSteps > 0 && poleMoveSteps <= 20)
        {
            MovePoleTo(PoleMoveList[poleMoveSteps - 1]);
            poleMoveSteps++;
            if (poleMoveSteps == 21)
            {
                ResetToPole();
            }
        }
        if (sec > lastSec)
        {
            // Do event every sec
            FPS = (cnt - lastCnt);
            if (FPS < 1)
            {
                FPS = 130f;
            }
            if (sec % 10 == 0)
            {
                Debug.Log("Every 10 Sec cnt=" + cnt + " new FPS=" + FPS);
                if (tronCnt < 32)
                {
                    Create1(true);
                }
            }
            if (sec % 10 == 9)
            {
                Utils.drawTronLine(TronList);
            }
        }
        Utils.UpdateCoulomb(TronList, CoulombK);

        lastSec = sec;
        lastCnt = cnt;
    }

    void SetPoleXMove(Vector3 p)
    {
        PoleMoveList = new List<Vector3>();
        TupleSph sph = new TupleSph(p);
        TupleSph sph1 = new TupleSph(sph.r, sph.th, 0);
        for (int i = 1; i <= 20; ++i)
        {
            sph1.ph = i * sph.ph / 20f;
            Vector3 p1 = sph1.GetVector3();
            PoleMoveList.Add(p1);
            Debug.DrawLine(Vector3.zero, p1, Color.gray, 1f);
        }

        Debug.DrawLine(Vector3.zero, p, Color.black, 10f);
        poleMoveSteps = 1;
    }

    void Create1(bool isRandom)
    {
        Vector3 pos = new Vector3(-3f, 0, 0);

        GameObject obj = Instantiate(TronPrefab, Vector3.zero, Quaternion.identity);
        TronScript tron = obj.GetComponent<TronScript>();
        tron.TronID = tronCnt++;
        tron.Position = pos;
        tron.isInside = false;
        float v = 0.024f / FPS;
        float dv = 0.05f;
        if (isRandom)
        {
            tron.LaunchForce = new Vector3(v, v*Random.Range(-1*dv, dv), v*Random.Range(-1*dv, dv));
        }
        else
        {
            tron.LaunchForce = new Vector3(v, 0, 0);
        }
        TronList.Add(obj);
    }

    void Create4(bool isRandom)
    {
        for (int i = 0; i < 4; ++i)
        {
            Vector3 pos;
            if (isRandom)
            {
                pos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
            else
            {
                pos = new Vector3(-1 + 2 * (i & 1), -1 + 2 * ((i & 2) >> 1), -1 + 2 * ((i & 4) >> 2));
            }
            TupleSph tuple = new TupleSph(pos);
            tuple.Unify();

            GameObject obj = Instantiate(TronPrefab, Vector3.zero, Quaternion.identity);
            TronScript tron = obj.GetComponent<TronScript>();
            tron.TronID = tronCnt++;
            tron.Position = tuple.GetVector3();
            tron.isInside = true;
            TronList.Add(obj);
        }
    }

    private void MovePoleTo(Vector3 p)
    {
        Vector3 newPoleY = p;
        Vector3 newPoleZ;
        Vector3 newPoleX;
        newPoleZ = Vector3.Cross(PoleX, newPoleY).normalized;
        newPoleX = Vector3.Cross(newPoleY, newPoleZ).normalized;

        PoleX = newPoleX;
        PoleY = newPoleY;
        PoleZ = newPoleZ;

        Debug.DrawLine(Vector3.zero, PoleX, Color.red, 1f);
        Debug.DrawLine(Vector3.zero, PoleY, Color.green, 1f);
        Debug.DrawLine(Vector3.zero, PoleZ, Color.blue, 1f);
    }

    private void ResetToPole()
    {
        foreach (GameObject obj in TronList)
        {
            TronScript tron = obj.GetComponent<TronScript>();
            obj.transform.position = tron.GetDisplayPosition();
        }
        PoleX = Vector3.right;
        PoleY = Vector3.up;
        PoleZ = Vector3.forward;
    }
}
