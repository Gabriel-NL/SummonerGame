using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileInteraction
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
{
    // This will be called when the mouse pointer enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered the tile");
    }

    // This will be called when the mouse pointer exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exited the tile");
    }

    // This will be called when the tile is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        // Perform any action on click
        Debug.Log("Tile clicked");

        Debug.Log($"Obj: {gameObject.name}");
        GameObject sel_obj = GameObject.FindWithTag("sel_tile");

        // Check if it's active by itself
        Debug.Log("Active Self: " + sel_obj.activeSelf);

        // Check if it's active in the scene (considering parents)
        Debug.Log("Active in Hierarchy: " + sel_obj.activeInHierarchy);

        if (sel_obj.activeSelf == false)
        {
            sel_obj.SetActive(true);
            sel_obj.transform.parent = gameObject.transform;
            sel_obj.transform.localScale = Vector3.one; // Use Vector3.one for (1, 1, 1)
            sel_obj.transform.localPosition = gameObject.transform.localPosition;
            return;
        }
        else
        {
            if (sel_obj.transform.parent == gameObject.transform)
            {
                sel_obj.SetActive(false);
            }
            else
            {
                sel_obj.transform.parent = gameObject.transform;
                sel_obj.transform.localScale = Vector3.one; // Use Vector3.one for (1, 1, 1)
                sel_obj.transform.localPosition = gameObject.transform.localPosition;
            }
        }

       
    }
}
