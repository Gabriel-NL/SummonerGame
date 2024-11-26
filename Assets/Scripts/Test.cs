using System;
using System.Collections.Generic;

public class HelloWorld
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Try programiz.pro");

        (int, int) origin = (3, 2);
        int radius = 3;
        (int, int)[] bumps = new (int, int)[] { }; // Example bumps array

        ScanWalkable(origin, radius, bumps);
    }


    public static (int, int)[] ScanWalkable((int, int) origin, int radius, (int, int)[] bumps)
    {
        List<(int, int)> possible_directions = new List<(int, int)>();
        int i = 0;
        while (i < radius)
        {
            i+=1;
            (int, int) currentScanPos = (i, 0);
             Console.WriteLine(currentScanPos);

            for (int j = 0; j < i+1; j++)
            {
                int x = currentScanPos.Item1;
                int y = currentScanPos.Item2;

                (int, int) direction;
                direction = (x, y);
                possible_directions.Add(direction);
                if (y != 0)
                {
                    direction = (x, -y);
                    possible_directions.Add(direction);
                }
                if (x != 0)
                {
                    direction = (-x, y);
                    possible_directions.Add(direction);
                }
                if (x != 0 && y != 0)
                {
                    direction = (-x, -y);
                    possible_directions.Add(direction);
                }
                if (currentScanPos.Item1 > 0)
                {
                    currentScanPos.Item1 -= 1;
                    currentScanPos.Item2 += 1;
                }
            }

        }
        return possible_directions.ToArray();
    }


}

