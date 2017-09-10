using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoCollisions : MonoBehaviour
{
    static public List<GameObject> hitMe = new List<GameObject>();

    static public Block WhatHit(Polar posP, Vector2 size)
    {
        foreach (GameObject obj in hitMe)
        {
			Block blok = obj.GetComponent<Blocky> ().myType;
			float rotB = obj.transform.eulerAngles.z;

			Recto rectP = new Recto(posP, size.y);
            Recto rectB = GetRecto(obj);

			if (rectP.DoesItHit(rectB))
				return blok;
        }
        return Block.AIR;
    }

    static private Recto GetRecto(GameObject obj)
    {
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;
        float r = Mathf.Sqrt(x * x + y * y);
        return new Recto(new Polar(obj.transform.rotation.eulerAngles.z, r), obj.transform.localScale.y);
    }

    static public Polar ContactPointDown(Polar posP, Vector2 size)
    {
        foreach (GameObject obj in hitMe)
        {
            float rotB = obj.transform.eulerAngles.z;

            Recto rectP = new Recto(posP, size.y);
            Recto rectB = GetRecto(obj);

            if (rectP.DoesItHit(rectB))
            {
                Polar finalPoint = rectB.position;
				float radius = rectB.hsize.r + rectP.hsize.r; // Gets the distance the player should be from the center of the box
                finalPoint.r -= radius;
                finalPoint.a = rotB * Mathf.Deg2Rad;
                return finalPoint;
            }
        }
        return posP;
    }
}

public class Recto
{
    public Polar position;
    public Polar hsize;
    public Polar min;
    public Polar max;

    public Recto(Polar _position, float height)
    {
        position = _position;
        hsize = new Polar(Mathf.PI / MakeLevel.sides, height / 2);
        min = new Polar(position.a - hsize.a, position.r - hsize.r);
        max = new Polar(position.a + hsize.a, position.r + hsize.r);
    }

    public bool DoesItHit(Recto other)
    {
        float difference = Utils.AngleDifference(position.a, other.position.a);
        float slice = 360 / MakeLevel.sides / 4;
        return difference < slice && !(max.r < other.min.r || min.r > other.max.r);
    }
}
