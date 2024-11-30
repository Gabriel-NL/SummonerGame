using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This class is for 2d only. No flying things

public class UnityBoardClass
{
    // Board layers
    private GameObject[,] board_map_layer;
    private GameObject[,] board_entity_layer;
    private GameObject[,] board_player_layer;

    private int grid_n_columns;
    private int grid_n_rows;
    private float tile_width;
    private float tile_height;
    private float parent_width;
    private float parent_height;
    private float stepSizeX=0;
    private float stepSizeY=0;
    private (int,int) current_selected_coordinates;
    private (int, int)[] bumps_array;

    // Public constructor
    public UnityBoardClass(Transform[] all_childs, (float,float) tile_width_height,(float,float) parent_width_height)
    {
        board_map_layer = BoardLibrary.ListTo2dGrid(all_childs, true);
        (grid_n_columns, grid_n_rows) = BoardLibrary.GetWidthAndHeight(board_map_layer);

        (tile_width,tile_height)=tile_width_height;
        (parent_width,parent_height) = parent_width_height;

        BuildBoard();
    }

    public UnityBoardClass(GameObject tile_prefab, Transform parent,(float,float) parent_width_height)
    {
        (parent_width,parent_height) = parent_width_height;

        float new_tile_size_width = parent_width / grid_n_columns;
        float new_tile_size_height = parent_height / grid_n_rows;
        tile_width=new_tile_size_width;
        tile_height=new_tile_size_height;

        board_map_layer = BoardLibrary.CreateNewBoard(
            grid_n_columns,
            grid_n_rows,
            tile_prefab,
            (tile_width,tile_height),
            parent
        );
        BuildBoard();
    }

    private void BuildBoard()
    {
        board_entity_layer = BoardLibrary.InitializeNewLayer(
            grid_n_columns,
            grid_n_rows
        );
        board_player_layer = BoardLibrary.InitializeNewLayer(
            grid_n_columns,
            grid_n_rows
        );
        stepSizeX=parent_width / (grid_n_columns - 1);
        stepSizeY= parent_height / (grid_n_rows - 1);
        bumps_array = AIScanner.SetMapLimits((grid_n_columns,grid_n_rows));
    }

    public GameObject GetObjectOnEntityLayer(int x, int y){
        return board_entity_layer[x,y];
    }
    public GameObject GetObjectOnMapLayer(int x, int y){
        return board_map_layer[x,y];
    }
    public (int,int)[] GetBoardLimits(){
        return bumps_array;
    }
    public (int,int) GetGridWidthHeight(){
        return (grid_n_columns,grid_n_rows);
    }

public void CreateValueEntityTable((int, int) grid_coord, GameObject entity)
    {
        board_entity_layer[grid_coord.Item1, grid_coord.Item2] = entity;
    }
    public void MoveEntity((int, int) old_coord,(int, int) new_coord)
    {

        board_entity_layer[new_coord.Item1,new_coord.Item2] = board_entity_layer[old_coord.Item1, old_coord.Item2];
        board_entity_layer[old_coord.Item1, old_coord.Item2] = null;
    }

    public void SetPositionOnGrid(Vector3 position)
    {
        (int, int) normalized_current_pos = NormalizeStepValue(
            (position.x, position.y)
        );
        Debug.Log("CoordPos:" + normalized_current_pos);
        current_selected_coordinates = normalized_current_pos;
    }

    public (int,int) GetCoordinates(){
        return current_selected_coordinates;
    }

    public (int, int) NormalizeStepValue(
        (float, float) value
    )
    {
        // Normalize the coordinates
        int new_x = Mathf.RoundToInt((value.Item1 + (parent_width / 2)) / stepSizeX);
        int new_y = Mathf.RoundToInt((value.Item2 + (parent_height / 2)) / stepSizeY);

        (int, int) normalized_value = (new_x, new_y);
        // Return the normalized grid indices
        //Debug.Log($"Normalized Value: {normalized_value}");
        return normalized_value;
    }

    public (float, float) RevertNormalization(
        (int, int) gridValue
    )
    {
        float new_x = gridValue.Item1 * stepSizeX - (parent_width - stepSizeX) / 2;
        float new_y = gridValue.Item2 * stepSizeY - (parent_height - stepSizeY) / 2;

        (float, float) denormalized_value = (new_x, new_y);

        Debug.Log("Denormalization: " + denormalized_value);
        return denormalized_value;
    }
    
    



    

    
}
