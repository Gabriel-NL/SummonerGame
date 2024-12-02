using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private (int, int) current_selected_coordinates;
    private (int, int)[] bumps_array;



    // Public constructor
    public void UseExistingBoard(
        Transform[] all_childs,
        (float, float) tile_width_height,
        (float, float) parent_width_height
    )
    {
        board_map_layer = BoardLibrary.ListTo2dGrid(all_childs, true);
        (grid_n_columns, grid_n_rows) = BoardLibrary.GetWidthAndHeight(board_map_layer);
        coordinates = new (float, float)[grid_n_columns, grid_n_rows];

        (tile_width, tile_height) = tile_width_height;
        (parent_width, parent_height) = parent_width_height;

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
        BuildBoard();
    }

    private void BuildBoard()
    {
        string[] parts;
        string xPart,yPart;
        int x,y;
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
            
            coordinates[x,y] = (
                pos.transform.localPosition.x,
                pos.transform.localPosition.y
            );
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

    public GameObject GetObjectOnEntityLayer()
    {
        return board_entity_layer[
            current_selected_coordinates.Item1,
            current_selected_coordinates.Item2
        ];
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

    public void MoveEntity((int, int) old_coord, (int, int) new_coord)
    {
        board_entity_layer[new_coord.Item1, new_coord.Item2] = board_entity_layer[
            old_coord.Item1,
            old_coord.Item2
        ];
        board_entity_layer[old_coord.Item1, old_coord.Item2] = null;
        Debug.Log($"Entity now on {new_coord}");
    }

    public void SetPositionOnGrid(Vector3 position)
    {
        (int, int) normalized_current_pos = GetNormalizedPos((position.x, position.y));
        Debug.Log("CoordPos:" + normalized_current_pos);
        current_selected_coordinates = normalized_current_pos;
    }

    public (int, int) GetCoordinates()
    {
        return current_selected_coordinates;
    }
}
