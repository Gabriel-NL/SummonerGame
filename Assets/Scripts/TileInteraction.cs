using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class TileInteraction
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
{
    public Image image_child,
        image_highlight;
    private SelectionTile sel_tile_script;
    private bool walkable = false;

    private (int,int) grid_position;

    public void Start()
    {
        
        SetWalkableState(false);
        sel_tile_script = GameObject
            .FindWithTag(Constants.selection_object_tag)
            .GetComponent<SelectionTile>();
    }
    public (int,int) GetGridPosition(){
        return grid_position;
    }
    public void SetGridPosition(int x,int y){
        grid_position = new (x,y);
    }

    // This will be called when the mouse pointer enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeBasedOnState(true);
        //Debug.Log("Mouse entered the tile");
    }

    // This will be called when the mouse pointer exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeBasedOnState(false);
    }

    // This will be called when the tile is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        // Perform any action on click
        BoardLibrary.SetColorAlpha(image_child, 1f);
        // Debug.Log($"Clicked:{gameObject.name}" );
        ChangeBasedOnState(true);
    }

    public void ChangeBasedOnState(bool hovered){
        
        if (walkable&&hovered)
        {
           BoardLibrary.SetColorAlpha(image_highlight, 1f);
           BoardLibrary.SetColorAlpha(image_child, 0.8f);
        }
        if (walkable&&hovered==false)
        {
            BoardLibrary.SetColorAlpha(image_highlight, 1f);
            BoardLibrary.SetColorAlpha(image_child, 0.9f);
        }
        if (walkable==false&&hovered)
        {
            BoardLibrary.SetColorAlpha(image_child, 0.75f);
        }
        if (walkable==false&&hovered==false)
        {
            BoardLibrary.SetColorAlpha(image_child, 1f); 
        }
        

    }
    public void SetWalkableState(bool state)
    {
        walkable=state;
        ChangeBasedOnState(false);
    }
    public bool GetWalkableState(){
        return walkable;
    }
}
