using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiceInteraction
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
{
    private DiceContainer dice_container;
    private int dice_value;

    [SerializeField]
    private Image img,
        highlight;

    [SerializeField]
    private TextMeshProUGUI text;
    private bool selected = false;

    void Start()
    {
        dice_container = GameObject
            .FindGameObjectWithTag(Constants.game_controller_object_tag)
            .GetComponent<DiceContainer>();
        SetInteractable(false);
        HighLightDice(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Dice clicked");
    }

    public void OnPointerEnter(PointerEventData eventData) { }

    public void OnPointerExit(PointerEventData eventData) { }

    public void SetValue(int value)
    {
        dice_value = value;
        gameObject.transform.Find("Dice_value").GetComponent<TextMeshProUGUI>().text = $"{value}";
    }

    public int GetValue()
    {
        return dice_value;
    }

    public void SetInteractable(bool state)
    {
        if (state)
        {
            img.raycastTarget = true;
        }
        else
        {
            img.raycastTarget = false;
        }
    }

    public void HighLightDice(bool state)
    {
        selected = state;
        if (state)
        {
            BoardLibrary.SetColorAlpha(highlight, 1);
        }
        else
        {
            BoardLibrary.SetColorAlpha(highlight, 0);
        }
    }
}
