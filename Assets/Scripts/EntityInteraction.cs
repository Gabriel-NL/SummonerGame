using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInteraction : MonoBehaviour
{
    public bool interactable;
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
}
