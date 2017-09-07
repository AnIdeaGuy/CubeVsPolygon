using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayahMove : MonoBehaviour
{

    public bool gravOn = false;
    public GameObject cubo;

    private float gacc = 0;
    private bool hitD = false;
    private float upVelocity = 0;
    private float sidestepProg = 0;
    private float prestep = 0;
    private float sidestepRadius = 0;
    private int sidestepDir = 0;
    private int ssdBuffer = 0;
    private const float STEP_SPEED = 6.0f;
    public const float JUMP_STRENGTH = 17.0f;
    private const float GRAVITY = 40.0f;

    private float danceProg = 0;
    private float danceRate = .4f;

	void Start ()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot.z = -90;
        transform.rotation = Quaternion.Euler(rot);
	}

    void Update()
    {
        TheInput();

        if (sidestepDir != 0)
            Sidestep();
        else
            CuboDance();

        if (gravOn)
        {
            Vector2 pos = transform.position;
            pos += (-upVelocity + gacc) * GetDown() * Time.deltaTime;
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            Collision();
        }
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
            Vector3 pos = transform.position;
            Vector3 rot = transform.rotation.eulerAngles;
            Vector3 crot = cubo.transform.localRotation.eulerAngles;
            rot.z = doAngle * Mathf.Rad2Deg;
            sidestepRadius += (-upVelocity + gacc) * Time.deltaTime; // Applying gravity during rotation
            pos.x = Mathf.Cos(doAngle) * sidestepRadius;
            pos.y = Mathf.Sin(doAngle) * sidestepRadius;
            crot.z = 90 * -dir * sidestepProg;
            transform.position = pos;
            transform.rotation = Quaternion.Euler(rot);
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
        Vector2 posPlus = transform.position;
        posPlus += GetDown() * Time.deltaTime;
        Block what = DoCollisions.WhatHit(posPlus, transform.localScale, -transform.rotation.eulerAngles.z * Mathf.Deg2Rad);
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
                break;
            default:
                hitD = false;
                gacc += GRAVITY * Time.deltaTime;
                break;
        }
    }

    private Vector2 GetDown()
    {
        float rot = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rot), Mathf.Sin(rot));
    }

    private void SnapToGround()
    {
        Vector3 pos;
        Vector2 posPlus = transform.position;
        //posPlus += (-upVelocity + GRAVITY) * GetDown() * Time.deltaTime;
        pos = DoCollisions.ContactPoint(posPlus, transform.localScale, -transform.rotation.eulerAngles.z * Mathf.Deg2Rad);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    private void SetStepStart()
    {
        sidestepProg = 0;
        float x = transform.position.x;
        float y = transform.position.y;
        sidestepRadius = Mathf.Sqrt(x * x + y * y);
        prestep = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
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
