using UnityEngine;

public class Camera_Move : MonoBehaviour
{
    [Range(1, 100)]
    public float speed;
    public GameObject game_object_to_move;
    public static int size;
    public static Vector3 grid_pos;
    public static Vector3 current_grid_pos;

    public GameObject settings_window;

    void Start()
    {
        Camera.main.transform.position = new Vector3(size * 0.75f, size * 0.375f, -10);
        game_object_to_move.transform.position = grid_pos;
    }

    Vector3 start_mouse_position;
    Vector3 start_grid_position;
    Vector3 current_mouse_position;

    void Update()
    {
        current_grid_pos = game_object_to_move.transform.position;
        if (settings_window.activeSelf) return;
        float horizontal_axis = Input.GetAxisRaw("Horizontal");
        float vertical_axis = Input.GetAxisRaw("Vertical");
        if (game_object_to_move.transform.position.x <= -((size / 40 - 1) * 30 + 18) & horizontal_axis == 1) horizontal_axis = 0;
        else if (game_object_to_move.transform.position.x >= (size / 40 - 1) * 30 + 19 & horizontal_axis == -1) horizontal_axis = 0;
        if (game_object_to_move.transform.position.y <= -((size / 40 - 1) * 15 + 10) & vertical_axis == 1) vertical_axis = 0;
        else if (game_object_to_move.transform.position.y >= (size / 40 - 1) * 15 + 8 & vertical_axis == -1) vertical_axis = 0;

        game_object_to_move.transform.position -= new Vector3(horizontal_axis, vertical_axis, 0).normalized / (speed * 10);
        if (Input.GetMouseButtonDown(2))
        {
            start_mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            start_grid_position = game_object_to_move.transform.position;
        }
        if (Input.GetMouseButton(2))
        {
            current_mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 distance = current_mouse_position - start_mouse_position;
            Vector3 new_grid_position = new Vector3(Mathf.Max((start_grid_position + distance).x, -((size / 40 - 1) * 30 + 18)), Mathf.Max((start_grid_position + distance).y, -((size / 40 - 1) * 15 + 10)), start_grid_position.z);
            new_grid_position = new Vector3(Mathf.Min((size / 40 - 1) * 30 + 19, new_grid_position.x), Mathf.Min((size / 40 - 1) * 15 + 8, new_grid_position.y), start_grid_position.z);
            game_object_to_move.transform.position = new_grid_position;
        }
    }
}