using System.Collections;

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceInteraction : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
{
    private int dice_value;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }



    public void SetValue(int value){
        dice_value = value;
        gameObject.transform.Find("Dice_value").GetComponent<TextMeshProUGUI>().text=$"{value}";

    }
    public int GetValue(){
        return dice_value;
    }

    


}
