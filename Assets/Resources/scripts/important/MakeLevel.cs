using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The possible blocks that can be spawned.
/// </summary>
public enum Block { AIR, GROUND, SPIKE, HALF, POWERUP };

public class MakeLevel : MonoBehaviour
{
    static public MakeLevel makeLevel;

    public GameObject spike;
    public GameObject ground;
    public GameObject powerup;
    public GameObject half;

    static public int sides;
    public const int DEPTH = 4; // Powerups on depth-4, nothing else
    static public Vector3 blockSize = new Vector3(2.0f, 1.0f, 1.0f);
    public const float START_Z = 16.0f;
    /// <summary>
    /// Whether or not the game is paused.
    /// </summary>
    static public bool paused = false;
    /// <summary>
    /// The thing that determines the new row of shit to be spawned.
    /// </summary>
    private Block[,] spawnBar = new Block[sides, DEPTH]; // NOTE: x wraps around the pipe, y goes toward the center.
    private float progress = 0.0f;
    private float progressSinceLast = 0.0f;
    static public float speed = 14.0f;
    static public float killZ = -12.0f;
    static public float pKillZ = -10.0f;
    static public float killRadius;
    static public float awakeZ = -3.0f;
    static public float superRadius = 0;
    private LevelChunk currentChunk;
    private bool first = true;
    static public bool resetThisFrame = false;

    static public bool readyToLevelUp = false;
    private float timeSurvived = 0;
    private float surviveThisLong;
    private const float SURVIVAL_INCREMENT = 8.0f;
    private const float SURVIVE_START = 20.0f;
    private const int START_LEVEL = 5;

    private List<LevelChunk>[] allChunks = new List<LevelChunk>[12];

    private void Start()
    {
        sides = START_LEVEL;
        makeLevel = this;
        for (int i = 0; i < allChunks.Length; i++)
            allChunks[i] = new List<LevelChunk>();
        SpawnBlanks();
        surviveThisLong = SURVIVE_START;
    }

    /// <summary>
    /// Spawns a plain map to start the level with.
    /// </summary>
    private void SpawnBlanks()
    {
        currentChunk = new LevelChunk(25);
        for (int i = 0; i < 25; i++)
        {
            spawnBar = currentChunk.GetNextRow();
            SpawnThem(i);
        }
    }

    private void Init()
    {
        LoadAllChunks();
        MakeRandomChunk();
    }

    void Update()
    {
        if (first)
        {
            Init();
            first = false;
        }
        if (!paused)
        {
            if (readyToLevelUp)
            {
                if (DisplayControl.displayControl.whiteOutIsFull)
                {
                    readyToLevelUp = false;
                    sides++;
                    timeSurvived = 0;
                    surviveThisLong += SURVIVAL_INCREMENT;
                    ResetLevel(false);
                    MakeRandomChunk();
                }
            }
            float changeInZ = speed * Time.deltaTime;
            progress += changeInZ;
            progressSinceLast += changeInZ;
            if (progressSinceLast > blockSize.z)
            {
                progressSinceLast -= blockSize.z;
                DetermineSpawn();
                SpawnThem(progressSinceLast);
            }

            if (timeSurvived >= surviveThisLong)
            {
                if (sides < 7)
                    IncreaseLevel();
            }

            timeSurvived += Time.deltaTime;
        }
    }

    private void DetermineSpawn()
    {
        Block[,] row = currentChunk.GetNextRow();
        if (row == null)
        {
            MakeRandomChunk();
            row = currentChunk.GetNextRow();
        }
        spawnBar = row;
    }

    private void SpawnThem(float extra)
    {
        float radius = GetRadius(sides);
        radius = Mathf.Max(radius, GetRadius(12));
        superRadius = radius;
        killRadius = radius + 4;
        for (int i = 0; i < sides; i++)
            for (int w = 0; w < DEPTH; w++)
            {
                float rot = -Mathf.PI / 2 + Mathf.PI * 2 / sides * i;
                float newRadius = radius + blockSize.y / 2 - blockSize.y * w;
                Vector3 realPos = new Vector3(Mathf.Cos(rot) * newRadius, Mathf.Sin(rot) * newRadius, START_Z - extra);
                Quaternion realRot = Quaternion.Euler(new Vector3(0, 0, rot * Mathf.Rad2Deg));
                if (spawnBar[i, w] != Block.AIR)
                    Instantiate(Block2Obj(spawnBar[i, w]), realPos, realRot);
            }
    }

    private void IncreaseLevel()
    {
        if (PlayahMove.alive)
        {
            DisplayControl.displayControl.StartWhiteOut();
            readyToLevelUp = true;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    /// <summary>
    /// Resets the level.
    /// </summary>
    static public void ResetLevel(bool resetSides = true)
    {
        resetThisFrame = true;
        if (resetSides)
            sides = START_LEVEL;
        DoCollisions.hitMe.Clear();
        makeLevel.SpawnBlanks();
        makeLevel.timeSurvived = 0;
        makeLevel.surviveThisLong = SURVIVE_START;
        readyToLevelUp = false;
    }

    static public void UnresetLevel()
    {
        resetThisFrame = false;
    }

    private void MakeRandomChunk()
    {
        if (readyToLevelUp)
            currentChunk = new LevelChunk(12);
        else
            currentChunk = allChunks[sides - 1][(int)Mathf.Floor(Random.Range(0, allChunks[sides - 1].Count - .01f))].Clone();
        currentChunk.Init();
    }

    GameObject Block2Obj(Block blok)
    {
        switch (blok)
        {
            case Block.GROUND:
                return ground;
            case Block.SPIKE:
                return spike;
            case Block.POWERUP:
                return powerup;
            case Block.HALF:
                return half;
            default:
                return null;
        }
    }

    float GetRadius(int _sides)
    {
        return blockSize.x / (2 * Mathf.Sin(Mathf.PI / _sides));
    }

    public void LoadAllChunks()
    {
        LoadThis("first5", 5);
        LoadThis("second5", 5);
        LoadThis("ducking5", 5);
        LoadThis("fifth5", 5);
        LoadThis("actualfifth5", 5);
        LoadThis("powerup5", 5);
        
        LoadThis("first6", 6);
        LoadThis("pitfall6", 6);
        LoadThis("third6", 6);
        LoadThis("ducking6", 6);
        LoadThis("number6", 6);
        LoadThis("sexy6", 6);
        LoadThis("uhhh6", 6);
        LoadThis("final6", 6);

        LoadThis("first7", 7);
        LoadThis("second7", 7);
        LoadThis("pitfall7", 7);
        LoadThis("powerup7", 7);
        LoadThis("butt7", 7);
        LoadThis("ass7", 7);
        LoadThis("braindead7", 7);
        LoadThis("final7", 7);
    }

    public void LoadThis(string name, int sideNumber)
    {
        ChunkConverter cc = gameObject.GetComponent<ChunkConverter>();
        cc.SetFile(name);
        cc.LoadChunk();
        allChunks[sideNumber - 1].Add(new LevelChunk(cc.CopyMap()));
    }
}
