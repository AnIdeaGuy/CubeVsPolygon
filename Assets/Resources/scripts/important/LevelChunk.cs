using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChunk
{
    public int length;
    public List<Block[,]> map = new List<Block[,]>();
    private int at = 0;
    public int sidesMinimum;

    public LevelChunk(int _length)
    {
        length = _length;
        for (int i = 0; i < length; i++)
            map.Add(new Block[MakeLevel.sides, MakeLevel.DEPTH]);
        sidesMinimum = MakeLevel.sides;
        GeneratePlainMap();
    }

    public LevelChunk(List<List<List<Block>>> _map)
    {
        length = _map[0][0].Count;
        SetRawMap(_map);
    }

    public LevelChunk(List<Block[,]> _map)
    {
        length = _map.Count;
        Block[][,] copy2Me = new Block[_map.Count][,];
        _map.CopyTo(copy2Me);
        foreach (Block[,] b in copy2Me)
            map.Add(b);
    }

    public void Init()
    {
        List<Block[,]> map2 = new List<Block[,]>();
        List<Block[,]> map3 = new List<Block[,]>();
        for (int z = 0; z < length; z++)
        {
            map2.Add(new Block[MakeLevel.sides, MakeLevel.DEPTH]);
            map3.Add(new Block[MakeLevel.sides, MakeLevel.DEPTH]);
        }

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < sidesMinimum; x++)
                for (int y = 0; y < MakeLevel.DEPTH; y++)
                {
                    map2[z][x, y] = map[z][x, y];
                    map3[z][x, y] = map[z][x, y];
                }
            for (int x = sidesMinimum - 1; x < MakeLevel.sides; x++)
            {
                map2[z][x, 0] = Block.GROUND;
                map3[z][x, 0] = Block.GROUND;
                for (int y = 1; y < MakeLevel.DEPTH; y++)
                {
                    map2[z][x, y] = Block.AIR;
                    map3[z][x, y] = Block.AIR;
                }
            }
        }

        map = map2;

        // Rotate the map

        int offset = (int)Mathf.Round(Random.Range(0, MakeLevel.sides));

        for (int z = 0; z < length; z++)
            for (int x = 0; x < MakeLevel.sides; x++)
                for (int y = 0; y < MakeLevel.DEPTH; y++)
                {
                   
                    map[z][x, y] = map3[z][(x + offset) % MakeLevel.sides, y];
                }
    }

    public Block[,] GetNextRow()
    {
        if (at == length)
            return null;
        int preAt = at;
        at++;
        return map[preAt];
    }

    public void SetRawMap(List<List<List<Block>>> _map)
    {
        sidesMinimum = _map.Count;
        for (int i = 0; i < length; i++)
            map.Add(new Block[sidesMinimum, MakeLevel.DEPTH]);

        for (int x = 0; x < sidesMinimum; x++)
            for (int y = 0; y < MakeLevel.DEPTH; y++)
                for (int z = 0; z < length; z++)
                    map[z][x, y] = _map[x][y][z];
    }

    public LevelChunk Clone()
    {
        LevelChunk fresh = new LevelChunk(map);
        fresh.sidesMinimum = sidesMinimum;
        return fresh;
    }

    public void GeneratePlainMap()
    {
        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < MakeLevel.sides; x++)
            {
                map[z][x, 0] = Block.GROUND;
                for (int y = 1; y < MakeLevel.DEPTH; y++)
                    map[z][x, y] = Block.AIR;
            }
        }
    }

    public void GenerateRandomMap(/*Block[,] lastRow*/)
    {
        RoughDraftGen();

        // TODO: Making it fair
    }

    public void GenerateHurdleMap()
    {
        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < MakeLevel.sides; x++)
            {
                map[z][x, 0] = Block.GROUND;
                for (int y = 1; y < MakeLevel.DEPTH; y++)
                {
                    if (Mathf.Floor(z % ((length - 1) / 2)) == 0 && z != 0)
                    {
                        if (y == MakeLevel.DEPTH - 1)
                            map[z][x, y] = Block.SPIKE;
                        else
                            map[z][x, y] = Block.GROUND;
                    }
                    else
                        map[z][x, y] = Block.AIR;
                }
            }
        }
    }

    private void RoughDraftGen()
    {
        List<int[]> heightMap = new List<int[]>();
        for (int i = 0; i < length; i++)
        {
            int[] row = new int[MakeLevel.sides];
            for (int w = 0; w < MakeLevel.sides; w++)
                row[w] = (int) Mathf.Round(Random.Range(1, MakeLevel.DEPTH));
            heightMap.Add(row);
        }

        for (int z = 0; z < length; z++)
            for (int x = 0; x < MakeLevel.sides; x++)
            {
                int y;
                for (y = 0; y < heightMap[z][x]; y++)
                    map[z][x, y] = Block.GROUND;
                for (y = y;  y < MakeLevel.DEPTH; y++)
                {
                    map[z][x, y] = Block.AIR;
                }
            }
    }
}
