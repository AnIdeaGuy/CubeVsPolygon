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

			Recto2 rectP = new Recto2 (posP, size, angle);
			Recto2 rectB = new Recto2 (posB, scaleB, rotB);

			if (rectP.DoesItHit (rectB))
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

            Recto2 rectP = new Recto2(posP, size, angle);
            Recto2 rectB = new Recto2(posB, scaleB, rotB);

			if (rectP.DoesItHit(rectB))
            {
                Vector2 finalPoint = posB;
				float radius = rectB.hsize.x + rectP.hsize.y; // Gets the distance the player should be from the center of the box
				float aang = rotB * Mathf.Deg2Rad + Mathf.PI;
                finalPoint.x += Mathf.Cos(aang) * radius;
                finalPoint.y += Mathf.Sin(aang) * radius;
                return finalPoint;
            }
        }
        return posP;
    }
}

public class Recto2
{
	public Vector2 position;
	public Vector2 hsize;
	public Vector2 min;
	public Vector2 max;
	public float rotation; // in degrees

	public Recto2(Vector2 _position, Vector2 _size, float _rotation)
	{
		position = _position;
		hsize = _size / 2;
		rotation = Mathf.Round(_rotation);
	}

	private void SetBounds ()
	{
		min = position - hsize;
		max = position + hsize;
	}

	public bool DoesItHit(Recto2 other)
	{
		if (rotation != other.rotation)
			return false;

		float radiusMe = Mathf.Sqrt ((position.x * position.x) + (position.y * position.y));
		float radiusThem = Mathf.Sqrt ((other.position.x * other.position.x) + (other.position.y * other.position.y));

		// Straightens out the Rectos

		Debug.Log (position.x + " vs " + other.position.x);

		position.x -= Mathf.Cos (rotation * Mathf.Deg2Rad) * radiusMe;
		position.y -= Mathf.Sin (rotation * Mathf.Deg2Rad) * radiusMe;
		other.position.x -= Mathf.Cos (rotation * Mathf.Deg2Rad) * radiusThem;
		other.position.y -= Mathf.Sin (rotation * Mathf.Deg2Rad) * radiusThem;

		SetBounds ();
		other.SetBounds ();

		return !(max.x < other.min.x || min.x > other.max.x || max.y < other.min.y || min.y > other.max.y);
	}
}
