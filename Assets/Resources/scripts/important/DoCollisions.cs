using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoCollisions : MonoBehaviour
{
    static public List<GameObject> hitMe = new List<GameObject>();

    static public Block WhatHit(Vector2 posP, Vector2 size, float angle)
    {
        foreach (GameObject obj in hitMe)
        {
			Block blok = obj.GetComponent<Blocky> ().myType;
			Vector2 posB = obj.transform.position;
			Vector2 scaleB = obj.transform.localScale;
			float rotB = obj.transform.eulerAngles.z;

			Recto rectP = new Recto(posP, size.y, angle);
			Recto rectB = new Recto(posB, scaleB.x, rotB);

			if (rectP.DoesItHit(rectB))
				return blok;
        }
        return Block.AIR;
    }

    static public Vector2 ContactPoint(Vector2 posP, Vector2 size, float angle)
    {
        foreach (GameObject obj in hitMe)
        {
            Vector2 posB = obj.transform.position;
            Vector2 scaleB = obj.transform.localScale;
            float rotB = obj.transform.eulerAngles.z;

            Recto rectP = new Recto(posP, size.y, angle);
            Recto rectB = new Recto(posB, scaleB.x, rotB);

			if (rectP.DoesItHit(rectB))
            {
                Vector2 finalPoint = posB;
				float radius = rectB.hsize.r + rectP.hsize.r; // Gets the distance the player should be from the center of the box
				float aang = rotB * Mathf.Deg2Rad + Mathf.PI;
                finalPoint.x += Mathf.Cos(aang) * radius;
                finalPoint.y += Mathf.Sin(aang) * radius;
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

    public Recto(Vector2 _position, float height, float _rotation)
    {
        float transr = Mathf.Sqrt(_position.x * _position.x + _position.y * _position.y);
        position = new Polar(Mathf.Round(_rotation), transr);
        hsize = new Polar(Mathf.PI / MakeLevel.sides, height / 2);
        min = new Polar(position.a - hsize.a, position.r - hsize.r);
        max = new Polar(position.a + hsize.a, position.r + hsize.r);
    }

    public bool DoesItHit(Recto other)
    {

        float difference = Mathf.Abs(position.a - other.position.a) % 360;

        if (difference > 180)
            difference = 360 - difference;

        float slice = 360 / MakeLevel.sides / 4;
        return difference < slice && !(max.r < other.min.r || min.r > other.max.r);
    }
}
