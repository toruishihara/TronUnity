using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScript : MonoBehaviour
{
    public float alpha = .1f;
    public GameObject TronPrefab;
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
            Vector3 pos = transform.position + new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(-2.5f, 2.5f), Random.Range(-2.5f, 2.5f));
            float angleDegrees = -angle * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, angleDegrees, 0);
            GameObject obj = Instantiate(TronPrefab, pos, rot);
            obj.GetComponent<TronScript>().TronID = cnt;
            lastCreation = t;
            cnt = cnt + 1;
        }

    }
}
