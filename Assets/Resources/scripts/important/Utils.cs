using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    /// <summary>
    /// Rounds to a multiple of a given number
    /// </summary>
    /// <param name="roundMe">The value to be rounded</param>
    /// <param name="toValue">The value that the return should be a multiple of</param>
    /// 
    /// <returns>Returns the rounded value</returns>
    static public float RoundSpec(float roundMe, float toValue)
    {
        if (toValue == 0)
            return roundMe;
        float to = 1 / toValue;
        return Mathf.Round(roundMe * to) / to;
    }

    static public Vector3 RoundVector(Vector3 roundMe, float toValue)
    {
        return new Vector3(RoundSpec(roundMe.x, toValue), RoundSpec(roundMe.y, toValue), RoundSpec(roundMe.z, toValue));
    }

    static public Polar RoundPolar(Polar roundMe, float toValue)
    {
        return new Polar(RoundSpec(roundMe.a, toValue), RoundSpec(roundMe.r, toValue));
    }

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

public struct Polar
{
    public float a;
    public float r;
    public Polar(float _a, float _r)
    {
        a = _a;
        r = _r;
    }
}