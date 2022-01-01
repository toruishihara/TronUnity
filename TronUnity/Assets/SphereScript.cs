using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScript : MonoBehaviour
{
    public float alpha = .1f;
    public int TotalTrons = 0;
    public GameObject TronPrefab;
    public GameObject FacePrefab;
    public GameObject CylinderPrefab;
    public List<GameObject> TronList = new List<GameObject>();
    public List<GameObject> FaceList = new List<GameObject>();
    public List<GameObject> CylinderList = new List<GameObject>();
    public const float CoulombK = 0.004f;
    public Vector3 PoleX = new Vector3(1f, 0, 0);
    public Vector3 PoleY = new Vector3(0, 1f, 0);
    public Vector3 PoleZ = new Vector3(0, 0, 1f);

    public Vector3 LastPoleX;
    public Vector3 LastPoleY;
    public Vector3 LastPoleZ;

    public List<Vector3> PoleMoveList;// = new List<Vector3>();
    public float FPS = 130f;
    private int poleMoveSteps;
    public int cnt;
    private int lastCnt;
    private int lastSec = 0;
    private int tronCnt = 0;

    private Vector3 RotateTarget = Vector3.zero;
    private int RotateTotalSteps = 1000;
    private int RotateStep = 0;
    private bool RotateNearY = false;

    public List<Vector3> FacePoints;

    // Start is called before the first frame update
    void Start()
    {
        float t = Time.realtimeSinceStartup;
        Debug.Log("Sph start t=" + t);
        Random.InitState(0);
        TotalTrons = 72;
    }

    // Update is called once per frame
    void Update()
    {
        cnt++;
        float t = Time.realtimeSinceStartup;
        int sec = (int)t;

        if (RotateStep > 0 && RotateTotalSteps > 0)
        {
            float fracComplete = (float)RotateStep / (float)RotateTotalSteps;
            Vector3 newX = Vector3.Slerp(Vector3.right, RotateTarget, fracComplete);
            if (Mathf.Abs(PoleX.x - newX.x) > 0.1f)
            {
                Debug.Log("Huge change break");
            }
            LastPoleX = PoleX;
            LastPoleY = PoleY;
            LastPoleZ = PoleZ;

            PoleX = newX;

            if (RotateNearY)
            {
                PoleY = Vector3.Cross(PoleZ, PoleX);
                PoleZ = Vector3.Cross(PoleX, PoleY);
            }
            else
            {
                PoleZ = Vector3.Cross(PoleX, PoleY);
                PoleY = Vector3.Cross(PoleZ, PoleX);
            }
            //Debug.DrawLine(Vector3.zero, newX, Color.gray, 1f);
            ++RotateStep;
            if (RotateStep > RotateTotalSteps)
            {
                Vector3 d = RotateTarget - PoleX;
                Vector3 nextTarget = RotateTarget + d;

                ResetToPole();
                FaceWork();
                RotateTarget = nextTarget;
                RotateStep = 1;
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

            if (sec > 10 && sec % 2 == 1)
            {
                //Debug.Log("Every 10 Sec cnt=" + cnt + " new FPS=" + FPS);
                if (tronCnt < TotalTrons)
                {
                    AddTron(false);
                }
            }
            //if (sec % 20 == 15)
            //{
            //    Utils.drawTronLine(TronList);
            //}
            if (sec % 20 == 1 || RotateStep == 0)
            {
                //Utils.drawTronLine(TronList);
                ResetToPole();
                FaceWork();
                Vector3 p2 = new Vector3(0.5f, Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                p2.Normalize();
                SetPoleXMove(p2);
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
        RotateTotalSteps = (int)(angle * FPS / 20.0f); // 2.5 degree / sec
        float dotY = Mathf.Abs(Vector3.Dot(PoleY, RotateTarget));
        float dotZ = Mathf.Abs(Vector3.Dot(PoleZ, RotateTarget));
        Debug.Log("SetPoleXMove=" + p + " dot=" + dot + " angle=" + angle + " dotY=" + dotY + " dotZ=" + dotZ);
        if (dotY > dotZ)
        {
            RotateNearY = true;
        }
        else
        {
            RotateNearY = false;
        }
        Debug.DrawLine(Vector3.zero, RotateTarget, Color.black, 1f);
    }

    void AddTron(bool isRandom)
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
            tron.LaunchForce = new Vector3(v, v * Random.Range(-1 * dv, dv), v * Random.Range(-1 * dv, dv));
        }
        else
        {
            tron.LaunchForce = new Vector3(v, 0, 0);
        }
        TronList.Add(obj);
    }

    void AddTronN(int n, bool isRandom)
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
            if (tron.isInside == false) { continue; }
            float x = Vector3.Dot(PoleX, tron.Position);
            float y = Vector3.Dot(PoleY, tron.Position);
            float z = Vector3.Dot(PoleZ, tron.Position);
            Vector3 p = new Vector3(x, y, z);
            Debug.Log("ResetToPole old=" + tron.Position + " new=" + p);
            tron.Position = p;
        }
        PoleX = Vector3.right;
        PoleY = Vector3.up;
        PoleZ = Vector3.forward;
    }

    private void FaceWork()
    {
        float near0 = 99.9f;
        float near1 = 99.9f;
        for (int i = 0; i < FaceList.Count; ++i)
        {
            Destroy(FaceList[i]);
        }
        FaceList.Clear();
        for (int i = 0; i < CylinderList.Count; ++i)
        {
            Destroy(CylinderList[i]);
        }
        CylinderList.Clear();

        FacePoints = Utils.GetFacePoints(TronList);
        // calc Tron - FacePoint nearest distance
        for (int i = 0; i < FacePoints.Count; ++i)
        {
            Vector3 p0 = FacePoints[i];
            for (int j = 0; j < TronList.Count; ++j)
            {
                Vector3 p1 = TronList[j].GetComponent<TronScript>().Position;
                float dis = Vector3.Distance(p0, p1);
                if (dis < near0)
                {
                    near0 = dis;
                }
            }
        }
        // calc Tron - FacePoint nearest distance
        for (int i = 0; i < FacePoints.Count; ++i)
        {
            Vector3 p0 = FacePoints[i];
            for (int j = i+1; j < FacePoints.Count; ++j)
            {
                Vector3 p1 = FacePoints[j];
                float dis = Vector3.Distance(p0, p1);
                if (dis < near1)
                {
                    near1 = dis;
                }
            }
        }

        for (int i = 0; i < TronList.Count; ++i)
        {
            Vector3 p0 = TronList[i].GetComponent<TronScript>().Position;
            for (int j = 0; j < FacePoints.Count; ++j)
            {
                Vector3 p1 = FacePoints[j];
                float dis01 = Vector3.Distance(p0, p1);
                if (dis01 < 1.7*near0)
                {
                    Debug.DrawLine(Vector3.zero, p0, Color.blue, 2f);
                    Debug.DrawLine(Vector3.zero, p1, Color.green, 1f);
                    for (int k = j + 1; k < FacePoints.Count; ++k)
                    {
                        Vector3 p2 = FacePoints[k];
                        float dis02 = Vector3.Distance(p0, p2);
                        float dis12 = Vector3.Distance(p1, p2);
                        if (dis02 < 1.7*near0 && dis12 < 1.7*near1)
                        {
                            GameObject f = Instantiate(FacePrefab, Vector3.zero, Quaternion.identity);
                            //MeshRenderer mesh = f.GetComponent<MeshRenderer>();
                            //mesh.enabled = false;
                            FaceScript face = f.GetComponent<FaceScript>();
                            face.SetPoints(p0, p1, p2);
                            FaceList.Add(f);
                            Debug.DrawLine(Vector3.zero, p2, Color.green, 1f);

                            addCylinder(p1, p2);
                        }
                    }
                }
            }
        }
    }

    private void FaceWork2()
    {
        GameObject f0 = Instantiate(FacePrefab, Vector3.zero, Quaternion.identity);
        FaceScript face0 = f0.GetComponent<FaceScript>();
        face0.SetPoints(new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1));
    }

    private void addCylinder(Vector3 p1, Vector3 p2)
    {
        GameObject obj = Instantiate(CylinderPrefab, Vector3.zero, Quaternion.identity);
        //MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
        //mesh.enabled = false;
        CylinderScript cyl = obj.GetComponent<CylinderScript>();
        cyl.SetStartEnd(p1, p2);
        CylinderList.Add(obj);
    }
}