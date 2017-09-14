using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayahMove : MonoBehaviour
{

    public bool gravOn = false;
    public GameObject cubo;

    /// <summary>
    /// The player position in polar coordinates
    /// </summary>
    public Polar loc;
    public float locZ;
    private float gacc = 0;
    private bool hitD = false;
    private float upVelocity = 0;
    private float sidestepProg = 1;
    private float prestep = 0;
    private int sidestepDir = 0;
    private int ssdBuffer = 0;
    private const float STEP_SPEED = 6.0f;
    public const float JUMP_STRENGTH = 24.0f;
    private const float GRAVITY = 100.0f;
    private const float posMultiple = .05f;
    private const float goalZ = -5.0f;
    private const float goalSpeed = 2.0f;

    private float danceProg = 0;
    private float danceRate = .4f;

    void Start()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot.z = -90;
        transform.rotation = Quaternion.Euler(rot);

        // TODO: Replace cartesian with polar. It'll be cleaner
    }

    void Update()
    {
        TheInput();

        float x = transform.position.x;
        float y = transform.position.y;
        float r = Mathf.Sqrt(x * x + y * y);
        float a = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        // Allows for a negative radius
        if (Utils.AngleDifference(a, Mathf.Atan2(y, x), true) > Mathf.PI / 2)
            r = -r;
        loc = new Polar(a, r);
        locZ = transform.position.z;
        if (sidestepDir != 0)
            Sidestep();
        else
            CuboDance();

        if (gravOn)
        {
            loc.r += (-upVelocity + gacc) * Time.deltaTime;
            Collision();
        }

        if (locZ < MakeLevel.pKillZ)
        {
            // TODO: Game Over
        }

        transform.position = new Vector3(Mathf.Cos(loc.a) * loc.r, Mathf.Sin(loc.a) * loc.r, locZ);
        transform.rotation = Quaternion.Euler(0, 0, loc.a * Mathf.Rad2Deg);
    }

    private void TheInput()
    {
        if (Input.GetAxis("Vertical") > 0)
            gravOn = true;

        if (Input.GetButtonDown("Jump"))
        {
            if (hitD && gravOn)
            {
                upVelocity = JUMP_STRENGTH;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            float verticalness = upVelocity - gacc;
            if (verticalness > 0)
            {
                upVelocity = verticalness;
                gacc = 0;
            }
        }

        if (!Input.GetButton("Jump") || sidestepProg < 1)
        {
            if (!hitD && upVelocity - gacc > 0)
            {
                if (upVelocity > .9f)
                    upVelocity *= .8f;
                else
                    upVelocity = 0;
            }
        }

        if (Input.GetButtonDown("Left"))
        {
            if (sidestepDir == 0)
                SetStepStart();
            if (sidestepDir <= 0)
            {
                sidestepDir--;
                if (ssdBuffer <= 0)
                    ssdBuffer = sidestepDir;
            }
            else
                sidestepDir = -1;
        }

        if (Input.GetButtonDown("Right"))
        {
            if (sidestepDir == 0)
                SetStepStart();
            if (sidestepDir >= 0)
            {
                sidestepDir++;
                if (ssdBuffer >= 0)
                    ssdBuffer = sidestepDir;
            }
            else
                sidestepDir = 1;
        }
    }

    private void Sidestep()
    {
        int dir = ssdBuffer < 0 ? -1 : 1;
        if (sidestepProg < 1)
        {
            sidestepProg += STEP_SPEED * Time.deltaTime;
            if (sidestepProg > 1)
                sidestepProg = 1;
            float doAngle = prestep + dir * Mathf.PI * 2 / MakeLevel.sides * sidestepProg;
            Vector3 crot = cubo.transform.localRotation.eulerAngles;
            loc.a = doAngle;
            crot.z = 90 * -dir * sidestepProg;
            cubo.transform.localRotation = Quaternion.Euler(crot);
        }
        if (sidestepProg == 1)
        {
            if (sidestepDir == ssdBuffer)
                sidestepDir -= dir;
            ssdBuffer = sidestepDir;
            Vector3 crot = cubo.transform.localRotation.eulerAngles;
            crot.z = 0;
            cubo.transform.localRotation = Quaternion.Euler(crot);
            if (sidestepDir != 0)
                SetStepStart();
        }
    }

    private void Collision()
    {
        Polar loc2 = loc.ToDeg();
        loc2.r += Time.deltaTime;
        Block what = DoCollisions.WhatHit(loc2, locZ + .1f, transform.localScale);
        switch (what)
        {
            case Block.GROUND:
                if (!hitD)
                {
                    hitD = true;
                    upVelocity = 0;
                    gacc = 0;
                }
                SnapToGround();
                SnapToWall();
                break;
            default:
                hitD = false;
                DoGravity();
                break;
        }

        if (!SnapToWall(false))
            MoveToGoalZ();
    }

    private void MoveToGoalZ()
    {
        if (locZ < goalZ)
            locZ += goalSpeed * Time.deltaTime;
        if (locZ > goalZ)
            locZ = goalZ;
    }

    private void DoGravity()
    {
        gacc += GRAVITY * Time.deltaTime;
    }

    private bool CheckGround()
    {
        Polar loc2 = loc.ToDeg();
        loc2.r += Time.deltaTime;
        Vector3 fakeScale = transform.localScale;
        fakeScale.z /= 4;
        loc2 = DoCollisions.ContactPointDown(loc2, locZ, fakeScale);
        return Utils.RoundSpec(loc.r, .1f) != Utils.RoundSpec(loc2.r, .1f);
    }

    private bool SnapToGround(bool changeRadius = true)
    {
        Polar loc2 = loc.ToDeg();
        loc2.r += Time.deltaTime;
        loc2 = DoCollisions.ContactPointDown(loc2, locZ, transform.localScale);
        if (loc.r == loc2.r)
            return false;
        if (changeRadius)
            loc = loc2;
        return true;
    }

    private bool SnapToWall(bool changeZ = true)
    {
        float tempZ = DoCollisions.ContactPointForward(loc.ToDeg(), locZ, transform.localScale);
        if (tempZ == locZ)
            return false;
        if (changeZ)
            locZ = tempZ;
        return true;
    }

    private void SetStepStart()
    {
        sidestepProg = 0;
        prestep = loc.a;
        if (hitD)
            upVelocity = JUMP_STRENGTH;
        danceProg = 0;
    }

    private void CuboDance()
    {
        if (!hitD)
            danceProg = 0;
        float danceScale = 1 + Mathf.Sin(danceProg % (Mathf.PI * 2)) / 4;
        cubo.transform.localScale = new Vector3(danceScale, 1, 1);
        danceProg += danceRate;
    }
}
