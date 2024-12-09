using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//This class is for 2d only. No flying things

public class UnityBoardClass
{
    // Static variable to hold the single instance
    private static UnityBoardClass instance;

    // Private constructor to prevent instantiation
    private UnityBoardClass()
    {
        // Initialization code here
    }

    // Public static method to provide global access to the instance
    public static UnityBoardClass Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UnityBoardClass();
            }
            return instance;
        }
    }

    // Board layers
    private GameObject[,] board_map_layer;
    private GameObject[,] board_entity_layer;
    private GameObject[,] board_player_layer;

    private (float, float)[,] coordinates;

    private int grid_n_columns;
    private int grid_n_rows;
    private float tile_width;
    private float tile_height;
    private float parent_width;
    private float parent_height;

    private float position_pivot_x;
    private float position_pivot_y;

    private (int, int)[] bumps_array;
    private (int, int)[] highlighted_coordinates = Array.Empty<(int, int)>();

    // Public constructor
    public void UseExistingBoard(
        Transform[] all_childs,
        (float, float) tile_width_height,
        (float, float) parent_width_height
    )
    {
        (tile_width, tile_height) = tile_width_height;
        (parent_width, parent_height) = parent_width_height;
        board_map_layer = BoardLibrary.ListTo2dGrid(all_childs, true);

        BuildBoard();
    }

    public void CreateNewBoard(
        GameObject tile_prefab,
        Transform parent,
        (float, float) parent_width_height
    )
    {
        (parent_width, parent_height) = parent_width_height;

        tile_width = parent_width / grid_n_columns;
        tile_height = parent_height / grid_n_rows;

        board_map_layer = BoardLibrary.CreateNewBoard(
            grid_n_columns,
            grid_n_rows,
            tile_prefab,
            (tile_width, tile_height),
            parent
        );
        Transform[] child_array = BoardLibrary.CollectChildren(parent.transform);

        board_map_layer = BoardLibrary.ListTo2dGrid(child_array, true);

        BuildBoard();
    }

    private void BuildBoard()
    {
        foreach (GameObject item in board_map_layer)
        {
            if (item.GetComponent<SelectionTile>())
            {
                Debug.Log($"Hold up! {item.name} is selection tile!");
            }
        }
        (grid_n_columns, grid_n_rows) = BoardLibrary.GetWidthAndHeight(board_map_layer);
        coordinates = new (float, float)[grid_n_columns, grid_n_rows];
        string[] parts;
        string xPart,
            yPart;
        int x,
            y;
        foreach (var pos in board_map_layer)
        {
            // Split the string at '/'
            parts = pos.name.Split(' ');

            // Extract and parse the X value
            xPart = parts[0].Split(':')[1]; // Get the value after "X:"
            x = int.Parse(xPart);

            // Extract and parse the Y value
            yPart = parts[1].Split(':')[1]; // Get the value after "Y:"
            y = int.Parse(yPart);

            coordinates[x, y] = (pos.transform.localPosition.x, pos.transform.localPosition.y);
        }

        board_entity_layer = BoardLibrary.InitializeNewLayer(grid_n_columns, grid_n_rows);
        board_player_layer = BoardLibrary.InitializeNewLayer(grid_n_columns, grid_n_rows);

        position_pivot_x = (parent_width - tile_width) / 2;
        position_pivot_y = (parent_height - tile_height) / 2;

        bumps_array = AIScanner.SetMapLimits((grid_n_columns, grid_n_rows));
        Debug.Log($"parent_width:{parent_width},parent_height:{parent_height}");
    }

    public GameObject GetObjectOnEntityLayer(int x, int y)
    {
        return board_entity_layer[x, y];
    }

    public (int, int) GetNormalizedPos((float, float) value)
    {
        // Iterate through the 2D array
        for (int x = 0; x < coordinates.GetLength(0); x++) // Loop through rows
        {
            for (int y = 0; y < coordinates.GetLength(1); y++) // Loop through columns
            {
                // Check if the current entry matches the target value
                if (
                    Mathf.Approximately(coordinates[x, y].Item1, value.Item1)
                    && Mathf.Approximately(coordinates[x, y].Item2, value.Item2)
                )
                {
                    return (x, y); // Return the position if a match is found
                }
            }
        }

        // Return (-1, -1) if the value is not found
        return (-1, -1);
    }

    public (float, float) GetDenormalizedPos((int, int) value)
    {
        return coordinates[value.Item1, value.Item2];
    }

    public GameObject GetObjectOnMapLayer(int x, int y)
    {
        return board_map_layer[x, y];
    }

    public (int, int)[] GetBoardLimits()
    {
        return bumps_array;
    }

    public (int, int) GetGridWidthHeight()
    {
        return (grid_n_columns, grid_n_rows);
    }

    public void CreateValueEntityTable((int, int) grid_coord, GameObject entity)
    {
        board_entity_layer[grid_coord.Item1, grid_coord.Item2] = entity;
    }
    public void DelValueEntityTable((int, int) grid_coord){
        
        board_entity_layer[grid_coord.Item1, grid_coord.Item2]=null;
    }

    public void MoveEntity((int, int) old_coord, (int, int) new_coord)
    {
        board_entity_layer[new_coord.Item1, new_coord.Item2] = board_entity_layer[
            old_coord.Item1,
            old_coord.Item2
        ];
        board_entity_layer[old_coord.Item1, old_coord.Item2] = null;
        Debug.Log($"Entity now on {new_coord}");
    }

    public void HighlightAround(string tag, int radius, int x_coord, int y_coord)
    {
        (int, int)[] bumps = GetBoardLimits();
        List<(int, int)> bump_list = bumps.ToList();
        foreach (var item in board_entity_layer)
        {
            if (item != null)
            {
                // Assuming board_entity_layer is a 2D array
                for (int x = 0; x < board_entity_layer.GetLength(0); x++)
                {
                    for (int y = 0; y < board_entity_layer.GetLength(1); y++)
                    {

                        if (
                            board_entity_layer[x, y] != null
                            && board_entity_layer[x, y].gameObject.CompareTag(tag)
                        )
                        {
                            bump_list.Add((x, y));
                        }
                    }
                }
            }
        }
        List<(int, int)> new_bump_list = new List<(int, int)>();
        foreach (var item in bump_list)
        {
            if (new_bump_list.Contains(item) == false)
            {
                new_bump_list.Add(item);
            }
        }
        new_bump_list.Remove((x_coord, y_coord));
        bumps = new_bump_list.ToArray();

        highlighted_coordinates = AIScanner.ScanForWalkable(
            (x_coord, y_coord),
            radius,
            bumps,
            null,
            null
        );
        GameObject tile = null;
        foreach (var highlighted_coordinate in highlighted_coordinates)
        {
            tile = GetObjectOnMapLayer(highlighted_coordinate.Item1, highlighted_coordinate.Item2);
            tile.GetComponent<TileInteraction>().SetWalkableState(true);
        }
    }

    public void RemoveHighlight()
    {
        if (highlighted_coordinates == null || highlighted_coordinates.Length == 0)
        {
            Debug.Log("No coordinate highlighted");
        }
        else
        {
            foreach (var highlighted_coordinate in highlighted_coordinates)
            {
                try
                {
                    GetObjectOnMapLayer(highlighted_coordinate.Item1, highlighted_coordinate.Item2)
                        .GetComponent<TileInteraction>()
                        .SetWalkableState(false);
                }
                catch (Exception e)
                {
                    Debug.Log(
                        $"Problematic coord: ({highlighted_coordinate.Item1},{highlighted_coordinate.Item2})"
                    );
                    throw;
                }
            }
        }
    }
}
