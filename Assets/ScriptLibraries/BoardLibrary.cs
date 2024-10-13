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

    private (int, int) GetWidthAndHeight(GameObject[,] board_array)
    {
        return (board_array.GetLength(1), board_array.GetLength(0));
    }

    private int GetWidth(GameObject[,] board_array)
    {
        return board_array.GetLength(1);
    }

    private int GetHeight(GameObject[,] board_array)
    {
        return board_array.GetLength(0);
    }
}
