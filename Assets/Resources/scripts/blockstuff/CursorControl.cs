﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControl : MonoBehaviour
{
    private bool horhit = false;
    private bool verthit = false;
    public GameObject ground;
    public GameObject spikes;
    public GameObject half;
    public GameObject powerup;
    public GameObject delete;

	// Use this for initialization
	void Start ()
    {
        transform.localPosition = Vector3.zero;
	}

    // Update is called once per frame
    void Update()
    {
        ChunkConverter canvas = transform.parent.gameObject.GetComponent<ChunkConverter>();
        if (!canvas.saveActive && !canvas.loadActive)
        {
            if (Input.GetKeyDown("0"))
            {
                Place(Block.GROUND);
            }

            if (Input.GetKeyDown("1"))
            {
                Place(Block.SPIKE);
            }

            if (Input.GetKeyDown("2"))
            {
                Place(Block.HALF);
            }

            if (Input.GetKeyDown("3"))
            {
                Place(Block.POWERUP);
            }

            if (Input.GetKeyDown("delete"))
            {
                Place(Block.AIR);
            }

            if (Input.GetButtonDown("Up"))
            {
                if (!verthit)
                {
                    Vector3 pos = transform.position;
                    pos.z += MakeLevel.blockSize.z;
                    transform.position = pos;
                }
            }

            if (Input.GetButtonDown("Duck"))
            {
                if (!verthit)
                {
                    Vector3 pos = transform.position;
                    pos.z -= MakeLevel.blockSize.z;
                    transform.position = pos;
                }
            }

            if (Input.GetButtonDown("Right"))
            {
                if (!horhit)
                {
                    Vector3 pos = transform.position;
                    pos.x += MakeLevel.blockSize.x;
                    transform.position = pos;
                }
            }

            if (Input.GetButtonDown("Left"))
            {
                if (!horhit)
                {
                    Vector3 pos = transform.position;
                    pos.x -= MakeLevel.blockSize.x;
                    transform.position = pos;
                }
            }

            if (Input.GetButtonDown("Start"))
            {
                Vector3 pos = transform.position;
                pos.y += MakeLevel.blockSize.y;
                transform.position = pos;
            }

            if (Input.GetButtonDown("Select"))
            {
                Vector3 pos = transform.position;
                pos.y -= MakeLevel.blockSize.y;
                transform.position = pos;
            }
        }
    }

    public void Place(Block what)
    {
        GameObject obj = null;
        switch (what)
        {
            case Block.GROUND:
                obj = ground;
                break;
            case Block.SPIKE:
                obj = spikes;
                break;
            case Block.POWERUP:
                obj = powerup;
                break;
            case Block.HALF:
                obj = half;
                break;
            case Block.AIR:
                obj = delete;
                break;
        }
        if (obj != null)
            Instantiate(obj, transform.position, Quaternion.Euler(0, 0, -90), transform.parent);
    }
}
