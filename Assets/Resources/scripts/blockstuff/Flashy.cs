using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashy : MonoBehaviour
{
    private float t = 0;
    private const float T_MAX = .2f;
	void Start ()
    {
		
	}
	
	void Update ()
    {
        if (t < T_MAX)
            t += Time.deltaTime;
        else
        {
            t = 0;
            gameObject.GetComponent<Renderer>().material.color = new Color(RandomFloat(), RandomFloat(), RandomFloat());
        }
	}

    private float RandomFloat()
    {
        return Utils.RoundSpec(Random.Range(0.0f, 1.0f), .5f);
    }
}
