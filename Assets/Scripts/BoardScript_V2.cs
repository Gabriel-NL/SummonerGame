using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardScript_V2 : MonoBehaviour
{
    [SerializeField]
    private GameObject board_obj;

    [SerializeField]
    private EventSystem event_system;

    private SelectionTile sel_tile_script;

    public int current_selection_coord_x,
        current_selection_coord_y;
    private UnityBoardClass board_instance;

    GameObject selected_entity = null;

    public GameObject tile_prefab;

    public bool debug_boolean;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform board_rect = board_obj.GetComponent<RectTransform>();
        (float, float) board_rect_width_and_height = (
            board_rect.rect.width,
            board_rect.rect.height
        );

        RectTransform rect_tile = tile_prefab.GetComponent<RectTransform>();
        (float, float) tile_width_height = (rect_tile.rect.width, rect_tile.rect.height);

        Transform[] child_array = BoardLibrary.CollectChildren(board_obj.transform);
        board_instance=UnityBoardClass.Instance;
        board_instance.UseExistingBoard(
            child_array,
            tile_width_height,
            board_rect_width_and_height
        );
        GameObject sel_tile = GameObject.FindWithTag(Constants.selection_object_tag);
        sel_tile_script = sel_tile.GetComponent<SelectionTile>();

        //public UnityBoardClass(Transform[] all_childs, (float,float) tile_width_height,(float,float) parent_width_height)
    }
     public void UpdateSelTilePosition(){
        selected_entity=null;
     }

    public void UpdateSelTilePosition(GameObject target_obj)
    {
        Transform target_transform = target_obj.transform;
        board_instance.SetPositionOnGrid(target_transform.localPosition);

        

        if (selected_entity != null)
        {
            SeeLogs("Moving object");
            int radius = 1;
            (int, int)[] bumps = board_instance.GetBoardLimits();
            var dictionary = AIScanner.ScanForWalkable(
                board_instance.GetCoordinates(),
                radius,
                bumps,
                null,
                null
            );

            MoveObject(selected_entity, target_obj.transform);
            selected_entity = null;
            return;
        }

        GameObject entity_obj = board_instance.GetObjectOnEntityLayer();
        if (entity_obj == null)
        {
            sel_tile_script.SetVisibility(true);
            SeeLogs("No entity object here");
            return;
        }

        EntityInteraction entity = entity_obj.GetComponent<EntityInteraction>();
        if (entity == null)
        {
            sel_tile_script.SetVisibility(true);
            SeeLogs("No component EntityInteraction");
            return;
        }

        if (entity.GetPermission() == false)
        {
            sel_tile_script.SetVisibility(true);
            SeeLogs("No allowed to interact");
            return;
        }

        if (selected_entity == null)
        {
            sel_tile_script.SetVisibility(true);
            selected_entity = entity_obj;
            SeeLogs("Entity found and selected");
            return;
        }
    }

    private void ShowMovementOptions() { }

    private void MoveObject(GameObject entity, Transform target)
    {
        Vector3 origin_pos = entity.transform.localPosition;
        Vector3 target_pos = target.transform.localPosition;

        SeeLogs($"[Original] origin pos:{origin_pos} target pos:{target_pos}");

        (int, int) origin_x_y_normalized = board_instance.GetNormalizedPos(
            (origin_pos.x, origin_pos.y)
        );


        (int, int) target_x_y_normalized = board_instance.GetNormalizedPos(
            (target_pos.x, target_pos.y)
        );

        SeeLogs($"[Normalized] Origin:{origin_x_y_normalized} Target:{target_x_y_normalized}");

        (int, int)[] path = AIScanner.FindFastestPath(
            origin_x_y_normalized,
            target_x_y_normalized,
            board_instance.GetBoardLimits(),
            null,
            null
        );

        StartCoroutine(
            MoveObjectCoroutine(entity.transform, path, board_instance.GetGridWidthHeight())
        );
        int old_x,
            old_y;
        (old_x, old_y) = origin_x_y_normalized;
        int new_x,
            new_y;
        (new_x, new_y) = target_x_y_normalized;

        board_instance.MoveEntity((old_x, old_y),(new_x, new_y));
    }

    private IEnumerator MoveObjectCoroutine(
        Transform entity,
        (int, int)[] path,
        (int, int) grid_width_and_height
    )
    {
        //SeeLogs("Start Moving...");

        Vector3 start_pos = entity.localPosition;
        Vector3 target_next_pos;
        float journeyLength;
        float speed = 100f;
        float startTime;
        int path_x,path_y;
        (float, float) de_normalized_target;
        float distanceCovered;
        float fractionOfJourney;

        for (int i = 1; i < path.Length; i++)
        {
            
             path_x = path[i].Item1;
             path_y = path[i].Item2;

            de_normalized_target = board_instance.GetDenormalizedPos((path_x, path_y));

            target_next_pos = new Vector3(
                de_normalized_target.Item1,
                de_normalized_target.Item2,
                0
            );

            journeyLength = Vector3.Distance(start_pos, target_next_pos);
            startTime = Time.time;
            SeeLogs($"Next pos: {target_next_pos}");

            // Continue moving until the object reaches the target position
            while (entity.localPosition != target_next_pos)
            {
                
                distanceCovered = (Time.time - startTime) * speed;
                fractionOfJourney = distanceCovered / journeyLength;

                entity.localPosition = Vector3.Lerp(start_pos, target_next_pos, fractionOfJourney);

                yield return null; // Wait for the next frame
            }
            start_pos = target_next_pos;
        }
        event_system.enabled = true;

        //SeeLogs("End coroutine");
    }

    public void AddToEntityTable((int, int) grid_coord, GameObject entity)
    {
        entity.transform.SetParent(board_obj.transform);
        entity.transform.localScale = Vector3.one;

        Vector3 new_pos = board_instance
            .GetObjectOnMapLayer(grid_coord.Item1, grid_coord.Item2)
            .transform.localPosition;
        entity.transform.localPosition = new_pos;
        board_instance.CreateValueEntityTable(grid_coord, entity);
    }

    public GameObject getBoardObj()
    {
        return board_obj;
    }

    public void SeeLogs( string message){
        if (debug_boolean)
        {
            Debug.Log(message);
        }
    }
}
