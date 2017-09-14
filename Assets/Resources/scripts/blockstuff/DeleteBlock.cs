using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteBlock : MonoBehaviour
{
    private int life = 0;
    private const int DIE_AT = 4;
	// Use this for initialization
	void OnCollisionEnter (Collision col)
    {
		if (col.gameObject.GetComponent<FakeBlock>() != null)
            Destroy(col.gameObject);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (life >= DIE_AT)
            Destroy(gameObject);
        life++;
	}
}
