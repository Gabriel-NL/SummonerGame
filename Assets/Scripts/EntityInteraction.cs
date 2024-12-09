using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInteraction : MonoBehaviour
{
    public bool interactable;
    private string name;
    private EntityClass entity;
    private float current_hp=0;
      public string GetPermissionType(){
        
        string obj_tag=gameObject.tag;
        return obj_tag;
    }
    public void SetPermission(bool boolean){
        interactable = boolean;
    }
    public bool GetPermission(){
        return interactable;
    }

    public void InitializeAttributes( string entity_name){
        bool has_key=Constants.enemyDictionary.ContainsKey(entity_name);
        if (has_key)
        {
            name=entity_name;
            entity=Constants.enemyDictionary[entity_name];
            current_hp=entity.HealthPoints;
            gameObject.GetComponent<Image>().sprite = entity.Image;
        }else
        {
            Debug.LogError("This name is not valid:"+entity_name);
        }
        
        
    }
    public int GetRadius(){
        return entity.Movement;
    }
    public string GetTag(){
        return gameObject.tag;
    }
    public float GetDamage(){
        return entity.Damage;
    }
    public float GetHP(){
        return current_hp;
    }
    public void SetHP(float hp){
        current_hp=hp;
    }


}
