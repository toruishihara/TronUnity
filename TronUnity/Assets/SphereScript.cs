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
    private int poleMoveSteps;
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
        if (poleMoveSteps > 0 && poleMoveSteps <= 20)
        {
            MovePoleTo(PoleMoveList[poleMoveSteps-1]);
            poleMoveSteps++;
            if (poleMoveSteps == 21)
            {
                ResetToPole();
            }
        }
        
        if (t < 30.0f)
        {
            Utils.UpdateCoulomb(TronList, CoulombK);
        }

        if (t > 30.0f && step1 == false)
        {
            step1 = true;

            Utils.drawTronLine(TronList);
        }
        if (t > 32.0f && step2 == false)
        {
            step2 = true;
            Vector3 newPos = Utils.FindFreeSpacePoint(TronList);
            Debug.DrawLine(Vector3.zero, newPos, Color.black, 10f);
            SetPoleMove(newPos);
        }
        if (t > 40.0f && step3 == false)
        {
            step3 = true;
            Utils.drawTronLine(TronList);
        }
    }

    void SetPoleMove(Vector3 p)
    {
        PoleMoveList = new List<Vector3>();
        TupleSph sph = new TupleSph(p);
        TupleSph sph1 = new TupleSph(sph.r, sph.th, 0);
        for (int i = 1; i <= 20; ++i)
        {
            sph1.ph = i * sph.ph / 20f;
            Vector3 p1 = sph1.GetVector3();
            PoleMoveList.Add(p1);
            Debug.DrawLine(Vector3.zero, p, Color.gray, 100f);
        }

        Debug.DrawLine(Vector3.zero, p, Color.black, 100f);
        poleMoveSteps = 1;
    }

    void Create8(bool isRandom)
    {
        for (int i = 0; i < 8; ++i)
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
            obj.GetComponent<TronScript>().TronID = i;
            obj.GetComponent<TronScript>().Position = tuple.GetVector3();
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
