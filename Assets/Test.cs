using System;
using System.Collections.Generic;
using System.Linq;

public class HelloWorld
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Try programiz.pro");

        (int, int) origin = (0, 0);
        (int, int) target = (0, 5);

        (int, int)[] bumps = new (int, int)[] { }; // Example bumps array
        int step_count = 4;

        (int, int)[] path = FindPossiblePaths(origin, target, bumps, step_count,15);
        if (path != null)
        {
            foreach (var item in path)
            {
                Console.WriteLine(item);
            }
        }
    }
    

    public static (int, int)[] FindPossiblePaths(
        (int, int) origin,
        (int, int) target,
        (int, int)[] bump_list,
        int? max_step_count,
        int? max_attempt
    )
    {
        List<(int, int)> possible_path = new List<(int, int)>();
        (int, int) current_pos = new(origin.Item1, origin.Item2);
        possible_path.Add(current_pos);
        Dictionary<(int, int), (int, int)[]> complex_directions_dict =
            new Dictionary<(int, int), (int, int)[]>();
        List<(int, int)> blocked_directions = new List<(int, int)>();
        int iteration = 0;
        while (current_pos != target)
        {
            if (max_attempt.HasValue && iteration > max_attempt.Value)
{
    Console.WriteLine("Can't attempt any more. Path failed.");
    return null;
}

            Console.WriteLine($"------------------------------");
            Console.WriteLine($"Iteration:{iteration}");
            iteration++;
            string path = string.Join(
                ", ",
                possible_path.Select(direction => $"({direction.Item1}, {direction.Item2})")
            );
            Console.WriteLine("path: " + path);
            (int, int) direction;
            if (complex_directions_dict.ContainsKey(current_pos))
            {
                direction = GetNextStepDirection(
                    current_pos,
                    target,
                    complex_directions_dict[current_pos]
                );
            }
            else
            {
                direction = GetNextStepDirection(current_pos, target, blocked_directions.ToArray());
            }
            //Console.WriteLine($"pos:{current_pos} direction: {direction}");
            if (
                direction == (0, 0)
                || (max_step_count.HasValue && possible_path.Count > max_step_count.Value)
            )
            {
                Console.WriteLine($"Max limit hit or direction 0");
                if (complex_directions_dict.ContainsKey(current_pos))
                {
                    // Get the existing list of blocked directions
                    var existingBlockedDirections = complex_directions_dict[current_pos];

                    // Add the new blocked directions without duplicates
                    var combinedBlockedDirections = existingBlockedDirections
                        .Concat(blocked_directions) // Concatenate the existing and new blocked directions
                        .Distinct() // Remove duplicates
                        .ToArray(); // Convert back to an array (if needed)

                    // Update the dictionary with the combined list
                    complex_directions_dict[current_pos] = combinedBlockedDirections;
                }
                else
                {
                    // Add the blocked directions as the value for the current_pos key
                    complex_directions_dict.Add(current_pos, blocked_directions.ToArray());
                }

                blocked_directions.Clear();
                if (possible_path.Count < 2)
                {
                    Console.WriteLine("Impossible to reach the current path...");
                    return null;
                }
                
                (int, int) position_before = possible_path[possible_path.Count - 2];

                (int, int) dead_end_direction = (
                    current_pos.Item1 - position_before.Item1,
                    current_pos.Item2 - position_before.Item2
                );
                blocked_directions.Add(dead_end_direction);

                if (complex_directions_dict.ContainsKey(position_before))
                {
                    // Get the existing list of blocked directions
                    var existingBlockedDirections = complex_directions_dict[position_before];

                    // Add the new blocked directions without duplicates
                    var combinedBlockedDirections = existingBlockedDirections
                        .Concat(blocked_directions) // Concatenate the existing and new blocked directions
                        .Distinct() // Remove duplicates
                        .ToArray(); // Convert back to an array (if needed)

                    // Update the dictionary with the combined list
                    complex_directions_dict[position_before] = combinedBlockedDirections;
                }
                else
                {
                    // Add the blocked directions as the value for the current_pos key
                    complex_directions_dict.Add(position_before, blocked_directions.ToArray());
                }

                //Console.WriteLine($"Current pos: {position_before} \\ Blocked directions: {string.Join(", ", blocked_directions)}");
                blocked_directions.Clear();
                possible_path.Remove(current_pos);
                current_pos = position_before;
                Console.WriteLine("Backtracing...");

                continue;
            }
            (int, int) new_position = (
                current_pos.Item1 + direction.Item1,
                current_pos.Item2 + direction.Item2
            );

            if (bump_list.Contains(new_position) || possible_path.Contains(new_position))
            {
                //Debug.Log( $"Cant go into {new_position}! It is either a bump or already crossed that" );
                //Debug.Log($"List size: {possible_path.Count}");
                blocked_directions.Add(direction);
                //Debug.Log($"Block List size {blocked_directions.Count}");
                Console.WriteLine("Bumped");

                if (complex_directions_dict.ContainsKey(current_pos))
                {
                    // Get the existing list of blocked directions
                    var existingBlockedDirections = complex_directions_dict[current_pos];

                    // Add the new blocked directions without duplicates
                    var combinedBlockedDirections = existingBlockedDirections
                        .Concat(blocked_directions) // Concatenate the existing and new blocked directions
                        .Distinct() // Remove duplicates
                        .ToArray(); // Convert back to an array (if needed)

                    // Update the dictionary with the combined list
                    complex_directions_dict[current_pos] = combinedBlockedDirections;
                }
                else
                {
                    // Add the blocked directions as the value for the current_pos key
                    complex_directions_dict.Add(current_pos, blocked_directions.ToArray());
                }
                blocked_directions.Clear();
                continue;
            }

            //Se o passo eh 3+1, e a lista for ter +1, bloqueia
            /*
            if (possible_path.Count > max_step_count)
            {
                blocked_directions.Add(direction);
                continue;
            }
            */

            possible_path.Add(new_position);
            current_pos = new_position;
            blocked_directions.Clear();
            Console.WriteLine("Added position!");
        }

        return possible_path.ToArray();
        ;
    }

    
    
    private static (int, int) GetNextStepDirection(
        (int, int) current_pos,
        (int, int) target_pos,
        (int, int)[] blocked_directions // Now using an array
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

        // Sort directions based on Manhattan distance, and prioritize right/left moves
        // Sort directions based on Manhattan distance to the target
        List<(int, int)> best_directions_order = all_possible_directions
            .OrderBy(d => GetManhattanDistance(current_pos, d, target_pos))
            .ToList();
        (int, int) previous_direction = (
            -best_directions_order[0].Item1,
            -best_directions_order[0].Item2
        );
        best_directions_order.Remove(previous_direction);

        if (blocked_directions.Length > 0)
        {
            string listAsString = string.Join(
                ", ",
                best_directions_order.Select(direction => $"({direction.Item1}, {direction.Item2})")
            );
            Console.WriteLine("List: " + listAsString);
            string blocked = string.Join(
                ", ",
                blocked_directions.Select(direction => $"({direction.Item1}, {direction.Item2})")
            );
            Console.WriteLine("blocked: " + blocked);
        }

        // Iterate through the best directions
        foreach (var direction in best_directions_order)
        {
            if (blocked_directions.Contains(direction))
            {
                continue; // Skip blocked directions
            }

            // If the direction is not blocked, return it
            return direction;
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
        return System.Math.Abs(target_pos.Item1 - new_x)
            + System.Math.Abs(target_pos.Item2 - new_y);
    }
}
//to run use:    dotnet run
