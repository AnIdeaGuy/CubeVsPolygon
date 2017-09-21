using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoCollisions
{
    /// <summary>
    /// All the objects that the player can collide with.
    /// </summary>
    static public List<GameObject> hitMe = new List<GameObject>();

    /// <summary>
    /// Takes the given position and size and sees if it collides with anything in "hitMe".
    /// </summary>
    /// <param name="position">The Polar position to place the object.</param>
    /// <param name="positionZ">The z position to place the object.</param>
    /// <param name="size">The size to assume of the object.</param>
    /// <returns>Returns the Block type that the object collides with. (Block.AIR if there was no collision)</returns>
    static public GameObject[] WhatBlockDidItHit(Polar position, float positionZ, Vector3 size)
    {
        List<GameObject> blockList = new List<GameObject>();
        PolarBox boxA = new PolarBox(position, size.y); // The PolarBox to check collision against.
        foreach (GameObject obj in hitMe)
        {
            Block blok = obj.GetComponent<Blocky>().myType;

            PolarBox boxB = GetPolarBox(obj); // The PolarBox that might collide with "boxA".

            if (HitZ(positionZ, size.z, obj) && boxA.DoesItHit(boxB))
                blockList.Add(obj);
        }
        GameObject[] arr = new GameObject[blockList.Count];
        for (int i = 0; i < blockList.Count; i++)
            arr[i] = blockList[i];
        return arr;
    }

    /// <summary>
    /// The corrected polar coordinates place right on top of whatever block it hits inside hitMe.
    /// </summary>
    /// <param name="position">The Polar position to place the object.</param>
    /// <param name="positionZ">The z position to place the object.</param>
    /// <param name="size">The size to assume of the object.</param>
    /// <returns>Returns the polar coordinates of where to place the object so it's outside of the collided object.</returns>
    static public Polar ContactPointDown(Polar position, float positionZ, Vector3 size, GameObject obj)
    {
        PolarBox boxA = new PolarBox(position, size.y); // The PolarBox to check collision against.

       float boxBRotation = obj.transform.eulerAngles.z;
       PolarBox boxB = GetPolarBox(obj); // The PolarBox that might collide with "boxA".
       Block type = obj.GetComponent<Blocky>().myType;
       if (type == Block.GROUND && HitZ(positionZ, size.z, obj) && boxA.DoesItHit(boxB))
       {
           float radius = boxB.halfsize.r + boxA.halfsize.r; // Gets the distance the player should be from the center of the box
           return new Polar(boxBRotation * Mathf.Deg2Rad, boxB.position.r - radius);
       }
       
       return position.ToRad();
    }

    /// <summary>
    /// The corrected polar coordinates place right behind whatever block it hits inside hitMe.
    /// </summary>
    /// <param name="position">The Polar position to place the object.</param>
    /// <param name="positionZ">The z position to place the object.</param>
    /// <param name="size">The size to assume of the object.</param>
    /// <returns>Returns the polar coordinates of where to place the object so it's outside of the collided object.</returns>
    static public float ContactPointForward(Polar position, float positionZ, Vector3 size, GameObject obj)
    {
        PolarBox boxA = new PolarBox(position, size.y); // The PolarBox to check collision against.
        Polar position2 = position;
        position2.r -= .1f;
        PolarBox boxB = GetPolarBox(obj); // The PolarBox that might collide with "boxA".
        Block type = obj.GetComponent<Blocky>().myType;

        if ((type == Block.GROUND || type == Block.HALF) && HitZ(positionZ + .1f, size.z, obj, true) && boxA.DoesItHitForward(boxB))
        {
            float forward = obj.transform.localScale.z / 2 + size.z / 2;
            return obj.transform.position.z - forward;
        }
        return positionZ;
    }

    /// <summary>
    /// Converts a GameObject to a PolarBox.
    /// </summary>
    /// <param name="obj">The GameObject to convert.</param>
    /// <returns>Returns the new PolarBox.</returns>
    static public PolarBox GetPolarBox(GameObject obj)
    {
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;
        float r = Mathf.Sqrt(x * x + y * y);
        return new PolarBox(new Polar(obj.transform.rotation.eulerAngles.z, r), obj.transform.localScale.y);
    }

    /// <summary>
    /// Checks collision between two objects on the z axis only.
    /// </summary>
    /// <param name="firstZ">The z to place the first object.</param>
    /// <param name="firstLength">The length to assume of the first object.</param>
    /// <param name="secondObject">The second object as a GameObject.</param>
    /// <param name="isForward">Whether or not to check collision in front of the first object.</param>
    /// <returns>Returns whether or not the objects have collided on the z axis.</returns>
    static private bool HitZ(float firstZ, float firstLength, GameObject secondObject, bool isForward = false)
    {
        float firstHalfLength = firstLength / 2; // Half length of the first object.
        float secondHalfLength = secondObject.transform.localScale.z / 2; // Half length of the second object.
        float firstMax = firstZ + firstHalfLength; // Maximum z of the first object.
        float firstMin = firstZ - firstHalfLength; // Minimum z of the second object.
        float secondMax = secondObject.transform.position.z + secondHalfLength; // Maximum z of the second object.
        float secondMin = secondObject.transform.position.z - secondHalfLength; // Minimum z of the second object.

        if (isForward)
            return firstMax > secondMin && secondMax > firstMin;
        else
            return firstMax > secondMin && firstMin < secondMax;
    }
}

/// <summary>
/// It's a class that's sort of like a bent rectangle. It behaves similar to AABB logic, as in, it's for collision detection, except on a polar coordinate plane.
/// </summary>
public class PolarBox
{
    /// <summary>
    /// The polar coordinates to place this box at.
    /// </summary>
    public Polar position;
    /// <summary>
    /// The polar half size of the box. Half the radius thatr it takes up, half the degrees that it takes up.
    /// </summary>
    public Polar halfsize;
    /// <summary>
    /// The polar coordinates of the smallest possible values that a collision can be at.
    /// </summary>
    public Polar min;
    /// <summary>
    /// The polar coordinates of the largest possible values that a collision can be at.
    /// </summary>
    public Polar max;

    /// <summary>
    /// Creates a new PolarBox!
    /// </summary>
    /// <param name="_position">The position to assign this box.</param>
    /// <param name="height">The radius/height that the box should take up.</param>
    public PolarBox(Polar _position, float height)
    {
        position = _position;
        halfsize = new Polar(Mathf.PI / MakeLevel.sides, height / 2);
        min = new Polar(position.a - halfsize.a, position.r - halfsize.r);
        max = new Polar(position.a + halfsize.a, position.r + halfsize.r);
    }

    /// <summary>
    /// Checks if this PolarBox collides with another PolarBox.
    /// </summary>
    /// <param name="other">The other PolarBox to check collision with.</param>
    /// <returns>Returns whether there was a collision between the two boxes.</returns>
    public bool DoesItHit(PolarBox other)
    {
        float difference = Utils.AngleDifference(position.a, other.position.a);
        float slice = 360 / MakeLevel.sides / 3; // The minimum distance angle that the objects have to be from each other to collide.
        return difference < slice && max.r > other.min.r && min.r < other.max.r;
    }

    /// <summary>
    /// Checks if this PolarBox collides with another PolarBox in front of it.
    /// </summary>
    /// <param name="other">The other PolarBox to check collision with.</param>
    /// <returns>Returns whether there was a collision between the two boxes.</returns>
    public bool DoesItHitForward(PolarBox other)
    {
        float difference = Utils.AngleDifference(position.a, other.position.a);
        float slice = 360 / MakeLevel.sides / 4; // The minimum distance angle that the objects have to be from each other to collide.
        return difference < slice && max.r > other.min.r && min.r < other.max.r;
    }
}
