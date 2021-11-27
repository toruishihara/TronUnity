using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static void drawTronLine(List<GameObject> tronList)
    {
        foreach (GameObject obj0 in tronList)
        {
            TronScript tron0 = obj0.GetComponent<TronScript>();
            int i = tron0.TronID;
            if (tron0.isInside == false)
            {
                continue;
            }
            foreach (GameObject obj1 in tronList)
            {
                TronScript tron1 = obj1.GetComponent<TronScript>();
                int j = obj1.GetComponent<TronScript>().TronID;
                if (j <= i)
                {
                    continue;
                }
                Debug.DrawLine(tron0.GetDisplayPosition(), tron1.GetDisplayPosition(), Color.blue, 3f);
            }
        }
    }

    public static void UpdateCoulomb(List<GameObject> tronList, float k)
    {
        foreach (GameObject obj0 in tronList)
        {
            TronScript tron0 = obj0.GetComponent<TronScript>();
            int i = tron0.TronID;
            Vector3 newCoulonb = new Vector3(0,0,0);
            foreach (GameObject obj1 in tronList)
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
}
