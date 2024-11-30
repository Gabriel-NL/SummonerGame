using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectionTile : MonoBehaviour
{
    private Image image;
    private bool visible;
    private BoardScript_V2 main_script;
    
    // Start is called before the first frame update
    void Start()
    {
        image=gameObject.GetComponent<Image>();
        SetVisibility(false);
        main_script = GameObject.FindWithTag(Constants.game_controller_object_tag).GetComponent<BoardScript_V2>();
        gameObject.transform.SetParent(main_script.getBoardObj().transform);
    }
    
    public GameObject GetCurrentParent(){
        return gameObject.transform.parent.gameObject;
    }
    public void SetParent(GameObject next_parent){
        GameObject current_parent=gameObject.transform.parent.gameObject;
        bool is_same_parent= next_parent==current_parent;
        if (is_same_parent)
        {
            ToggleVisibility();
        }else
        {
            SetVisibility(false);
            main_script.UpdateSelTilePosition(next_parent);
            gameObject.transform.SetParent(next_parent.transform);
            gameObject.transform.localPosition=Vector3.zero;
            gameObject.transform.localScale=Vector3.one;
        }
        //Debug.Log($"New parent: {next_parent.name}");
    }
    public Vector3 GetLocalPosition(){
        return gameObject.transform.parent.transform.localPosition;;
    }

    public void SetVisibility(bool state)
    { 
        Color new_color=image.color;
        if (state) {
            new_color.a = 0.7f;
         }
         else {
            new_color.a = 0;
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
