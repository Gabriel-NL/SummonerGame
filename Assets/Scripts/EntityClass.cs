using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityClass{

    public float HealthPoints=0 ;
    public int Strength =0 ;
    public int Accuracy =0 ;
    public int Dextery=0 ;
    public int Constitution=0 ;
    public int Defense=0 ;
    public float Damage =0 ;
    public int Range =0 ;
    public int Movement =0 ;
    public Sprite Image{ get; set; }

    // Constructor to initialize all fields
    public EntityClass(
        int strength=0,
        int accuracy=0,
        int dextery=0,
        int constitution=0,
        int defense=0,
        int range=0
    )
    {

        Strength = strength;
        Accuracy =accuracy;
        Dextery = dextery;
        Constitution =constitution;
        Defense = defense;
        Range = 1 + range;

        HealthPoints = 2 + 2 * Constitution;
        Damage = 1 + Strength;
        Movement = 2 + Dextery;
    }

    
}