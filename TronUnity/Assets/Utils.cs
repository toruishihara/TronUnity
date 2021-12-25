using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static void drawTronLine(List<GameObject> tronList)
    {
        float min_d = 2.0f;
        foreach (GameObject obj0 in tronList)
        {
            TronScript tron0 = obj0.GetComponent<TronScript>();
            int i = tron0.TronID;
            if (tron0.isInside == false) { continue; }
            foreach (GameObject obj1 in tronList)
            {
                TronScript tron1 = obj1.GetComponent<TronScript>();
                int j = obj1.GetComponent<TronScript>().TronID;
                if (j <= i || tron1.isInside == false) { continue; }
                float d = Vector3.Distance(tron0.Position, tron1.Position);
                if (d < min_d)
                {
                    min_d = d;
                }
                //Debug.DrawLine(tron0.GetDisplayPosition(), tron1.GetDisplayPosition(), Color.blue, 3f);
            }
        }
        foreach (GameObject obj0 in tronList)
        {
            TronScript tron0 = obj0.GetComponent<TronScript>();
            int i = tron0.TronID;
            if (tron0.isInside == false) { continue; }
            foreach (GameObject obj1 in tronList)
            {
                TronScript tron1 = obj1.GetComponent<TronScript>();
                int j = obj1.GetComponent<TronScript>().TronID;
                if (j <= i || tron1.isInside == false) { continue; }
                float d = Vector3.Distance(tron0.Position, tron1.Position);
                if (d < 1.1 * min_d)
                {
                    Debug.DrawLine(tron0.GetDisplayPosition(), tron1.GetDisplayPosition(), Color.blue, 3f);
                }
                else if (d < 1.5 * min_d)
                {
                    Debug.DrawLine(tron0.GetDisplayPosition(), tron1.GetDisplayPosition(), Color.green, 2f);
                }
            }
        }

    }

    public static void UpdateCoulomb(List<GameObject> tronList, float k)
    {
        foreach (GameObject obj0 in tronList)
        {
            TronScript tron0 = obj0.GetComponent<TronScript>();
            if (tron0.isInside == false) { continue; }
            int i = tron0.TronID;
            Vector3 newCoulonb = new Vector3(0, 0, 0);
            foreach (GameObject obj1 in tronList)
            {
                TronScript tron1 = obj1.GetComponent<TronScript>();
                int j = tron1.TronID;
                if (i == j) { continue; }
                if (tron1.isInside == false) { continue; }
                float dis2 = CalcDistance2(tron0.Position, tron1.Position);
                if (dis2 < TupleSph.diff)
                {
                    // Too close, add random force
                    float r0 = -0.001f;
                    float r1 = 0.001f;
                    Vector3 rand = new Vector3(Random.Range(r0, r1), Random.Range(r0, r1), Random.Range(r0, r1));
                    newCoulonb += rand;
                }
                Vector3 d = tron0.Position - tron1.Position;
                d /= dis2;
                d *= k;
                newCoulonb += d;
            }
            tron0.Coulomb = newCoulonb;
        }
    }

    private static float CalcDistance2(Vector3 a, Vector3 b)
    {
        return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
    }

    public static Vector3 FindFreeSpacePoint(List<GameObject> tronList)
    {
        float fine = 32.0f; //1024.0;
        float minDot = 1.0f;
        Vector3 freePoint = new Vector3();
        for (float ph = 0; ph < Mathf.PI; ph += Mathf.PI / fine)
        {
            float thInc = Mathf.PI / (fine * Mathf.Sin(ph));
            for (float th = 0; th < 2 * Mathf.PI; th += thInc)
            {
                TupleSph sph = new TupleSph(1f, th, ph);
                int closestIndex;
                float maxDot = -1.0f;
                Vector3 p = sph.GetVector3();
                //Debug.DrawLine(Vector3.zero, p, Color.gray, 1f);
                foreach (GameObject obj in tronList)
                {
                    TronScript tron = obj.GetComponent<TronScript>();
                    float dot = Vector3.Dot(tron.Position, p);//ti.dot(p);
                    if (dot > maxDot)
                    {
                        maxDot = dot;
                        closestIndex = tron.TronID;
                    }
                }
                if (minDot > maxDot)
                {
                    minDot = maxDot;
                    freePoint = p;
                }
            }
        }
        return freePoint;
    }

    private static Vector3 getVertialCrossPointOfTwoTron(Vector3 p0, Vector3 p1)
    {
        Vector3 p01 = p0 + p1;//new Vector3(p0.x + p1.x, p0.y + p0.y, p0.z + p1.z);
        if (p01.sqrMagnitude < 0.00001)
        {
            return Vector3.zero;
        }
        p01.Normalize();
        p01 *= (1.0f/ Vector3.Dot(p01,p0));
        return p01;
    }

    // find minimum distance point of two lines, 
    // p0 : one point of line0
    // v0 : vector of line0
    // p1 : one point of line1
    // v1 : vector of line1
    // return S: p0 + S*v0
    private static float minimumDistancePointOfTwoLines(Vector3 p0, Vector3 v0, Vector3 p1, Vector3 v1)
    {
        Vector3 d01 = p1 - p0;

        // http://d.hatena.ne.jp/obelisk2/20101228/1293521247
        float s = Vector3.Dot(d01, v0) - Vector3.Dot(d01, v1) * Vector3.Dot(v0, v1);
        s /= (1.0f - Vector3.Dot(v0, v1) * Vector3.Dot(v0, v1));
        return s;
    }

    public static List<Vector3> GetFacePoints(List<GameObject> tronList)
    {
        List<Vector3> facepoints = new List<Vector3>();
        //List<int> dupCheck = new List<int>();
        // sort Trons
        //sortedTrons = Trons.concat();
        //sortedTrons.sort(TronSort);

        // Find shortest pair
        float shortest = 1000f;
        foreach (GameObject obj0 in tronList)
        {
            TronScript tron0 = obj0.GetComponent<TronScript>();
            foreach (GameObject obj1 in tronList)
            {
                TronScript tron1 = obj1.GetComponent<TronScript>();
                if (tron1.TronID <= tron0.TronID) { continue; }
                float dis = Vector3.Distance(tron0.Position, tron1.Position);
                if (dis < shortest)
                {
                    shortest = dis;
                }
            }
        }
        //int cnt = 0;
        foreach (GameObject obj0 in tronList)
        {
            TronScript tron0 = obj0.GetComponent<TronScript>();
            foreach (GameObject obj1 in tronList)
            {
                TronScript tron1 = obj1.GetComponent<TronScript>();
                if (tron1.TronID <= tron0.TronID) { continue; }
                float dis01 = Vector3.Distance(tron0.Position, tron1.Position);
                if (dis01 < shortest * 1.5 && dis01 > 0.00001)
                {
                    foreach (GameObject obj2 in tronList)
                    {
                        TronScript tron2 = obj2.GetComponent<TronScript>();
                        if (tron2.TronID <= tron1.TronID) { continue; }
                        float dis02 = Vector3.Distance(tron0.Position, tron2.Position);
                        float dis12 = Vector3.Distance(tron1.Position, tron2.Position);
                        if (dis02 < shortest * 1.5 && dis12 < shortest * 1.5)
                        {
                            Vector3 p01 = getVertialCrossPointOfTwoTron(tron0.Position, tron1.Position);
                            Vector3 p12 = getVertialCrossPointOfTwoTron(tron1.Position, tron2.Position);
                            Vector3 p20 = getVertialCrossPointOfTwoTron(tron2.Position, tron0.Position);
                            if (p01 == Vector3.zero || p12 == Vector3.zero || p20 == Vector3.zero) { continue; }
                            Vector3 cr01 = Vector3.Cross(tron0.Position, tron1.Position);
                            cr01.Normalize();
                            Vector3 cr12 = Vector3.Cross(tron1.Position, tron2.Position);
                            cr12.Normalize();
                            if (Vector3.Dot(cr01, cr12) > 0.9999)
                            {
                                continue; //parallel
                            }

                            float s = minimumDistancePointOfTwoLines(p01, cr01, p12, cr12);
                            Vector3 ps = new Vector3(p01.x, p01.y, p01.z);
                            cr01 *= (s);
                            ps += (cr01);
                            // if already has same point, skip it. Happens on square case 
                            Vector3 pn = new Vector3(ps.x, ps.y, ps.z);
                            pn.Normalize();
                            //int idx = (int)Mathf.Floor(pn.x * 63f) + 64;
                            //idx *= 128;
                            //idx += (int)Mathf.Floor(pn.y * 63f) + 64;
                            //idx *= 128;
                            //idx += (int)Mathf.Floor(pn.z * 63f) + 64;
                            //console.log("x=" + ps.x + " y=" + ps.y + " z=" + ps.z + " i=" + idx);
                            //if (dupCheck[idx] == 1)
                            //{
                            //console.log("SKIPP" + "i=" + idx);
                            //continue;
                            //}
                            //dupCheck[idx] = 1;
                            // above is not perfect code for floating xyz values
                            //Debug.DrawLine(Vector3.zero, ps, Color.blue, 1f);
                            facepoints.Add(ps);
                            //drawDotps, 0x000000, 2);
                            //TODO: addTriangleFace(p01, p0, ps);
                            //TODO: addTriangleFace(p0, p20, ps);
                            //TODO: addTriangleFace(p1, p01, ps);
                            //TODO: addTriangleFace(p12, p1, ps);
                            //TODO: addTriangleFace(p2, p12, ps);
                            //TODO: addTriangleFace(p20, p2, ps);
                            //return;
                        }
                    }
                }
            }
        }
        //TODO FacePoints.sort(facePointSort);
        return facepoints;
    }
}
