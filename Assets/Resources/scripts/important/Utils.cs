using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General tools to be used while coding
/// </summary>
public class Utils
{
    /// <summary>
    /// Rounds to a multiple of a given number.
    /// </summary>
    /// <param name="roundMe">The float to be rounded.</param>
    /// <param name="toValue">The float that the return should be a multiple of.</param>
    /// 
    /// <returns>Returns the rounded value</returns>
    static public float RoundSpec(float roundMe, float toValue)
    {
        if (toValue == 0)
            return roundMe;
        float to = 1 / toValue;
        return Mathf.Round(roundMe * to) / to;
    }

    /// <summary>
    /// Rounds a Vector3 to a multiple of a given number.
    /// </summary>
    /// <param name="roundMe">The Vector3 to be rounded.</param>
    /// <param name="toValue">The float that the "x", "y", and "z" of "roundMe" should be multiples of.</param>
    /// <returns>Returns the rounded Vector3</returns>
    static public Vector3 RoundVector(Vector3 roundMe, float toValue)
    {
        return new Vector3(RoundSpec(roundMe.x, toValue), RoundSpec(roundMe.y, toValue), RoundSpec(roundMe.z, toValue));
    }

    /// <summary>
    /// Rounds a Polar to a multiple of a given number.
    /// </summary>
    /// <param name="roundMe">The Polar to be rounded.</param>
    /// <param name="toValueAngle">The float that the "a" of "roundMe" should be a multiple of.</param>
    /// <param name="toValueRadius">The float that the "r" of "roundMe" should be a multiple of.</param>
    /// <returns>Returns the rounded Polar</returns>
    static public Polar RoundPolar(Polar roundMe, float toValueAngle, float toValueRadius)
    {
        return new Polar(RoundSpec(roundMe.a, toValueAngle), RoundSpec(roundMe.r, toValueRadius));
    }

    /// <summary>
    /// Finds the difference between two angles.
    /// </summary>
    /// <param name="a">The first angle.</param>
    /// <param name="b">The second angle.</param>
    /// <param name="inRadians">Whether or not parameters "a" and "b" are in radians.</param>
    /// <returns>Returns the difference between the given angless.</returns>
    static public float AngleDifference(float a, float b, bool inRadians = false)
    {
        float a2 = inRadians ? a * Mathf.Rad2Deg : a;
        float b2 = inRadians ? b * Mathf.Rad2Deg : b;
        float difference = Mathf.Abs(a2 - b2) % 360;

        if (difference > 180)
            difference = 360 - difference;

        return inRadians ? difference * Mathf.Deg2Rad : difference;
    }
}

/// <summary>
/// A polar coordinates struct. Made mainly for movement and collision.
/// </summary>
public struct Polar
{
    /// <summary>
    /// The angle property of the coordinates
    /// </summary>
    public float a;
    /// <summary>
    /// The radius property of the coordinates
    /// </summary>
    public float r;

    /// <summary>
    /// Creates a new polar struct.
    /// </summary>
    /// <param name="_a">The angle parameter.</param>
    /// <param name="_r">the radius parameter.</param>
    public Polar(float _a, float _r)
    {
        a = _a;
        r = _r;
    }

    /// <summary>
    /// Makes a copy of the coordinates with the angle converted to degrees.
    /// WARNING: Returns incorrect coordinates if the angle is already in degrees.
    /// </summary>
    /// <returns>Returns a copy of the polar coordinates with the angle in degrees.</returns>
    public Polar ToDeg()
    {
        return new Polar(a * Mathf.Rad2Deg, r);
    }

    /// <summary>
    /// Makes a copy of the coordinates with the angle converted to radians.
    /// WARNING: Returns incorrect coordinates if the angle is already in radians.
    /// </summary>
    /// <returns>Returns a copy of the polar coordinates with the angle in radians.</returns>
    public Polar ToRad()
    {
        return new Polar(a * Mathf.Deg2Rad, r);
    }
}