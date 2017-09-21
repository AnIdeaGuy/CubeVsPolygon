using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the cam follow the playah.
/// </summary>
public class PlayahCam : MonoBehaviour
{
    public GameObject playah;
    private Vector3 startPoint;
    private Quaternion startRot;
    private float progress = 0;
	void Start ()
    {
        startPoint = transform.position;
        startRot = transform.rotation;
	}
	
	void Update ()
    {
        float angle = (playah.transform.rotation.eulerAngles.z + 45) * Mathf.Deg2Rad;
        Vector3 pos = startPoint;
        Vector3 target = new Vector3(Mathf.Cos(angle) * MakeLevel.superRadius, Mathf.Sin(angle) * MakeLevel.superRadius, playah.transform.position.z + 5);
        Vector3 rotTarget = (target - transform.position).normalized;
        pos.x = Mathf.Cos(angle) * startPoint.y;
        pos.y = Mathf.Sin(angle) * startPoint.y;
        
        Vector3 rot = Quaternion.LookRotation(rotTarget).eulerAngles;
        transform.position = pos;
        rot.z += playah.transform.eulerAngles.z + 90 - rot.z;
        progress += Time.deltaTime * 4;
        if (progress > 1)
            progress = 1;
        transform.rotation = Quaternion.Slerp(startRot, Quaternion.Euler(rot), Mathf.Sin(progress * (Mathf.PI / 2)));
    }

    public void StartRotation()
    {
        gameObject.GetComponent<AudioSource>().Play();
        progress = 0;
        startRot = transform.rotation;
    }
}
