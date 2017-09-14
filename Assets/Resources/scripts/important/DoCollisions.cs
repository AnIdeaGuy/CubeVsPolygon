using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoCollisions : MonoBehaviour
{
    static public List<GameObject> hitMe = new List<GameObject>();

    static public Block WhatHit(Polar posP, float z, Vector3 size)
    {
        foreach (GameObject obj in hitMe)
        {
            Block blok = obj.GetComponent<Blocky>().myType;
            float rotB = obj.transform.eulerAngles.z;

            Recto rectP = new Recto(posP, size.y);
            Recto rectB = GetRecto(obj);

            if (HitZ(z, size.z, obj) && rectP.DoesItHit(rectB))
                return blok;
        }
        return Block.AIR;
    }

    static public Polar ContactPointDown(Polar posP, float z, Vector3 size)
    {
        foreach (GameObject obj in hitMe)
        {
            float rotB = obj.transform.eulerAngles.z;

            Recto rectP = new Recto(posP, size.y);
            Recto rectB = GetRecto(obj);

            if (HitZ(z, size.z, obj) && rectP.DoesItHit(rectB))
            {
                float radius = rectB.hsize.r + rectP.hsize.r; // Gets the distance the player should be from the center of the box
                return new Polar(rotB * Mathf.Deg2Rad, rectB.position.r - radius);
            }
        }
        return posP.ToRad();
    }

    static public float ContactPointForward(Polar posP, float z, Vector3 size)
    {
        foreach (GameObject obj in hitMe)
        {
            float rotB = obj.transform.eulerAngles.z;
            Polar posP2 = posP;
            posP2.r -= .1f;
            Recto rectP = new Recto(posP2, size.y);
            Recto rectB = GetRecto(obj);

            if (HitZ(z + .1f, size.z, obj, true) && rectP.DoesItHitF(rectB))
            {
                float forward = obj.transform.localScale.z / 2 + size.z / 2;
                return obj.transform.position.z - forward;
            }
        }
        return z;
    }

    static private Recto GetRecto(GameObject obj)
    {
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;
        float r = Mathf.Sqrt(x * x + y * y);
        return new Recto(new Polar(obj.transform.rotation.eulerAngles.z, r), obj.transform.localScale.y);
    }

    static private bool HitZ(float pz, float pl, GameObject b, bool forward = false)
    {
        float phalf = pl / 2;
        float bhalf = b.transform.localScale.z / 2;
        float pMax = pz + phalf;
        float pMin = pz - phalf;
        float bMax = b.transform.position.z + bhalf;
        float bMin = b.transform.position.z - bhalf;

        if (forward)
            return pMax >= bMin && bMax > pMin;
        else
            return pMax > bMin && pMin < bMax;
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
        float slice = 360 / MakeLevel.sides / 3;
        return difference < slice && max.r > other.min.r && min.r < other.max.r;
    }

    public bool DoesItHitF(Recto other)
    {
        float difference = Utils.AngleDifference(position.a, other.position.a);
        float slice = 360 / MakeLevel.sides / 4;
        return difference < slice && max.r > other.min.r && min.r < other.max.r;
    }
}
