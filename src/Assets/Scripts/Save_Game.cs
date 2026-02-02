using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class Save_Game
{
    public static void Save_Button_Pressed(string save_name)
    {
        Data_To_Save data = new();

        int size = Data.size;

        data.general_data = Data.general_data;
        data.size = size;
        data.seed = Data.seed;
        data.grid_pos = Camera_Move.current_grid_pos;

        data.buildings = new();
        data.resources = new();
        data.units = new();
        data.light_map = new();
        for (int i = 0; i < size; i++)
            for (int j = 0; j <  size; j++)
            {
                if (Data.map[i, j].x != 0) data.buildings.Add(new Vector4(i, j, Data.map[i, j].x, Data.map[i, j].y));
                if (Data.resources_map[i, j] != null)
                    if (Data.resources_map[i, j].Any(x => x != 0))
                    {
                        data.resources.Add(new Resources_Tile());
                        data.resources[data.resources.Count - 1].coordinate = new Vector2Int(i, j);
                        data.resources[data.resources.Count - 1].array_of_resources = Data.resources_map[i, j];
                    }
                if (Data.units_map[i, j] != null) data.units.Add(Data.units_map[i, j]);
                if (Data.light_map[i, j] == 1) data.light_map.Add(new Vector3Int(i, j, Data.light_map[i, j]));
            }

        Saves.names[Convert.ToInt32(save_name.Split('_')[1])] = save_name;

        Save_Load_System.Update_Saves(Saves.names);

        Game_Serializator.SaveXml(data, Application.dataPath + '/' + save_name + ".xml");
    }
    
    public static void Load_Button_Pressed(string save_name)
    {
        Data_To_Save data = Game_Serializator.De_XML_Game(Application.dataPath + '/' + save_name + ".xml");

        Vector2Int[,] map = new Vector2Int[data.size, data.size];
        foreach (Vector4 x in data.buildings)
            map[(int)x.x, (int)x.y] = new Vector2Int((int)x.z, (int)x.w);

        int[,][] resources_map = new int[data.size, data.size][];
        foreach (Resources_Tile x in data.resources)
            resources_map[x.coordinate.x, x.coordinate.y] = x.array_of_resources;

        Unit[,] units_map = new Unit[data.size, data.size];
        foreach (Unit x in data.units)
            units_map[x.coord.x, x.coord.y] = x;

        int[,] light_map = new int[data.size, data.size];
        foreach (Vector3Int x in data.light_map)
            light_map[x.x, x.y] = x.z;

        Main.Preset_Game(data.seed, data.size, Generate_Map.Generate_Cave(data.seed, data.size), map, data.general_data, resources_map, data.grid_pos, units_map, light_map);
    }
}

[XmlRoot("Data_To_Save")]
public class Data_To_Save
{
    public List<Vector4> buildings;
    public List<Resources_Tile> resources;
    public int seed;
    public int size;
    public int[] general_data;
    public Vector3 grid_pos;
    public List<Unit> units;
    public List<Vector3Int> light_map;
}
[Serializable]
public class Resources_Tile
{
    public Vector2Int coordinate;
    public int[] array_of_resources;
}
[Serializable]
public class Unit
{
    public Vector2Int coord;
    public int[] carried_resources;
    public int AP;
    public int size;
    public int spec;
    public int carrying_capacity;
}

public class Game_Serializator
{
    static public void SaveXml(Data_To_Save data, string datapath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Data_To_Save));

        FileStream fs = new FileStream(datapath, FileMode.Create);
        serializer.Serialize(fs, data);
        fs.Close();
    }

    static public Data_To_Save De_XML_Game(string datapath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Data_To_Save));

        FileStream fs = new FileStream(datapath, FileMode.Open);
        Data_To_Save data = (Data_To_Save)serializer.Deserialize(fs);
        fs.Close();
        return data;
    }
}