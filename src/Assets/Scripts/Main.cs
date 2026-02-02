using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Main : MonoBehaviour
{
    public Tilemap tilemap_main;
    public Tile[] building_tiles;
    public Tile[] cave_tiles;
    public Tile[] units_tiles;

    public static void Preset_Game(int seed, int size, int[,] raw_map, Vector2Int[,] map, int[] general_data, int[,][] map_resources, Vector3 grid_pos, Unit[,] units_map, int[,] light_map)
    {
        Data.seed = seed;
        Data.size = size;
        Data.raw_map = raw_map;
        Data.map = map;
        Data.resources_map = map_resources;
        Data.general_data = general_data;
        Data.units_map = units_map;
        Data.light_map = light_map;

        Camera_Move.size = size;
        Camera_Move.grid_pos = grid_pos;
    }
    void Start()
    {
        int size = Data.size;
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                if (Data.map[i, j].x != 0)
                {
                    tilemap_main.SetTile(new Vector3Int(i, j, -2), building_tiles[Data.map[i, j].x]);
                    if (Data.resources_map[i, j].Any(x => x < 0)) tilemap_main.SetColor(new Vector3Int(i, j, -2), new Color(0.5f, 1, 1));
                }
                if (Data.units_map[i, j] != null)
                {
                    tilemap_main.SetTile(new Vector3Int(i, j, -5), units_tiles[Data.units_map[i, j].spec]);
                    Unit_Light_Influence(new Vector3Int(i, j, -5), 0, 5);
                }
            }
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                if (Data.light_map[i, j] == 1) tilemap_main.SetTile(new Vector3Int(i, j, -Data.raw_map[i, j]), cave_tiles[Data.raw_map[i, j]]);
            }
    }

    void Update()
    {
        Place_Building_Plan();

        Building_Plan_Preview();
    }

    Vector3Int current_building_cell;
    Vector3Int previous_building_cell;
    void Place_Building_Plan()
    {
        if (Input.GetMouseButtonDown(0) | (Ui_Logic.chosen_building == 0 & Input.GetMouseButton(0)))
        {
            Vector3Int current_mouse_pos_cell = tilemap_main.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            current_mouse_pos_cell.z = -2;
            if (Data.light_map[current_mouse_pos_cell.x, current_mouse_pos_cell.y] == 1 & Ui_Logic.is_building_chosen & Data.raw_map[current_mouse_pos_cell.x, current_mouse_pos_cell.y] == 0 & (Data.map[current_mouse_pos_cell.x, current_mouse_pos_cell.y].x == 0 | Ui_Logic.chosen_building == 0))
            {
                if (Ui_Logic.chosen_building == 0)
                {
                    Data.map[current_mouse_pos_cell.x, current_mouse_pos_cell.y] = new Vector2Int(0, -1);
                    tilemap_main.SetTile(current_mouse_pos_cell, null);
                }//remove building
                else
                {
                    Data.map[current_mouse_pos_cell.x, current_mouse_pos_cell.y].x = Ui_Logic.chosen_building;
                    tilemap_main.SetTile(current_mouse_pos_cell, building_tiles[Ui_Logic.chosen_building]);
                    Data.resources_map[current_mouse_pos_cell.x, current_mouse_pos_cell.y] = Data.costs[Ui_Logic.chosen_building];
                    tilemap_main.SetColor(current_mouse_pos_cell, new Color(0.5f, 1, 1));
                    Ui_Logic.is_building_chosen = false;

                }//place building plan
            }
        }
    }
    void Building_Plan_Preview()
    {
        current_building_cell = tilemap_main.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        current_building_cell.z = -1;
        if (Data.light_map[current_building_cell.x, current_building_cell.y] == 0 | current_building_cell.x >= Data.size | current_building_cell.y >= Data.size | current_building_cell.x < 0 | current_building_cell.y < 0) return; //keep cell for building inside borders
        if (Data.raw_map[current_building_cell.x, current_building_cell.y] == 0 & Ui_Logic.is_building_chosen & (Data.map[current_building_cell.x, current_building_cell.y].x == 0 | Ui_Logic.chosen_building == 0))
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ui_Logic.is_building_chosen = false;
                tilemap_main.SetTile(previous_building_cell, null);
            }//disable building selection
            if (previous_building_cell != current_building_cell)
            {
                tilemap_main.SetTile(previous_building_cell, null);
                previous_building_cell = current_building_cell;
                tilemap_main.SetTile(current_building_cell, building_tiles[Ui_Logic.chosen_building]);
            } //update building preview cell
        } //conditions for preview
    }

    public static bool data_display;
    void Update_General_Data(int effect, int index)
    {
        data_display = true;
        Data.general_data[index] += effect;
    }

    public static void Unit_Light_Influence(Vector3Int unit, int m, int radius)
    {
        for (int i = unit.x - radius; i < unit.x + radius; i++)
            for (int j = unit.y - radius; j < unit.y + radius; j++)
            {
                if (Vector3Int.Distance(unit, new Vector3Int(i, j, -5)) < radius - 1 & i >= 0 & j >= 0 & i < Data.size & j < Data.size)
                {
                    Data.light_map[i, j] = 1 + m;
                }
            }
    }

    public static Unit Create_Unit(int size, int spec, int carrying_capacity, int AP)
    {
        Unit unit = new();
        unit.size = size;
        unit.spec = spec;
        unit.carrying_capacity = carrying_capacity;
        unit.AP = AP;
        return unit;
    }
}

public static class Data
{
    public static int seed;
    public static int size;

    public static int[] general_data = new int[9];

    public static int[,] raw_map;
    public static Vector2Int[,] map;
    public static int[,][] resources_map;
    public static Unit[,] units_map;
    public static int[,] light_map;

    public static int[] specializable_buildings = new int[] { 2 };
    public static Dictionary<int, Vector2Int[]> influences = new()
    {
        { 0, new Vector2Int[]{  } },
        { 1, new Vector2Int[]{ new Vector2Int(1, 3) } },
        { 2, new Vector2Int[]{  } },
        { 3, new Vector2Int[]{ new Vector2Int(8, 10) } },
        { 4, new Vector2Int[]{ new Vector2Int(1, 2) } },
        { 5, new Vector2Int[]{  } }
    };
    public static int[][] costs = new int[][]
    {
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new int[] { -3, 0, 0, 0, 0, 0, 0, 1 },
        new int[] { -2, 0, -4, 0, 0, 0, 0, 2 }
    };
    public static Unit[] units_presets = new Unit[]
    {
        Main.Create_Unit(10, 0, 6, 4),//scout
        Main.Create_Unit(10, 1, 2, 6)//warior
    };
}

/*
 * Resources:
 * wood
 * raw stones
 * rawhide
 * leather
 * stone blocks
 * iron ore
 * cast iron
 * turns to build
 */