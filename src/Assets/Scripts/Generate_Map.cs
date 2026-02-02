using System.Collections.Generic;
using UnityEngine;

public class Generate_Map
{
	static int[,] map;
	static int fillborder = 50;
	static int repit = 10;
	static int neigbours = 5;

	public static int[,] Generate_Cave(int seed, int size)
	{
		System.Random r = new System.Random(seed);
		map = new int[size, size];
		List<Vector2Int> all_tiles = new();
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)
			{
				all_tiles.Add(new Vector2Int(i, j));
				if (i == 0 | j == 0 | i == size - 1 | j == size - 1) map[i, j] = 1;
				else map[i, j] = (r.Next(0, 100) < fillborder) ? 1 : 0;
			}
		for (int i = 0; i < repit; i++) Smooth_Map(size);
		Remove_Walls(size);
        return map;
	}

	static void Smooth_Map(int size)
	{
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
				if (Neighbours_Count(i, j, size) > neigbours) map[i, j] = (map[i, j] + 1) % 2;
            }
    }

	static int Neighbours_Count(int x, int y, int size)
	{
		int count = 0;
		if (x > 0 & y > 0 & x < size - 1 & y < size - 1)
			for (int i = x - 1; i <= x + 1; i++)
				for (int j = y - 1; j <= y + 1; j++)
				{
					if (map[x, y] != map[i, j]) count++;
				}
		return count;
    }

	static void Remove_Walls(int size)
	{
        for (int i = 1; i < size - 1; i++)
            for (int j = 1; j < size - 1; j++)
                if (map[i, j] == 1 & ((map[i + 1, j] + map[i - 1, j] == 0) | (map[i, j + 1] + map[i, j - 1] == 0))) map[i, j] = 0;
    }
}