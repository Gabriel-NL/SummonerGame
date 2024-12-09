using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionTile : MonoBehaviour
{
    
    private bool visible;

    // Start is called before the first frame update
    void Start()
    {
        SetVisibility(true);
    }

    public GameObject GetCurrentParent()
    {
        return gameObject.transform.parent.gameObject;
    }

    public void SetColor(Color color)
    {
        gameObject.GetComponent<Image>().color=color;
    }

    public Vector3 GetLocalPosition()
    {
        return gameObject.transform.parent.transform.localPosition;
        ;
    }

    public void SetVisibility(bool state)
    {
        Image image = gameObject.GetComponent<Image>();
        Color new_color = image.color;
        if (state)
        {
            new_color.a = 0.7f;
        }
        else
        {
            new_color.a = 0;
        }
        image.color = new_color;
        visible = state;
    }

    public void ToggleVisibility()
    {
        Image image = gameObject.GetComponent<Image>();
        Color new_color = image.color;
        if (visible)
        {
            new_color.a = 0;
            visible = false;
        }
        else
        {
            new_color.a = 0.7f;
            visible = true;
        }
        image.color = new_color;
    }
}
