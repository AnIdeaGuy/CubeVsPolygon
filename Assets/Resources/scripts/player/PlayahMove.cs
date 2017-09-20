using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script that determines how the playah should move and collide.
/// </summary>
public class PlayahMove : MonoBehaviour
{
    /// <summary>
    /// The child cube that the user sees as the playah.
    /// </summary>
    public GameObject cubo;

    /// <summary>
    /// The player position in polar coordinates.
    /// </summary>
    public Polar location;
    /// <summary>
    /// The player z position.
    /// </summary>
    public float locationZ;
    /// <summary>
    /// The force at which the player moves downward.
    /// </summary>
    private float downVelocity = 0;
    /// <summary>
    /// Whether the playah is grounded.
    /// </summary>
    private bool isGrounded = false;
    /// <summary>
    /// Whether the player is still living.
    /// </summary>
    static public bool alive = true;
    /// <summary>
    /// The force at which the player move upward.
    /// </summary>
    private float upVelocity = 0;
    /// <summary>
    /// A 0-1 value that determines where the playah is during the stepping transition.
    /// </summary>
    private float stepProgress = 1;
    /// <summary>
    /// The angle that the playah was at before it began the stepping transition.
    /// </summary>
    private float initialStepAngle = 0;
    /// <summary>
    /// The direction that the playah should go during the stepping transition. Its absolute value is greater than one to keep track of how many times the player should move in that direction.
    /// </summary>
    private int stepDirection = 0;
    /// <summary>
    /// Don't remember exactly how this one works. But it allows the playah to change direction even when "initialStepAngle" is going in the opposite direction.
    /// </summary>
    private int stepDirectionBuffer = 0;
    /// <summary>
    /// The speed of the stepping transition.
    /// </summary>
    private const float STEP_SPEED = 6.0f;
    /// <summary>
    /// The power of the jump.
    /// </summary>
    public const float JUMP_STRENGTH = 24.0f;
    /// <summary>
    /// Ther power of the gravity.
    /// </summary>
    private const float GRAVITY = 100.0f;
    /// <summary>
    /// The target z that the playah will always try to get back to as long as there isn't a block in the way.
    /// </summary>
    private const float goalZ = -5.0f;
    /// <summary>
    /// The pace that the playah takes while moving toward the "goalZ".
    /// </summary>
    private const float GOAL_SPEED = 2.0f;

    /// <summary>
    /// The infinite timer that determines where "cubo" is witth its dance.
    /// </summary>
    private float danceProgress = 0;
    /// <summary>
    /// The rate at which "danceProgress"... well... progresses.
    /// </summary>
    private float danceRate = .4f;
    /// <summary>
    /// What do you think? It's for ducking. Figure it out. It's pretty obvious.
    /// </summary>
    public bool isDucking = false;

    /// <summary>
    /// The amount of hitpoints the playah has.
    /// </summary>
    static public float hp;
    /// <summary>
    /// The maximum amount "hp" is allowed to be.
    /// </summary>
    public const float HP_MAX = 4;
    /// <summary>
    /// Keeps track of how long the playah is invincible.
    /// </summary>
    public float invFrames = 0;
    /// <summary>
    /// How long the player can be invincible.
    /// </summary>
    public const float INV_MAX = 1;
    /// <summary>
    /// Whether or not the playah is hovering.
    /// </summary>
    private bool hovering = false;
    /// <summary>
    /// Whether or not the game is resetting.
    /// </summary>
    private bool resetting = false;

    void Start()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = -90;
        transform.rotation = Quaternion.Euler(rotation);
        hp = HP_MAX;
        invFrames = INV_MAX;
    }

    void Update()
    {
        if (!MakeLevel.paused)
        {
            if (MakeLevel.readyToLevelUp)
                hovering = true;
            if (alive)
            {
                MakeLevel.UnresetLevel();
                float x = transform.position.x;
                float y = transform.position.y;
                float r = Mathf.Sqrt(x * x + y * y);
                float a = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                // Allows for a negative radius
                if (Utils.AngleDifference(a, Mathf.Atan2(y, x), true) > Mathf.PI / 2)
                    r = -r;
                location = new Polar(a, r);
                locationZ = transform.position.z;

                if (hovering)
                {
                    location.r *= .9f;
                    if (location.r < .1f)
                        location.r = 0;
                    if (DisplayControl.displayControl.whiteOutIsFull)
                    {
                        hovering = false;
                        location.a = -Mathf.PI / 2;
                    }
                }
                else
                {
                    MakeLevel.resetThisFrame = false;
                    TheInput();

                    if (stepDirection != 0)
                        Sidestep();
                    else
                    {
                        CuboDance();
                        if (isDucking)
                            DoDucking();
                        else
                            DoUnducking();
                    }

                    if (invFrames < INV_MAX)
                    {
                        cubo.GetComponent<Renderer>().enabled = !cubo.GetComponent<Renderer>().enabled;
                        invFrames += Time.deltaTime;
                    }
                    else
                        invFrames = INV_MAX;

                    if (location.r > MakeLevel.killRadius)
                        alive = false;
                    Collision();

                    if (locationZ < MakeLevel.pKillZ)
                        alive = false;
                }

                transform.position = new Vector3(Mathf.Cos(location.a) * location.r, Mathf.Sin(location.a) * location.r, locationZ);
                transform.rotation = Quaternion.Euler(0, 0, location.a * Mathf.Rad2Deg);
            }
            else
            {
                cubo.SetActive(false);
                hp = 0;
                DisplayControl.displayControl.RestartHealth();
                if (Input.GetButtonDown("Start") && !resetting)
                {
                    resetting = true;
                    DisplayControl.displayControl.StartWhiteOut();
                }

                if (DisplayControl.displayControl.whiteOutIsFull)
                {
                    MakeLevel.ResetLevel();
                    alive = true;
                    DoReset();
                    cubo.SetActive(true);
                    resetting = false;
                }
            }
        }
    }

    /// <summary>
    /// Handles the playah input.
    /// </summary>
    private void TheInput()
    {
        isDucking = Input.GetButton("Duck");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                upVelocity = JUMP_STRENGTH;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            float verticalness = upVelocity - downVelocity;
            if (verticalness > 0)
            {
                upVelocity = verticalness;
                downVelocity = 0;
            }
        }

        if (!Input.GetButton("Jump") || stepProgress < 1)
        {
            if (!isGrounded && upVelocity - downVelocity > 0)
            {
                if (upVelocity > .9f)
                    upVelocity *= .8f;
                else
                    upVelocity = 0;
            }
        }

        if (Input.GetButtonDown("Left"))
        {
            if (stepDirection == 0)
                SetStepStart();
            if (stepDirection <= 0)
            {
                stepDirection--;
                if (stepDirectionBuffer <= 0)
                    stepDirectionBuffer = stepDirection;
            }
            else
                stepDirection = -1;
        }

        if (Input.GetButtonDown("Right"))
        {
            if (stepDirection == 0)
                SetStepStart();
            if (stepDirection >= 0)
            {
                stepDirection++;
                if (stepDirectionBuffer >= 0)
                    stepDirectionBuffer = stepDirection;
            }
            else
                stepDirection = 1;
        }
    }

    /// <summary>
    /// This is the function that controls the stepping transition that the playah does when changing lanes.
    /// </summary>
    private void Sidestep()
    {
        int dir = stepDirectionBuffer < 0 ? -1 : 1; // The normalized direction that the player should step in.
        if (stepProgress < 1)
        {
            stepProgress += STEP_SPEED * Time.deltaTime;
            if (stepProgress > 1)
                stepProgress = 1;
            float finalAngle = dir * Mathf.PI * 2 / MakeLevel.sides;
            float doAngle = initialStepAngle + finalAngle * stepProgress;
            Vector3 cuboRotation = cubo.transform.localRotation.eulerAngles;
            location.a = doAngle;
            cuboRotation.z = 90 * -dir * stepProgress;
            cubo.transform.localRotation = Quaternion.Euler(cuboRotation);
        }
        if (stepProgress == 1)
        {
            if (stepDirection == stepDirectionBuffer)
                stepDirection -= dir;
            stepDirectionBuffer = stepDirection;
            Vector3 crot = cubo.transform.localRotation.eulerAngles;
            crot.z = 0;
            cubo.transform.localRotation = Quaternion.Euler(crot);
            if (stepDirection != 0)
                SetStepStart();
        }
    }

    /// <summary>
    /// The function that handles collisions... what else?
    /// </summary>
    private void Collision()
    {
        Polar locationNew = location.ToDeg();
        locationNew.r += GetVerticalMovement();
        GameObject[] whatWasHit = DoCollisions.WhatBlockDidItHit(locationNew, locationZ + .1f, transform.localScale);
        bool moveZ = true;
        bool justFall = true;

        foreach (GameObject obj in whatWasHit)
        {
            Block block = obj.GetComponent<Blocky>().myType;
            switch (block)
            {
                case Block.GROUND:
                    bool floorSnap = SnapToGround(obj);
                    SnapToWall(obj);
                    if (GetVerticalMovement() < 0)
                        location = locationNew.ToRad();
                    if (!isGrounded)
                    {
                        if (floorSnap)
                        {
                            isGrounded = true;
                            upVelocity = 0;
                            downVelocity = 0;
                            justFall = false;
                        }
                    }
                    else
                        justFall = false;
                    break;

                case Block.SPIKE:
                    if (DoCollisions.GetPolarBox(obj).position.r > locationNew.r) // HOW? DAFUQ?
                        ReduceHP(1);
                    break;

                case Block.POWERUP:
                    if (hp < HP_MAX)
                        hp = Mathf.Min(hp + 1, HP_MAX);
                    DisplayControl.displayControl.RestartHealth();
                    obj.GetComponent<Blocky>().KillMe();
                    // TODO: Effects and shit
                    break;

                case Block.HALF:
                    if (!isDucking)
                    {
                        bool floorSnap2 = SnapToGround(obj);
                        SnapToWall(obj);
                        if (GetVerticalMovement() < 0)
                            location = locationNew.ToRad();
                        if (!isGrounded)
                        {
                            if (floorSnap2)
                            {
                                isGrounded = true;
                                upVelocity = 0;
                                downVelocity = 0;
                                justFall = false;
                            }
                        }
                        else
                            justFall = false;
                    }
                    break;
            }

            if (obj != null && SnapToWall(obj, false))
                moveZ = false;
        }

        if (justFall)
        {
            isGrounded = false;
            DoGravity();
            location = locationNew.ToRad();
        }

        if (moveZ)
            MoveToGoalZ();
    }

    /// <summary>
    /// Do the thing with the ducking and such.
    /// </summary>
    private void DoDucking()
    {
        Vector3 tempScale = cubo.transform.localScale;
        Vector3 tempPosition = cubo.transform.localPosition;
        tempScale.x = .5f;
        tempPosition.x = .5f;

        cubo.transform.localScale = tempScale;
        cubo.transform.localPosition = tempPosition;
    }

    private void DoUnducking()
    {
        Vector3 tempPosition = cubo.transform.localPosition;
        tempPosition.x = 0;
        cubo.transform.localPosition = tempPosition;
    }

    /// <summary>
    /// Moves to "goalZ" on the z axis.
    /// </summary>
    private void MoveToGoalZ()
    {
        if (locationZ < goalZ)
            locationZ += GOAL_SPEED * Time.deltaTime;
        if (locationZ > goalZ)
            locationZ = goalZ;
    }

    /// <summary>
    /// Changes the "downVelocity" to mimic gravity.
    /// </summary>
    private void DoGravity()
    {
        downVelocity += GRAVITY * Time.deltaTime;
    }

    private void ReduceHP(float amount)
    {
        if (invFrames == INV_MAX)
        {
            hp -= amount;
            if (hp <= 0)
                alive = false;
            else
                invFrames = 0;
            DisplayControl.displayControl.StartDamageAnimation();
        }
    }

    /// <summary>
    /// Gets the combination of UpVelocity and DownVelocity.
    /// </summary>
    /// <returns>What I just said.</returns>
    private float GetVerticalMovement()
    {
        float changeInRadius = (-upVelocity + downVelocity) * Time.deltaTime;
        return changeInRadius != 0 ? changeInRadius : .1f;
    }

    /// <summary>
    /// Snaps the playah to the ground.
    /// </summary>
    /// <param name="changeRadius">Whether to actually change the radius of the playah.</param>
    /// <returns>Returns true if the radius is different than the previus radius.</returns>
    private bool SnapToGround(GameObject obj, bool changeRadius = true)
    {
        Polar location2 = location.ToDeg();
        location2.r += GetVerticalMovement();
        float oldRadius = location2.r;
        Vector3 fakeScale = transform.localScale;
        fakeScale.z *= .5f;
        location2 = DoCollisions.ContactPointDown(location2, locationZ, fakeScale, obj);
        if (oldRadius == location2.r)
            return false;
        if (changeRadius)
            location = location2;
        return true;
    }

    /// <summary>
    /// Snaps the playah forward to the wall.
    /// </summary>
    /// <param name="changeZ">Whether yo actually change the z of the playah.</param>
    /// <returns>Returns true if the old Z doesn't match the new Z.</returns>
    private bool SnapToWall(GameObject obj, bool changeZ = true)
    {
        Vector3 fakeScale = transform.localScale;
        fakeScale.y *= .5f;
        fakeScale.x *= .5f;
        float tempZ = DoCollisions.ContactPointForward(location.ToDeg(), locationZ, fakeScale, obj);
        if (tempZ == locationZ)
            return false;
        if (changeZ)
            locationZ = tempZ;
        return true;
    }

    /// <summary>
    /// This gets called whenever the playah is ready to do the stepping transition.
    /// </summary>
    private void SetStepStart()
    {
        stepProgress = 0;
        initialStepAngle = location.a;
        if (isGrounded)
            upVelocity = JUMP_STRENGTH;
        danceProgress = 0;
    }

    /// <summary>
    /// Resets the playah.
    /// </summary>
    private void DoReset()
    {
        stepProgress = 1;
        stepDirection = 0;
        stepDirectionBuffer = 0;
        upVelocity = 0;
        downVelocity = 0;
        transform.position = new Vector3(0, 0, goalZ);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
        initialStepAngle = -90;
        location = new Polar(-Mathf.PI / 2, 0);
        locationZ = goalZ;
        cubo.transform.localRotation = Quaternion.identity;
        hp = HP_MAX;
        DisplayControl.displayControl.GameStart();
        invFrames = INV_MAX;
        cubo.GetComponent<Renderer>().enabled = true;
        hovering = false;
    }

    /// <summary>
    /// Animates the child cube "cubo" while it's running.
    /// </summary>
    private void CuboDance()
    {
        if (!isGrounded)
            danceProgress = 0;
        float danceScale = 1 + Mathf.Sin(danceProgress % (Mathf.PI * 2)) / 4;
        cubo.transform.localScale = new Vector3(danceScale, 1, 1);
        danceProgress += danceRate;
    }
}
