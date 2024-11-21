using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class BoardLibrary : MonoBehaviour
{
    public static List<Transform> CollectChildren(Transform board_parent)
    {
        List<Transform> childTransforms = new List<Transform>();

        foreach (Transform child in board_parent)
        {
            childTransforms.Add(child);
        }
        return childTransforms;
    }

    public static GameObject[,] ListTo2dGrid(List<Transform> childTransforms, bool force_fix)
    {
        List<float> unique_x_values = GetCoordinateUniqueValues('x', childTransforms);
        List<float> unique_y_values = GetCoordinateUniqueValues('y', childTransforms);

        // Create a 2D array to represent the grid
        int gridWidth = unique_x_values.Count;
        int gridHeight = unique_y_values.Count;

        GameObject[,] board_map_filled = new GameObject[gridWidth, gridHeight];

        foreach (Transform obj in childTransforms)
        {
            int yIndex = unique_y_values.IndexOf(obj.transform.position.y);
            int xIndex = unique_x_values.IndexOf(obj.transform.position.x);
            obj.gameObject.name = $"X:{xIndex} Y:{yIndex}";
            bool is_available = board_map_filled[xIndex, yIndex] == null;
            //false,false= exception
            //false,true= fix
            //true,false= is available, moves on
            //true,true= ignores and moves on
            if (is_available == false && force_fix == false)
            {
                throw new System.ArgumentException("The specified position is already occupied!");
            }
            else
            {
                board_map_filled[xIndex, yIndex] = obj.gameObject;
            }
        }

        return board_map_filled;
    }

    public static GameObject[,] CreateNewBoard(
        int grid_x,
        int grid_y,
        GameObject tile,
        GameObject parent
    )
    {
        GameObject[,] new_board = new GameObject[grid_x, grid_y];
        //W.I.P
        //width=200 height 200
        //

        float parent_tile_size_width = parent.GetComponent<RectTransform>().rect.width;
        float parent_tile_size_height = parent.GetComponent<RectTransform>().rect.height;
        float new_tile_size_width = parent_tile_size_width / grid_x;
        float new_tile_size_height = parent_tile_size_height / grid_y;

        for (int x = 0; x < grid_x; x++)
        {
            for (int y = 0; y < grid_y; y++)
            {
                GameObject new_tile = Instantiate(tile);
                new_tile.name = $"X:{x}/Y:{y}";

                RectTransform rect = new_tile.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(new_tile_size_width, new_tile_size_height);

                // Calculate the X and Y positions
                float offset_x = (grid_x - 1) * new_tile_size_width / 2; // Center grid on the X-axis
                float offset_y = (grid_y - 1) * new_tile_size_height / 2; // Center grid on the Y-axis

                float pos_x = x * new_tile_size_width - offset_x; // X position
                float pos_y = y * new_tile_size_height - offset_y; // Y position
                rect.localPosition = new Vector2(pos_x, pos_y);

                // Optionally, set the new tile's parent
                new_tile.transform.SetParent(parent.transform, false);
                new_board[x, y] = new_tile;
            }
        }

        return new_board;
    }

    public static GameObject[,] InitializeNewLayer(GameObject[,] map_layer)
    {
        int width = map_layer.GetLength(0); // Get the number of rows (first dimension)
        int height = map_layer.GetLength(1); // Get the number of columns (second dimension)
        GameObject[,] new_layer = new GameObject[width, height];
        return new_layer;
    }

    public static (int, int) NormalizeStepValue(
    (float, float) value,
    (float, float) dimensions_w_h,
    (int, int) gridSize
)
{
    // Calculate the step size for each grid cell
    float stepSizeX = dimensions_w_h.Item1 / (gridSize.Item1 - 1);
    float stepSizeY = dimensions_w_h.Item2 / (gridSize.Item2 - 1);

    // Normalize the coordinates
    int new_x = Mathf.RoundToInt((value.Item1 + (dimensions_w_h.Item1 / 2)) / stepSizeX);
    int new_y = Mathf.RoundToInt((value.Item2 + (dimensions_w_h.Item2 / 2)) / stepSizeY);

    (int, int) normalized_value=(new_x, new_y);
    // Return the normalized grid indices
    //Debug.Log($"Normalized Value: {normalized_value}");
    return normalized_value;
}


    public static float RevertNormalization(int gridValue, float min, float max, int gridSize)
    {
        float stepSize = (max - min) / gridSize; // Calculate the step size
        return gridValue * stepSize + min; // Scale and shift back
    }

    public static (float, float) RevertNormalization(
        (int, int) gridValue,
        (float, float) dimensions_w_h,
        (int, int) gridSize
    )
    {
        // Calculate the min and max bounds for both X and Y
        float min_x = -dimensions_w_h.Item1 / 2;
        float max_x = dimensions_w_h.Item1 / 2;
        float min_y = -dimensions_w_h.Item2 / 2;
        float max_y = dimensions_w_h.Item2 / 2;

        // Calculate step sizes
        float stepSizeX = (max_x - min_x) / gridSize.Item1;
        float stepSizeY = (max_y - min_y) / gridSize.Item2;

        // Revert grid values back to world positions
        float worldX = gridValue.Item1 * stepSizeX + min_x;
        float worldY = gridValue.Item2 * stepSizeY + min_y;

        return (worldX, worldY);
    }

    public static GameObject FindChildWithTag(Transform parent, string tag)
    {
        // Loop through all the children of the parent
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag)) // Check if the child's tag matches
            {
                return child.gameObject;
            }
        }

        // Return null if no child with the specified tag is found
        return null;
    }

    public static void ShowAllCoordinates(List<Transform> childTransforms, bool is_top_down)
    {
        List<float> unique_x_values = GetCoordinateUniqueValues('x', childTransforms);
        List<float> unique_y_values = GetCoordinateUniqueValues('y', childTransforms);
        List<float> unique_z_values = GetCoordinateUniqueValues('z', childTransforms);

        string msg = "";

        if (!is_top_down)
        {
            // Assuming childTransforms contains objects with x, y, and z positions
            for (int z = 0; z < unique_z_values.Count; z++)
            {
                Debug.Log($"Layer {z}: {unique_z_values[z]}");

                for (int x = 0; x < unique_x_values.Count; x++)
                {
                    msg = "";
                    for (int y = 0; y < unique_y_values.Count; y++)
                    {
                        msg += $"({x}/{y}) / ";
                    }
                    Debug.Log(msg);
                }
            }
        }
        else
        {
            // Assuming childTransforms contains objects with x, y, and z positions
            for (int y = 0; y < unique_y_values.Count; y++)
            {
                Debug.Log($"Layer {y + 1}: {unique_y_values[y]}");
                for (int x = 0; x < unique_x_values.Count; x++)
                {
                    msg = "";
                    for (int z = 0; z < unique_z_values.Count; z++)
                    {
                        //msg+=$"({x}/{z}), ({unique_x_values[x]}, {unique_z_values[z]}) / ";
                        msg += $"({x}/{z}) / ";
                    }
                    Debug.Log(msg);
                }
            }
        }
    }

    public static List<float> GetCoordinateUniqueValues(char coordinate, List<Transform> transforms)
    {
        List<float> unique_coordinate_values;
        // Convert the character to uppercase
        coordinate = char.ToUpper(coordinate);

        switch (coordinate)
        {
            case 'X':
                // Extract unique x values in ascending order
                unique_coordinate_values = transforms
                    .Select(obj => obj.transform.position.x)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                break;
            case 'Y':
                // Extract unique x values in ascending order
                unique_coordinate_values = transforms
                    .Select(obj => obj.transform.position.y)
                    .Distinct()
                    .OrderBy(y => y)
                    .ToList();
                break;
            case 'Z':
                // Extract unique x values in ascending order
                unique_coordinate_values = transforms
                    .Select(obj => obj.transform.position.z)
                    .Distinct()
                    .OrderBy(z => z)
                    .ToList();
                break;

            default:
                unique_coordinate_values = null;
                Debug.LogWarning("Coordinate invalid: " + coordinate);
                break;
        }
        return unique_coordinate_values;
    }

    public static void ShowUniqueValues(List<float> unique_values, char coordinate)
    {
        string values_string = string.Join(", ", unique_values);
        Debug.Log($"{char.ToUpper(coordinate)} values: {values_string}");
    }

    public static void SetColorAlpha(Image image, float alpha)
    {
        Color new_color = image.color;
        new_color.a = alpha;
        image.color = new_color;
    }

    public static void SetColorAlpha(Image image, Color new_color, float alpha)
    {
        if (new_color == null)
        {
            new_color = image.color;
        }
        new_color.a = alpha;
        image.color = new_color;
    }

    public static (int, int) GetWidthAndHeight(GameObject[,] board_array)
    {
        return (board_array.GetLength(1), board_array.GetLength(0));
    }

    public static int GetWidth(GameObject[,] board_array)
    {
        return board_array.GetLength(1);
    }

    public static int GetHeight(GameObject[,] board_array)
    {
        return board_array.GetLength(0);
    }
}
