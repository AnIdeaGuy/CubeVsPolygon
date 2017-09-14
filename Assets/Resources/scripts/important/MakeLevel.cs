using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Block { AIR, GROUND, SPIKE };

public class MakeLevel : MonoBehaviour
{
    /// <summary>
    /// Enum Block is for spawning spaces.
    /// </summary>
    public GameObject spike;
    public GameObject ground;

    static public int sides = 10;
    public const int DEPTH = 3;
    static public Vector3 blockSize = new Vector3(2.0f, 1.0f, 1.0f);
    public const float START_Z = 12.0f;
    /// <summary>
    /// The thing that determines the new row of shit to be spawned.
    /// </summary>
    private Block[,] spawnBar = new Block[sides, DEPTH]; // NOTE: x wraps around the pipe, y goes toward the center.
    private float progress = 0.0f;
    private float progressSinceLast = 0.0f;
    static public float speed = 6.0f;
    static public float killZ = -12.0f;
    static public float pKillZ = -10.0f;
    static public float awakeZ = -3.0f;
    static public float superRadius = 0;
    private LevelChunk currentChunk;

    private List<LevelChunk> allChunks = new List<LevelChunk>();

    void Start()
    {
        LoadAllChunks();
        MakeRandomChunk();
    }

    void Update()
    {
        float changeInZ = speed * Time.deltaTime;
        progress += changeInZ;
        progressSinceLast += changeInZ;
        if (progressSinceLast > blockSize.z)
        {
            progressSinceLast -= blockSize.z;
            DetermineSpawn();
            SpawnThem(progressSinceLast);
        }
    }

    void DetermineSpawn()
    {
        Block[,] row = currentChunk.GetNextRow();
        if (row == null)
        {
            MakeRandomChunk();
            row = currentChunk.GetNextRow();
        }
        spawnBar = row;
    }

    void SpawnThem(float extra)
    {
        float radius = GetRadius(sides);
        radius = Mathf.Max(radius, GetRadius(10));
        superRadius = radius;
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

    private void MakeRandomChunk()
    {
        currentChunk = allChunks[(int)Mathf.Round(Random.Range(0, allChunks.Count-1))].Clone();
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
        LoadThis("furst");
    }

    public void LoadThis(string name)
    {
        ChunkConverter cc = gameObject.GetComponent<ChunkConverter>();
        cc.SetFile(name);
        cc.LoadChunk();
        allChunks.Add(new LevelChunk(cc.CopyMap()));
    }
}
