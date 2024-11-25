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
    public Image image_child;
    private SelectionTile sel_tile_script;
    bool selected = false;

    public void Awake()
    {
        main_script = GameObject.FindWithTag("GameController").GetComponent<BoardScript_V2>();
        sel_tile_script= GameObject.FindWithTag("Sel_tile").GetComponent<SelectionTile>();

        if (image_child != null)
        {
            Transform image_child_transform = gameObject.transform.Find("Image");
            image_child = image_child_transform.GetComponent<Image>();
        }
    }
    public void Start(){

    }

    // This will be called when the mouse pointer enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        BoardLibrary.SetColorAlpha(image_child, 0.7f);
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
        

    }
}
