using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Units : MonoBehaviour
{
    Dictionary<Vector3Int, int> path_tiles_for_even = new();
    Dictionary<Vector3Int, int> path_tiles_for_odd = new();

    public Tilemap tilemap;
    public GameObject unit_window;
    public Tile[] units_tiles;
    public Tile[] path_lines;
    public Tile[] cave_tiles;

    public static float speed;

    public Animator animator;

    void Start()
    {
        path_tiles_for_even.Add(new Vector3Int(-1, 0, 0), 0);
        path_tiles_for_even.Add(new Vector3Int(-1, -1, 0), 1);
        path_tiles_for_even.Add(new Vector3Int(-1, 1, 0), 2);
        path_tiles_for_even.Add(new Vector3Int(1, 0, 0), 3);
        path_tiles_for_even.Add(new Vector3Int(0, -1, 0), 5);
        path_tiles_for_even.Add(new Vector3Int(0, 1, 0), 4);

        path_tiles_for_odd.Add(new Vector3Int(-1, 0, 0), 0);
        path_tiles_for_odd.Add(new Vector3Int(0, -1, 0), 1);
        path_tiles_for_odd.Add(new Vector3Int(0, 1, 0), 2);
        path_tiles_for_odd.Add(new Vector3Int(1, 0, 0), 3);
        path_tiles_for_odd.Add(new Vector3Int(1, -1, 0), 5);
        path_tiles_for_odd.Add(new Vector3Int(1, 1, 0), 4);
    }

    Vector3Int chosen_unit;
    bool is_moving = false;
    List<Vector3Int> path;
    bool anim_active;
    Vector3Int closest_cell;
    Vector3Int current_cell;
    void Update()
    {
        if (anim_active)
        {
            animator.gameObject.transform.position += (tilemap.CellToWorld(closest_cell) - tilemap.CellToWorld(current_cell)) / 200 * ((speed * 10 + 1) / 2);
            if (Vector3.Distance(animator.gameObject.transform.position, tilemap.CellToWorld(path[path.Count - 1])) < 0.02f)
            {
                Data.units_map[path[0].x, path[0].y] = null;
                anim_active = false;
                int m = (animator.GetInteger("walk_direction") - 1) / 3;
                tilemap.SetTile(path[path.Count - 1], units_tiles[Data.units_map[path[path.Count - 1].x, path[path.Count - 1].y].spec + m]);
                animator.SetInteger("walk_direction", 0);
            }


            else if (Vector3.Distance(animator.gameObject.transform.position, tilemap.CellToWorld(closest_cell)) < 0.02f)
            {
                current_cell = closest_cell;
                closest_cell = path[path.IndexOf(closest_cell) + 1];
                anim_prep();
            }
        }

        Vector3Int mouse_cell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        mouse_cell.z = -5;
        if (Input.GetMouseButtonDown(0))
        {
            if (tilemap.GetTile(mouse_cell) != null)
            {
                unit_window.SetActive(true);
                chosen_unit = mouse_cell;
            }
            else
            {
                unit_window.SetActive(false);
                chosen_unit = Vector3Int.left;
            }
        }
        if (Input.GetMouseButtonDown(1) & path != null & anim_active == false)
        {
            chosen_unit = Vector3Int.left;
            is_moving = false;
            Delete_Path();
            path = null;
        }

        if (Input.GetMouseButton(0) & chosen_unit != Vector3Int.left)
        {
            Dictionary<Vector3Int, int> direction = (mouse_cell.y % 2 == 0) ? path_tiles_for_even : path_tiles_for_odd;
            if (mouse_cell == chosen_unit & !is_moving)
            {
                is_moving = true;
                if (path != null) Delete_Path();
                path = new()
                {
                    mouse_cell
                };
                availability = true;
            }
            else if (mouse_cell != path[path.Count - 1] & path.Count <= Data.units_map[path[0].x, path[0].y].AP & !path.Contains(mouse_cell) & direction.Keys.Contains(path[path.Count - 1] - mouse_cell))
            {
                path.Add(mouse_cell);
                Draw_Path();
            }
        }
        
        if (Input.GetMouseButtonUp(0) & is_moving)
        {
            if (path[0] != path[path.Count - 1] & Data.raw_map[path[path.Count - 1].x, path[path.Count - 1].y] == 0 & availability)
            {
                Delete_Path();
                is_moving = false;
                tilemap.SetTile(path[0], null);
                Data.units_map[path[path.Count - 1].x, path[path.Count - 1].y] = Data.units_map[path[0].x, path[0].y];
                Data.units_map[path[path.Count - 1].x, path[path.Count - 1].y].coord = new Vector2Int(path[path.Count - 1].x, path[path.Count - 1].y);
                current_cell = path[0];
                closest_cell = path[1];
                anim_prep();
            }
            else
            {
                chosen_unit = Vector3Int.left;
                is_moving = false;
                Delete_Path();
                path = null;
            }
        }
    }
    void anim_prep()
    {
        animator.gameObject.transform.position = tilemap.CellToWorld(current_cell);
        Dictionary<Vector3Int, int> directions = (current_cell.y % 2 == 0) ? path_tiles_for_even : path_tiles_for_odd;
        animator.SetInteger("walk_direction", directions[closest_cell - current_cell] + 1);
        anim_active = true;
        Update_Light();
    }

    void Update_Light()
    {
        Main.Unit_Light_Influence(current_cell, -1, 5);
        Main.Unit_Light_Influence(closest_cell, 0, 5);
        for (int i = closest_cell.x - 5; i < closest_cell.x + 5; i++)
            for (int j = closest_cell.y - 5; j < closest_cell.y + 5; j++)
            {
                if (i < 0 & j < 0 & i >= Data.size & j >= Data.size) continue;

                if (Data.light_map[i, j] == 0)
                {
                    tilemap.SetTile(new Vector3Int(i, j, -Data.raw_map[i, j]), null);
                    if (Data.map[i, j].x > 0) tilemap.SetColor(new Vector3Int(i, j, -2), Color.black);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(i, j, -Data.raw_map[i, j]), cave_tiles[Data.raw_map[i, j]]);
                    if (Data.map[i, j].x > 0) tilemap.SetColor(new Vector3Int(i, j, -2), (Data.resources_map[i, j].Any(x => x < 0)) ? new Color(0.5f, 1, 1) : Color.white);
                }
            }
    }

    void Delete_Path()
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector3Int p = path[i];
            p.z = -4;
            tilemap.SetTile(p, null);
            p.z = -3;
            tilemap.SetTile(p, null);
        }
    }

    bool availability;
    void Draw_Path()
    {
        Vector3Int start = path[path.Count - 2];
        Vector3Int end = path[path.Count - 1];

        Dictionary<Vector3Int, int> path_tiles = (start.y % 2 == 0) ? path_tiles_for_even : path_tiles_for_odd;

        if (path_tiles.Keys.Contains(end - start))
        {
            int index = path_tiles[end - start];
            tilemap.SetTile(start + Vector3Int.forward * 2, path_lines[index]);
            tilemap.SetTile(end + Vector3Int.forward, path_lines[(index + 3) % 6]);
        }
        if (Data.raw_map[end.x, end.y] == 1 | Data.map[end.x, end.y].x > 0) availability = false;
    }
}