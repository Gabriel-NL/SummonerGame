using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public const string player_1_tag = "Player_1_tag";
    public const string player_2_tag = "Player_2_tag";

    public const string selection_object_tag = "Sel_tile";
    public const string game_controller_object_tag = "GameController";
    public const bool enable_debug = true;

    public static readonly Dictionary<string, EntityClass> enemyDictionary = new Dictionary<string, EntityClass>()
    {
        {
            "Zombie",
            new EntityClass()
            {
                Strength = 1 + 1,
                Constitution = 1 + 1 + 1,
                Defense = 1 + 1 + 1,
                
            }
        },
        {
            "Skeleton Samurai",
            new EntityClass()
            {
                Strength = 1 + 1,
                Accuracy = 1 + 1,
                Dextery = 1 + 1,
                Constitution = 1 + 1
            }
        },
        {
            "Wendigo",
            new EntityClass()
            {
                Strength = 1 + 1 + 1 + 1,
                Dextery = 1 + 1,
                Constitution = 1 + 1,
                Image=Prefabs.Instance.images[1]
            }
        },
        {
            "Necromancer",
            new EntityClass()
            {
                Range=3,
                Movement=1,
                Image=Prefabs.Instance.images[0]
            }
        },
    };
}


