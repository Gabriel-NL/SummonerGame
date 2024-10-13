using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardScript : MonoBehaviour
{
    [SerializeField]
    private GameObject board_obj;

    [SerializeField]
    private GameObject selected_tile_prefab;

   [SerializeField] private GameObject selected_tile_instance;

    private GameObject[,] board_array;

    private GameObject current_obj_selected = null;

    // Start is called before the first frame update
    void Start()
    {

        List<Transform> child_list = BoardLibrary.CollectChildren(board_obj.transform);
        board_array = BoardLibrary.ListTo2dGrid(child_list, true);

        
        (int, int) origin = (0, 0);
        (int, int) target = (4, 5);
         //List<(int, int)> path= AIScanner.FindPath(origin,target,GetWidthAndHeight());
         List<(int, int)> path= AIScanner.FindPossiblePaths(origin,target).ToList();
         AIScanner.PrintPath(path);
    }

    

    // Update is called once per frame
    void Update() { }

/*
    private void AddClickHandlers()
    {
        foreach (GameObject obj in board_array)
        {
            if (obj != null)
            {
                EventTrigger eventTrigger =
                    obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();
                // Add PointerClick event handler
                AddEventHandler(
                    eventTrigger,
                    EventTriggerType.PointerClick,
                    (data) => OnObjectClicked(obj)
                );
                AddEventHandler(
                    eventTrigger,
                    EventTriggerType.PointerEnter,
                    (data) => OnObjectHovered(obj)
                );
                // Example for PointerExit
                AddEventHandler(
                    eventTrigger,
                    EventTriggerType.PointerExit,
                    (data) => OnObjectExited(obj)
                );
            }
        }
    }

    private void AddEventHandler(
        EventTrigger eventTrigger,
        EventTriggerType eventType,
        UnityAction<BaseEventData> callback
    )
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(callback);
        eventTrigger.triggers.Add(entry);
    }

    public void OnObjectClicked(GameObject obj)
    {
        Debug.Log($"Obj: {obj.name}");

        if (current_obj_selected == null)
        {
            selected_tile_instance = Instantiate(
                selected_tile_prefab,board_obj.transform
            );
            selected_tile_instance.transform.localScale = Vector3.one; // Use Vector3.one for (1, 1, 1)
            selected_tile_instance.transform.localPosition=obj.transform.localPosition;
            current_obj_selected = obj;

        }
        if (current_obj_selected == obj)
        {
            Destroy(selected_tile_instance);
            current_obj_selected = null;
            return;
        }

        if (current_obj_selected != obj)
        {
            selected_tile_instance.transform.localPosition = current_obj_selected.transform.localPosition;
            current_obj_selected = obj;
            return;
        }
    }
*/

    private void ChangeColorAlpha(Image image, float alpha)
    {
        Color new_color = image.color;
        new_color.a = alpha;
        image.color = new_color;
    }

    private void ChangeColor(Image image, Color color)
    {
        image.color = color;
    }

    private void OnObjectHovered(GameObject obj)
    {
        ChangeColorAlpha(obj.GetComponent<Image>(), 0.5f);
    }

    public void OnObjectExited(GameObject obj)
    {
        ChangeColorAlpha(obj.GetComponent<Image>(), 1f);
    }

    public (int, int) GetWidthAndHeight()
    {
        return (board_array.GetLength(1), board_array.GetLength(0));
    }

    public int GetWidth()
    {
        return board_array.GetLength(1);
    }

    public int GetHeight()
    {
        return board_array.GetLength(0);
    }
}
