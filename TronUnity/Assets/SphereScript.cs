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

    private Vector3 RotateTarget = Vector3.zero;
    private int RotateTotalSteps = 1000;
    private int RotateStep = 0;

    private bool Nth_method = false;
    private bool Slerp_method = true;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Sph start");
        Random.InitState(0);
        
        Debug.DrawLine(Vector3.zero, PoleX, Color.red, 1f);
        Debug.DrawLine(Vector3.zero, PoleY, Color.green, 1f);
        Debug.DrawLine(Vector3.zero, PoleZ, Color.blue, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        cnt++;
        float t = Time.realtimeSinceStartup;
        int sec = (int)t;

        if (Slerp_method == true && RotateStep > 0)
        {
            float fracComplete = (float)RotateStep / (float)RotateTotalSteps;
            Vector3 newX = Vector3.Slerp(Vector3.right, RotateTarget, fracComplete);
            PoleX = newX;
            PoleZ = Vector3.Cross(PoleX, PoleY);
            PoleY = Vector3.Cross(PoleZ, PoleX);
            //Debug.DrawLine(Vector3.zero, newX, Color.gray, 1f);
            ++RotateStep;
            if (RotateStep > RotateTotalSteps)
            {
                ResetToPole();
                RotateStep = 0;
                RotateTarget = Vector3.right;
            }
        }
        if (Nth_method == true && RotateStep > 0)
        {
            float p1 = (float)RotateStep / (float)RotateTotalSteps;
            float p0 = 1.0f - p1;
            Vector3 t1 = RotateTarget;
            Vector3 x0 = Vector3.right;

            Vector3 newX = new Vector3(p0*x0.x + p1*t1.x, p0* x0.y + p1*t1.y, p0* x0.z + p1*t1.z);
            newX.Normalize();
            PoleX = newX;
            PoleZ = Vector3.Cross(PoleX, PoleY);
            PoleY = Vector3.Cross(PoleZ, PoleX);
            if (RotateStep == RotateTotalSteps)
            {
                ResetToPole();
                RotateStep = 0;
                RotateTarget = Vector3.zero;
            }
            else
            {
                RotateStep++;
            }
        }
        if (sec > lastSec)
        {
            Debug.Log("sec=" + sec + " RotateStep=" + RotateStep);
            lastCnt = cnt;
            // Do event every sec
            FPS = (cnt - lastCnt);
            if (FPS <= 1)
            {
                FPS = 130f;
            }
            if (sec % 5 == 0)
            {
                SetPoleXMove(Utils.FindFreeSpacePoint(TronList));
            }

            if (sec % 10 == 0)
            {
                Debug.Log("Every 10 Sec cnt=" + cnt + " new FPS=" + FPS);
                if (tronCnt < 32)
                {
                    Create1(true);
                }
            }
            if (sec % 30 == 29)
            {
                Utils.drawTronLine(TronList);
            }
        }
        Utils.UpdateCoulomb(TronList, CoulombK);

        lastSec = sec;
        
    }

    void SetPoleXMove(Vector3 p)
    {
        RotateTarget = p;
        float dot = Vector3.Dot(PoleX, RotateTarget);
        float rad = Mathf.Acos(dot);
        float angle = 180f * rad / Mathf.PI;
        RotateTarget.Normalize();
        RotateStep = 1;
        RotateTotalSteps = (int)(angle * FPS / 15f); // 15 degree / sec
        Debug.DrawLine(Vector3.zero, RotateTarget, Color.black, 2f);
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

    void CreateN(int n, bool isRandom)
    {
        for (int i = 0; i < n; ++i)
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

    private void MovePoleXTo(Vector3 p)
    {
        Vector3 newPoleX = p;
        Vector3 newPoleY;
        Vector3 newPoleZ;
        newPoleZ = Vector3.Cross(newPoleX, PoleY).normalized;
        newPoleY = Vector3.Cross(newPoleZ, newPoleX).normalized;

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
            float x = Vector3.Dot(PoleX, tron.Position);
            float y = Vector3.Dot(PoleY, tron.Position);
            float z = Vector3.Dot(PoleZ, tron.Position);
            tron.Position = new Vector3(x,y,z);
        }
        PoleX = Vector3.right;
        PoleY = Vector3.up;
        PoleZ = Vector3.forward;
    }
}
