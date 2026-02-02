using System.IO;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Save_Load_System : MonoBehaviour
{
    static string datapath = Application.dataPath + "/SavedData.xml";
    static Current_Settings settings;

    public TMP_Dropdown dropdown_resolution;
    public TMP_Dropdown dropdown_language;
    public Toggle toggle_fullscreen;
    public Scrollbar scrollbar_units_speed;

    void Start()
    {
        if (File.Exists(datapath))
        {
            settings = System_Serializator.De_XML_System(datapath);
            dropdown_resolution.SetValueWithoutNotify(dropdown_resolution.options.FindIndex(s => s.text == settings.resolution_width.ToString() + 'x' + settings.resolution_height.ToString()));
            dropdown_language.SetValueWithoutNotify(dropdown_language.options.FindIndex(s => s.text == settings.language));
            toggle_fullscreen.SetIsOnWithoutNotify(settings.fullscreen);
            scrollbar_units_speed.value = settings.speed;
            Apply_Settings();
        }
        else
            Set_Default();
    }

    public static void Update_Saves(string[] saves)
    {
        settings.save_files_names = saves;
        System_Serializator.SaveXml(settings, datapath);
    }

    public void Apply_Settings()
    {
        GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        string[] resolutions = dropdown_resolution.options[dropdown_resolution.value].text.Split('x');
        settings.resolution_width = System.Convert.ToInt32(resolutions[0]);
        settings.resolution_height = System.Convert.ToInt32(resolutions[1]);
        settings.language = dropdown_language.options[dropdown_language.value].text;
        settings.fullscreen = toggle_fullscreen.isOn;
        settings.speed = scrollbar_units_speed.value;
        Saves.names = settings.save_files_names;
        Units.speed = settings.speed;
        Screen.SetResolution(settings.resolution_width, settings.resolution_height, settings.fullscreen);
        System_Serializator.SaveXml(settings, datapath);
    }
    public void Set_Default()
    {
        settings = new Current_Settings();
        settings.resolution_width = 1920;
        settings.resolution_height = 1080;
        settings.language = "English";
        settings.fullscreen = true;
        settings.save_files_names = new string[3];
        settings.speed = 1;
        Saves.names = settings.save_files_names;
        System_Serializator.SaveXml(settings, datapath);
    }
}

[XmlRoot("Current_Settings")]
public class Current_Settings
{
    public int resolution_width;
    public int resolution_height;
    public string language;
    public bool fullscreen;
    public string[] save_files_names;
    public float speed;
}

public class System_Serializator
{
    static public void SaveXml(Current_Settings settings, string datapath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Current_Settings));

        FileStream fs = new FileStream(datapath, FileMode.Create);
        serializer.Serialize(fs, settings);
        fs.Close();
    }

    static public Current_Settings De_XML_System(string datapath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Current_Settings));

        FileStream fs = new FileStream(datapath, FileMode.Open);
        Current_Settings settings = (Current_Settings)serializer.Deserialize(fs);
        fs.Close();
        return settings;
    }
}

public static class Saves
{
    public static string[] names;

    public static void Delete_Save(int save_number)
    {
        File.Delete(Application.dataPath + '/' + names[save_number] + ".xml");
        names[save_number] = null;
        Save_Load_System.Update_Saves(names);
    }
}