using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectionTile : MonoBehaviour
{
    private Image image;
    private GameObject parent;
    private bool visible;
    private BoardScript_V2 main_script;
    
    // Start is called before the first frame update
    void Start()
    {
        image=gameObject.GetComponent<Image>();
        main_script = GameObject.FindWithTag("GameController").GetComponent<BoardScript_V2>();
    }
    
    public GameObject GetCurrentParent(){
        return parent;
    }
    public void SetParent(GameObject next_parent){
        
        bool is_same_parent= next_parent==parent;
        if (is_same_parent)
        {
            ToggleVisibility();
        }else
        {
            SetVisibility(true);
            main_script.UpdateSelTilePosition(next_parent);
            gameObject.transform.localPosition=Vector3.zero;
            gameObject.transform.localScale=Vector3.one;
        }
        parent=next_parent;
    }
    public Vector3 GetLocalPosition(){
        return parent.transform.localPosition;;
    }
    public bool CheckParent(GameObject next_parent){
        
        bool verification= next_parent==parent;
        return verification;
    }

    public void SetVisibility(bool state)
    {
        
        Color new_color=image.color;
        if (state) {
            new_color.a = 0;
            
         }
         else {
            new_color.a = 0.7f;
         }
        image.color=new_color;
    }
    public void ToggleVisibility()
    {
        Color new_color=image.color;
        if (visible){
            new_color.a = 0;
            visible=!visible;
            
         }
         else {
            new_color.a = 0.7f;
            visible=!visible;
         }
        image.color=new_color;
    }
}
