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
    BoardScript_V2 main_script;
    public Image image_child,
        image_highlight;
    private SelectionTile sel_tile_script;
    bool walkable = false;

    private (int,int) grid_position;

    public void Start()
    {
        if (image_child == null)
        {
            image_child = gameObject.transform.Find("Image").GetComponent<Image>();
        }
        if (image_highlight == null)
        {
            image_highlight = gameObject.transform.Find("Highlight").GetComponent<Image>();
        }

        SetWalkableState(false);
        main_script = GameObject
            .FindWithTag(Constants.game_controller_object_tag)
            .GetComponent<BoardScript_V2>();
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
        BoardLibrary.SetColorAlpha(image_child, 0.75f);
        //Debug.Log("Mouse entered the tile");
    }

    // This will be called when the mouse pointer exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        BoardLibrary.SetColorAlpha(image_child, 1f);
    }

    // This will be called when the tile is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        // Perform any action on click
        BoardLibrary.SetColorAlpha(image_child, 1f);
        sel_tile_script.SetParent(gameObject);
        // Debug.Log($"Clicked:{gameObject.name}" );
    }

    public void SetWalkableState(bool state)
    {
        Color new_color = image_highlight.color;
        if (state)
        {
            new_color.a = 1f;
        }
        else
        {
            new_color.a = 0f;
        }
        walkable=state;
        image_highlight.color = new_color;
    }
}
