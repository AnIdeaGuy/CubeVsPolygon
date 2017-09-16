using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocky : MonoBehaviour
{
    public Block myType;
	void Update ()
    {
        if (!MakeLevel.paused)
        {
            Vector3 pos = transform.position;
            pos.z -= MakeLevel.speed * Time.deltaTime;
            transform.position = pos;

            /*Handles collisions*/
            if (pos.z < MakeLevel.awakeZ && !DoCollisions.hitMe.Contains(gameObject))
                DoCollisions.hitMe.Add(gameObject);

            /*Destroys the objerct when it's off screen*/
            if (pos.z < MakeLevel.killZ)
            {
                if (DoCollisions.hitMe.Contains(gameObject))
                    DoCollisions.hitMe.Remove(gameObject);
                Destroy(gameObject);
            }
        }

        if (MakeLevel.resetThisFrame)
        {
            if (DoCollisions.hitMe.Contains(gameObject))
                DoCollisions.hitMe.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
