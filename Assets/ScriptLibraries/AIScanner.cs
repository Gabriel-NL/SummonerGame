using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIScanner : MonoBehaviour
{
    public static List<(int, int)> FindPath(
        (int, int) origin,
        (int, int) target,
        (int, int) array_size
    )
    {
        List<(int, int)> path = new List<(int, int)>();
        path.Add(origin);

        (int, int) current_pos = origin;
        (int, int) bump = (3, 3);
        (int, int)[] bumps = new (int, int)[] { (1, 1), (2, 2), (3, 3), (4, 4) };

        while (current_pos != target)
        {
            (int, int) direction = GetNextStepDirection(current_pos, target);
            //Debug.Log($"Direction: {direction}");
            current_pos.Item1 += direction.Item1;
            current_pos.Item2 += direction.Item2;

            path.Add(current_pos);

            // Check if the current position is the bump
            if (current_pos == bump)
            {
                Debug.Log("Hit a bump. Stopping path.");
                return path; // Stop and return the path if we hit the bump
            }
        }

        return path;
    }

    private static (int, int) GetNextStepDirection((int, int) current_pos, (int, int) target_pos)
    {
        (int, int) direction = (0, 0);

        int current_pos_x,
            current_pos_y;
        (current_pos_x, current_pos_y) = current_pos;

        int target_pos_x,
            target_pos_y;
        (target_pos_x, target_pos_y) = target_pos;

        // Determine direction on the X axis
        direction.Item1 = target_pos_x.CompareTo(current_pos_x); // Move right (+1), left (-1), or stay (0)
        direction.Item2 = target_pos_y.CompareTo(current_pos_y); // Move up (+1), down (-1), or stay (0)

        // Instead of making 2 steps, do one.
        if (direction.Item1 != 0 && direction.Item2 != 0)
        {
            // Generate a random number, 0 or 1 using Unity's Random
            int choice = UnityEngine.Random.Range(0, 2); // 0 or 1

            // Set one of the values to 0 based on the random choice
            if (choice == 0)
            {
                direction.Item1 = 0; // Set Item1 to 0
            }
            else
            {
                direction.Item2 = 0; // Set Item2 to 0
            }
        }

        return direction;
    }

    public static (int, int)[] FindPossiblePaths((int, int) origin, (int, int) target)
    {
        List<(int, int)> possible_path = new List<(int, int)>();
        (int, int)[] possible_path_array;
        (int, int)[] bumps = new (int, int)[] { (0, 2) };
        (int, int) current_pos = new(origin.Item1, origin.Item2);

        possible_path.Add(current_pos);
        Dictionary<(int, int), (int, int)[]> complex_directions_dict =
            new Dictionary<(int, int), (int, int)[]>();

        List<(int, int)> blocked_directions = new List<(int, int)>();
        while (current_pos != target)
        {
            (int, int) direction = GetNextStepDirection(current_pos, target, blocked_directions);
            //Debug.Log($"");
            //Debug.Log($"Current: {current_pos}");
            //Debug.Log($"Direction: {direction}");

            if (direction == (0, 0))
            {
                complex_directions_dict.Add(current_pos, blocked_directions.ToArray());
                blocked_directions.Clear();

                if (possible_path.Count < 2)
                {
                    possible_path_array = possible_path.ToArray();

                    return possible_path_array;
                }
                else
                {
                    (int, int) position_before = possible_path[possible_path.Count - 2];
                    (int, int) dead_end_direction = (
                        current_pos.Item1 - position_before.Item1,
                        current_pos.Item2 - position_before.Item2
                    );

                    blocked_directions.Add(dead_end_direction);
                    complex_directions_dict.Add(position_before, blocked_directions.ToArray());
                    blocked_directions.Clear();
                    possible_path.Remove(current_pos);
                    current_pos = position_before;
                }

                continue;
            }

            (int, int) new_position = (
                current_pos.Item1 + direction.Item1,
                current_pos.Item2 + direction.Item2
            );

            if (bumps.Contains(new_position) || possible_path.Contains(new_position))
            {
                //Debug.Log( $"Cant go into {new_position}! It is either a bump or already crossed that" );
                //Debug.Log($"List size: {possible_path.Count}");
                blocked_directions.Add(direction);
                //Debug.Log($"Block List size {blocked_directions.Count}");
            }
            else
            {
                // Convert the list items to string and join them with a delimiter

                // Print the list in a single line
                //Debug.Log($"New pos:{new_position}");
                possible_path.Add(new_position);
                string listAsString = string.Join(
                    ", ",
                    possible_path.Select(pos => $"({pos.Item1}, {pos.Item2})")
                );
                //Debug.Log($"Current list:{listAsString}");
                current_pos = new_position;
                blocked_directions.Clear();
            }
        }

        possible_path_array = possible_path.ToArray();

        return possible_path_array;
    }

    private static (int, int) GetNextStepDirection(
        (int, int) current_pos,
        (int, int) target_pos,
        List<(int, int)> blocked_directions
    )
    {
        // List to hold the best directions sorted by distance to target
        (int, int)[] all_possible_directions = new (int, int)[]
        {
            (0, 1), // Up
            (0, -1), // Down
            (1, 0), // Right
            (-1, 0) // Left
        };

        List<(int, int)> best_directions_order = all_possible_directions
            .OrderBy(d => GetManhattanDistance(current_pos, d, target_pos))
            .ToList();

        // Convert the list items to string and join them with a delimiter
        string listAsString = string.Join(
            ", ",
            best_directions_order.Select(direction => $"({direction.Item1}, {direction.Item2})")
        );

        // Print the list in a single line
        //Debug.Log($"Direction priorities: {listAsString}");

        // Remove the farthest direction (last in the list)
        best_directions_order.RemoveAt(best_directions_order.Count - 1);

        // Iterate through the best directions
        foreach (var direction in best_directions_order)
        {
            // If the direction is not blocked, return it
            if (!blocked_directions.Contains(direction))
            {
                return direction;
            }
        }

        // If all directions are blocked, return (0, 0)
        return (0, 0);
    }

    private static int GetManhattanDistance(
        (int, int) current_pos,
        (int, int) direction,
        (int, int) target_pos
    )
    {
        // Calculate the new position by applying the direction to the current position
        int new_x = current_pos.Item1 + direction.Item1;
        int new_y = current_pos.Item2 + direction.Item2;

        // Return the Manhattan distance from the new position to the target position
        return Math.Abs(target_pos.Item1 - new_x) + Math.Abs(target_pos.Item2 - new_y);
    }

    // Helper method to check if the direction is blocked


    public static void PrintPath(List<(int, int)> path)
    {
        Debug.Log("Path values:");
        string final_path = "";
        foreach (var pos in path)
        {
            final_path += $"({pos.Item1},{pos.Item2}) / ";
        }
        Debug.Log($"Path: {final_path}");
    }

    public static void PrintPath((int, int)[] path)
    {
        Debug.Log("Path values:");
        string final_path = "";
        foreach (var pos in path)
        {
            final_path += $"({pos.Item1},{pos.Item2}) / ";
        }
        Debug.Log($"Path: {final_path}");
    }
}
