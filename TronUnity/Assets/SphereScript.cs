using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScript : MonoBehaviour
{
    public float alpha = .1f;
    public GameObject TronPrefab;
    public List<GameObject> TronList = new List<GameObject>();
    public const float CoulombK = 0.001f;
    private int cnt;
    private double lastCreation = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Sph start");
    }

    // Update is called once per frame
    void Update()
    {
        double t = Time.realtimeSinceStartup;
        if (t - lastCreation > 2)
        {
            Debug.Log(string.Format("Sph update t={0}", t));

            float angle = cnt * Mathf.PI * 2 / 10;
            float x = Mathf.Cos(angle) * 6;
            float z = Mathf.Sin(angle) * 6;
            Vector3 pos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            TupleSph tuple = new TupleSph(pos);
            tuple.Unify();

            float angleDegrees = -angle * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, angleDegrees, 0);
            GameObject obj = Instantiate(TronPrefab, tuple.GetVector3(), rot);
            obj.GetComponent<TronScript>().TronID = cnt;
            TronList.Add(obj);
            lastCreation = t;
            cnt = cnt + 1;
        }
        UpdateCoulomb();
    }

    void UpdateCoulomb()
    {
        foreach (GameObject obj0 in TronList)
        {
            TronScript tron = obj0.GetComponent<TronScript>();
            int i = tron.TronID;
            Vector3 newCoulonb = new Vector3();
            foreach (GameObject obj1 in TronList)
            {
                int j = obj1.GetComponent<TronScript>().TronID;
                if (i == j)
                {
                    continue;
                }
                float dis2 = CalcDistance2(obj0.transform.position, obj1.transform.position);
                if (dis2 < TupleSph.diff)
                {
                    continue;
                }
                Vector3 d = obj0.transform.position - obj1.transform.position;
                d /= dis2;
                d *= CoulombK;
                newCoulonb += d;
            }
            tron.Coulomb = newCoulonb;
        }
    }

    private float CalcDistance2(Vector3 a, Vector3 b)
    {
        return (a.x - b.x)*(a.x - b.x) + (a.y - b.y)*(a.y - b.y) + (a.z - b.z)*(a.z - b.z);
    }
}
