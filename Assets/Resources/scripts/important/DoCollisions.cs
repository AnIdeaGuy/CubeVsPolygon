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
            Block blok = obj.GetComponent<Blocky>().myType;
            Vector2 posB = obj.transform.position;
            Vector2 scaleB = obj.transform.localScale;
            float rotB = obj.transform.eulerAngles.z * Mathf.Deg2Rad;

            Recto rectP = new Recto(posP, size.x, size.y, angle);
            Recto rectB = new Recto(posB, scaleB.y, scaleB.x, rotB);

            if (BoxIntersect(rectP, rectB))
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
            float rotB = obj.transform.eulerAngles.z * Mathf.Deg2Rad;

            Recto rectP = new Recto(posP, size.x, size.y, angle);
            Recto rectB = new Recto(posB, scaleB.x, scaleB.y, rotB);

            if (BoxIntersect(rectP, rectB))
            {
                Vector2 finalPoint = posB;
                float radius = rectB.Width / 2 + rectP.Length / 2; // Gets the distance the player should be from the center of the box
                float aang = rotB + Mathf.PI;
                finalPoint.x += Mathf.Cos(aang) * radius;
                finalPoint.y += Mathf.Sin(aang) * radius;
                return finalPoint;
            }
        }
        return posP;
    }

    /*Credit to Markus Jarderot for this function*/
    static private bool BoxIntersect(Recto a, Recto b)
    {
        foreach (var polygon in new[] { a, b })
        {
            for (int i1 = 0; i1 < polygon.points.Length; i1++)
            {
                int i2 = (i1 + 1) % polygon.points.Length;
                var p1 = polygon.points[i1];
                var p2 = polygon.points[i2];

                var normal = new Vector2(p2.y - p1.y, p1.x - p2.x);

                double? minA = null, maxA = null;
                foreach (var p in a.points)
                {
                    var projected = normal.x * p.x + normal.y * p.y;
                    if (minA == null || projected < minA)
                        minA = projected;
                    if (maxA == null || projected > maxA)
                        maxA = projected;
                }

                double? minB = null, maxB = null;
                foreach (var p in b.points)
                {
                    var projected = normal.x * p.x + normal.y * p.y;
                    if (minB == null || projected < minB)
                        minB = projected;
                    if (maxB == null || projected > maxB)
                        maxB = projected;
                }

                if (maxA < minB || maxB < minA)
                    return false;
            }
        }
        return true;
    }
}

/*Credit to mrtig for this class*/
public class Recto
{
    public float Length;
    public float Width;
    public float Rotation;
    public Vector2 Center;
    public Vector2 TopLeft;
    public Vector2 TopRight;
    public Vector2 BottomLeft;
    public Vector2 BottomRight;
    public Vector2[] points = new Vector2[4];

    public Recto(Vector2 _origin, float _width, float _length, float angle)
    {
        Length = _length;
        Width = _width;
        Center = _origin;

        BottomLeft = new Vector2(Center.x - Width / 2, Center.y - Length / 2);
        BottomRight = new Vector2(Center.x + Width / 2, Center.y - Length / 2);
        TopLeft = new Vector2(Center.x - Width / 2, Center.y + Length / 2);
        TopRight = new Vector2(Center.x + Width / 2, Center.y + Length / 2);

        Rotate(angle);
    }

    private void InitCorners(Vector2 c)
    {
        BottomRight.x = (BottomRight.x + c.x);
        BottomRight.y = (BottomRight.y + c.y);

        BottomLeft.x = (BottomLeft.x + c.x);
        BottomLeft.y = (BottomLeft.y + c.y);

        TopRight.x = (TopRight.x + c.x);
        TopRight.y = (TopRight.y + c.y);

        TopLeft.x = (TopLeft.x + c.x);
        TopLeft.y = (TopLeft.y + c.y);

        points[0] = BottomRight;
        points[1] = TopRight;
        points[2] = TopLeft;
        points[3] = BottomLeft;
    }

    private void Move(Vector2 c)
    {
        InitCorners(new Vector2((c.x - Center.x), (c.y - Center.y)));
        Center.x = Center.x + (c.x - Center.x);
        Center.y = Center.y + (c.y - Center.y);
    }

    private void Rotate(float qtyRadians)
    {
        //Move center to origin
        Vector2 temp_orig = new Vector2(Center.x, Center.y);
        Move(new Vector2(0, 0));

        BottomRight = RotatePoint(BottomRight, qtyRadians);
        TopRight = RotatePoint(TopRight, qtyRadians);
        BottomLeft = RotatePoint(BottomLeft, qtyRadians);
        TopLeft = RotatePoint(TopLeft, qtyRadians);

        //Move center back
        Move(temp_orig);
    }

    private Vector2 RotatePoint(Vector2 p, float qtyRadians)
    {
        Vector2 temb_br = new Vector2(p.x, p.y);
        p.x = temb_br.x * Mathf.Cos(qtyRadians) - temb_br.x * Mathf.Sin(qtyRadians);
        p.y = temb_br.y * Mathf.Cos(qtyRadians) + temb_br.y * Mathf.Sin(qtyRadians);

        return p;
    }
}
