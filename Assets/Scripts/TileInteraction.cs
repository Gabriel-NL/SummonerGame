using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileInteraction
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
{
    BoardScript main_script;
    bool visible=false;

    // This will be called when the mouse pointer enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        BoardLibrary.SetColorAlpha(gameObject.GetComponent<Image>(),0.5f);
        //Debug.Log("Mouse entered the tile");
    }

    // This will be called when the mouse pointer exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        BoardLibrary.SetColorAlpha(gameObject.GetComponent<Image>(),1f);
    }

    // This will be called when the tile is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        // Perform any action on click
        BoardLibrary.SetColorAlpha(gameObject.GetComponent<Image>(),1f);

        Debug.Log($"Obj: {gameObject.name}");
        main_script = GameObject.FindWithTag("GameController").GetComponent<BoardScript>();
        main_script.SetSelTilePos(gameObject.transform.localPosition);
      
    }
}
