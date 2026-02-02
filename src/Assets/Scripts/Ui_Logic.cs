using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ui_Logic : MonoBehaviour
{
    public TMP_Text population_text;
    public TMP_Text housing_text;
    public TMP_Text[] saves;

    public GameObject buildings_window;
    public GameObject settings_window;
    public GameObject specialization_window;
    public GameObject save_window;

    public static int chosen_building;
    public static bool is_building_chosen = false;

    public TMP_Text FPS_count_text;

    public void Building_Chosen(int building_index)
    {
        is_building_chosen = true;
        buildings_window.SetActive(false);
        chosen_building = building_index;
    }
    public void Buildings_Button_Pressed()
    {
        buildings_window.SetActive(!buildings_window.activeSelf);
    }
    public void Back_To_Main_Menu()
    {
        SceneManager.LoadScene("Main_Menu");
        SceneManager.UnloadSceneAsync("Game");
    }
    public void Save_Load_Window_Open(int m)
    {
        for (int i = 0; i < 3; i++)
        {
            if (Saves.names[i] != null)
            {
                saves[i + m].text = Saves.names[i];
                saves[i + m].color = Color.black;
            }
            else
            {
                saves[i + m].text = "Empty Save";
                saves[i + m].color = Color.grey;
            }
        }
    }
    public void Save_Button_Pressed(string save_name)
    {
        Save_Game.Save_Button_Pressed(save_name);
        int save_number = System.Convert.ToInt32(save_name.Split('_')[1]);
        saves[save_number].text = save_name;
        saves[save_number].color = Color.black;
    }

    public void Load_Button_Pressd(string save_name)
    {
        if (Saves.names.Contains(save_name))
        {
            Camera_Move.grid_pos = Vector3.zero;
            Save_Game.Load_Button_Pressed(save_name);
            SceneManager.LoadScene("Game");
        }
    }
    public void Delete_Button_Pressed(int save_number)
    {
        Saves.Delete_Save(save_number);
        saves[save_number].text = "Empty Save";
        saves[save_number].color = Color.grey;
        saves[save_number + 3].text = "Empty Save";
        saves[save_number + 3].color = Color.grey;
    }

    void Update()
    {
        int fps = (int)(1f / Time.deltaTime);
        FPS_count_text.text = System.Convert.ToString(fps);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            buildings_window.SetActive(false);
            specialization_window.SetActive(false);
            settings_window.SetActive(!settings_window.activeSelf);
            save_window.SetActive(false);
        }
        if (Input.GetMouseButtonDown(1))
        {
            specialization_window.SetActive(false);
            buildings_window.SetActive(false);
        }
        if (Main.data_display)
        {
            Display_General_Data();
            Main.data_display = false;
        }
    }
    public void Display_General_Data()
    {
        population_text.text = Data.general_data[0].ToString();
        housing_text.text = Data.general_data[1].ToString();
    }
}