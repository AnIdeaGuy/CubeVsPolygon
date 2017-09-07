using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayahCam : MonoBehaviour
{
    public GameObject playah;
    private Vector3 startPoint;
	void Start ()
    {
        startPoint = transform.position;
	}
	
	void Update ()
    {
        float angle = playah.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 pos = startPoint;
        Vector3 target = new Vector3(Mathf.Cos(angle) * MakeLevel.superRadius, Mathf.Sin(angle) * MakeLevel.superRadius, playah.transform.position.z + 5);
        Vector3 rotTarget = (target - transform.position).normalized;
        pos.x = Mathf.Cos(angle) * startPoint.y;
        pos.y = Mathf.Sin(angle) * startPoint.y;
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(rotTarget);
    }
}
