using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocky : MonoBehaviour
{
    public Block myType;
	void Update ()
    {
        Vector3 pos = transform.position;
        pos.z -= MakeLevel.speed * Time.deltaTime;
        transform.position = pos;

        /*Handles collisions*/
        if (pos.z < MakeLevel.awakeZ && !DoCollisions.hitMe.Contains(gameObject))
            DoCollisions.hitMe.Add(gameObject);

        if (pos.z < MakeLevel.awakeZ - MakeLevel.blockSize.z * 1.5f)
            if (DoCollisions.hitMe.Contains(gameObject))
                DoCollisions.hitMe.Remove(gameObject);

        /*Destroys the objerct when it's off screen*/
        if (pos.z < MakeLevel.killZ)
            Destroy(gameObject);
    }
}
