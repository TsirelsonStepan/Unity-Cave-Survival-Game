using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class Main_Menu_Ui_Logic : MonoBehaviour
{
    public GameObject new_game_window;
    public GameObject settings_window;
    public GameObject load_window;

    public TMP_InputField input_seed;
    public TMP_Dropdown dropdown_size;

    public TMP_Text[] saves;

    public void New_Game_Button_Pressed()
    {
        new_game_window.SetActive(!new_game_window.activeSelf);
        settings_window.SetActive(false);
        load_window.SetActive(false);
    }
    public void Start_New_Game()
    {
        int seed = (input_seed.text == "") ? Random.Range(0, 10000) : System.Convert.ToInt32(input_seed.text);
        int size = (3 - dropdown_size.value) * 40;
        int[,] raw_map = Generate_Map.Generate_Cave(seed, size);
        int[,] light_map = new int[size, size];
        Unit[,] units_map = new Unit[size, size];
        Vector3 grid_pos;
        Vector2Int pos;
        Unit[] units_presets = Data.units_presets;
        do
        {
            pos = new Vector2Int(Random.Range(5, size - 5), Random.Range(5, size - 5));
            if (raw_map[pos.x, pos.y] == 0)
            {
                units_map[pos.x, pos.y] = units_presets[0];

                units_map[pos.x, pos.y].size = 10;
                units_map[pos.x, pos.y].spec = 0;
                units_map[pos.x, pos.y].carrying_capacity = 6;
                units_map[pos.x, pos.y].AP = 4;

                units_map[pos.x, pos.y].coord = pos;
                int new_size_x = size + 37 - ((3 - size / 40) * 20);
                grid_pos = new Vector3(-((float)pos.x / size * new_size_x - new_size_x / 2), -((float)pos.y / size * new_size_x - new_size_x / 2) / 2, 0);
                break;
            }
        } while (true);

        for (int i = pos.x - 4; i < pos.x + 4; i++)
            for (int j = pos.y - 4; j < pos.y + 4; j++)
            {
                if (Random.Range(0, 100) < 60) raw_map[i, j] = 0; //clear start location
            }

        Main.Preset_Game(seed, size, raw_map, new Vector2Int[size, size], new int[9], new int[size, size][], grid_pos, units_map, light_map);

        SceneManager.LoadScene("Game");
        SceneManager.UnloadSceneAsync("Main_Menu");
    }

    public void Load_Button_Pressed()
    {
        new_game_window.SetActive(false);
        settings_window.SetActive(false);
        load_window.SetActive(!load_window.activeSelf);
        for (int i = 0; i < 3; i++)
        {
            if (Saves.names[i] != null)
            {
                saves[i].text = Saves.names[i];
                saves[i].color = Color.black;
            }
            else
            {
                saves[i].text = "Empty Save";
                saves[i].color = Color.grey;
            }
        }
    }
    public void Load_Game(string save_name)
    {
        if (Saves.names.Contains(save_name))
        {
            Save_Game.Load_Button_Pressed(save_name);
            SceneManager.LoadScene("Game");
            SceneManager.UnloadSceneAsync("Main_Menu");
        }
    }

    public void Delete_Save_Button_Pressed(int save_number)
    {
        Saves.Delete_Save(save_number);
        saves[save_number].text = "Empty Save";
        saves[save_number].color = Color.grey;
    }

    public void Settings_Button_Pressed()
    {
        settings_window.SetActive(!settings_window.activeSelf);
        new_game_window.SetActive(false);
        load_window.SetActive(false);
    }
    public void Exit_Button_Pressed()
    {
        Application.Quit();
    }
}