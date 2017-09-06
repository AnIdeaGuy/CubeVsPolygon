using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChunk
{
    private int length;
    public List<Block[,]> map = new List<Block[,]>();
    private int at = 0;

    public LevelChunk(int _length)
    {
        length = _length;
        for (int i = 0; i < length; i++)
            map.Add(new Block[MakeLevel.sides, MakeLevel.DEPTH]);

        GenerateRandomMap();
    }

    public Block[,] GetNextRow()
    {
        if (at == length)
            return null;
        int preAt = at;
        at++;
        return map[preAt];
    }

    public void UseFileAsMap(string file)
    {
        // TODO: Everything involving this
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
