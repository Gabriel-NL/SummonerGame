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

    private static (int, int) board_grid_width_and_height;
    private static (float, float) board_rect_width_and_height;

    public int current_selection_coord_x,
        current_selection_coord_y;
    public (int, int) current_selection_coord;

    GameObject[,] board_map_layer,
        board_entity_layer,
        board_player_layer;
    
    GameObject selected_entity=null;



    

    // Start is called before the first frame update
    void Start()
    {
        GameObject sel_tile = GameObject.FindWithTag(Constants.selection_object_tag);
        sel_tile_script = sel_tile.GetComponent<SelectionTile>();

        RectTransform board_rect = board_obj.GetComponent<RectTransform>();
        board_rect_width_and_height = (board_rect.rect.width, board_rect.rect.height);

        //board_map_layer = BoardLibrary.CreateNewBoard(9,9,tile_prefab,board_obj);
        List<Transform> child_list = BoardLibrary.CollectChildren(board_obj.transform);

        board_map_layer = BoardLibrary.ListTo2dGrid(child_list, true);
        board_entity_layer = BoardLibrary.InitializeNewLayer(board_map_layer);
        board_player_layer = BoardLibrary.InitializeNewLayer(board_map_layer);

        board_grid_width_and_height = BoardLibrary.GetWidthAndHeight(board_map_layer);
    }

    public void UpdateSelTilePosition(GameObject target_obj)
    {
        Transform target_transform = target_obj.transform;
        SetPositionOnGrid(target_transform.localPosition);

        GameObject entity_obj = board_entity_layer[
            current_selection_coord.Item1,
            current_selection_coord.Item2
        ];
        if (selected_entity!=null){
            Debug.Log("Moving object");
            MoveObject(selected_entity, target_obj.transform);
            selected_entity=null;
        }
        
        if (entity_obj == null)
        {
            sel_tile_script.SetVisibility(true);
            Debug.Log("No entity object here");
            return;
        }

         EntityInteraction entity = entity_obj.GetComponent<EntityInteraction>();
        if (entity == null)
        {
            sel_tile_script.SetVisibility(true);
            Debug.Log("No component EntityInteraction");
            return;
        }

        if (entity.GetPermission() == false)
        {
            sel_tile_script.SetVisibility(true);
            Debug.Log("No allowed to interact");
            return;
        }

        if (selected_entity==null)
        {
            sel_tile_script.SetVisibility(true);
            selected_entity=entity_obj;
            return;
        }

        
    }

    private void SetPositionOnGrid(Vector3 position)
    {
        (int, int) normalized_current_pos = BoardLibrary.NormalizeStepValue(
            (position.x, position.y),
            board_rect_width_and_height,
            board_grid_width_and_height
        );
        Debug.Log("CoordPos:" + normalized_current_pos);
        current_selection_coord = normalized_current_pos;
        (current_selection_coord_x, current_selection_coord_y) = current_selection_coord;
    }

    private void MoveObject(GameObject entity, Transform target)
    {

        Vector3 origin_pos = entity.transform.localPosition;

        Vector3 target_pos = target.transform.localPosition;

        (int, int) origin_x_y_normalized = BoardLibrary.NormalizeStepValue(
            (origin_pos.x, origin_pos.y),
            board_rect_width_and_height,
            board_grid_width_and_height
        );
        (int, int) target_x_y_normalized = BoardLibrary.NormalizeStepValue(
            (target_pos.x, target_pos.y),
            board_rect_width_and_height,
            board_grid_width_and_height
        );
        Debug.Log($"Origin:{origin_x_y_normalized} Target:{target_x_y_normalized}");

        List<(int, int)> path = AIScanner
            .FindPossiblePaths(
                origin_x_y_normalized,
                target_x_y_normalized,
                board_grid_width_and_height
            )
            .ToList();

        StartCoroutine(MoveObjectCoroutine(entity.transform, path, board_grid_width_and_height));
        int old_x,
            old_y;
        (old_x, old_y) = origin_x_y_normalized;
        int new_x,
            new_y;
        (new_x, new_y) = target_x_y_normalized;
        board_entity_layer[new_x, new_y] = board_entity_layer[old_x, old_y];
        board_entity_layer[old_x, old_y] = null;
    }

    private IEnumerator MoveObjectCoroutine(
        Transform entity,
        List<(int, int)> path,
        (int, int) grid_width_and_height
    )
    {
        Debug.Log("Start Moving...");

        Vector3 start_pos = entity.localPosition;
        Vector3 target_next_pos;
        float journeyLength;
        float speed = 100f;
        float startTime;

        for (int i = 1; i < path.Count; i++)
        {
            (float, float) de_normalized_target;
            int path_x = path[i].Item1;
            int path_y = path[i].Item2;

            de_normalized_target = BoardLibrary.RevertNormalization(
                (path_x, path_y),
                board_rect_width_and_height,
                board_grid_width_and_height
            );

            target_next_pos = new Vector3(
                de_normalized_target.Item1,
                de_normalized_target.Item2,
                0
            );

            journeyLength = Vector3.Distance(start_pos, target_next_pos);
            startTime = Time.time;
            //Debug.Log($"Next pos: {target_next_pos}");

            // Continue moving until the object reaches the target position
            while (entity.localPosition != target_next_pos)
            {
                float distanceCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = distanceCovered / journeyLength;

                entity.localPosition = Vector3.Lerp(start_pos, target_next_pos, fractionOfJourney);

                yield return null; // Wait for the next frame
            }
            start_pos = target_next_pos;
        }
        event_system.enabled = true;

        Debug.Log("End coroutine");
    }

    public void AddToEntityTable((int, int) grid_coord, GameObject entity)
    {
        entity.transform.SetParent(board_obj.transform);
        entity.transform.localPosition = board_map_layer[grid_coord.Item1, grid_coord.Item2]
            .transform
            .localPosition;
        entity.transform.localScale = Vector3.one;
        board_entity_layer[grid_coord.Item1, grid_coord.Item2] = entity;
    }

    public GameObject getBoardObj(){
        return board_obj;
    }

    
}
