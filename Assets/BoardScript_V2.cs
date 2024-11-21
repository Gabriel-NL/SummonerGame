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

    [SerializeField]
    private GameObject tile_prefab,
        selected_tile_prefab;

    private static GameObject selected_tile_instance;

    private static (int, int) board_grid_width_and_height;
    private static (float, float) board_rect_width_and_height;

    public int current_selection_coord_x,
        current_selection_coord_y;
    public (int, int) current_selection_coord;

    GameObject[,] board_map_layer,
        board_entity_layer,
        board_player_layer;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform board_rect = board_obj.GetComponent<RectTransform>();
        board_rect_width_and_height = (board_rect.rect.width, board_rect.rect.height);

        //GameObject[,]board = BoardLibrary.CreateNewBoard(9,9,tile_prefab,board_obj);
        List<Transform> child_list = BoardLibrary.CollectChildren(board_obj.transform);

        board_map_layer = BoardLibrary.ListTo2dGrid(child_list, true);
        board_entity_layer = BoardLibrary.InitializeNewLayer(board_map_layer);
        board_player_layer = BoardLibrary.InitializeNewLayer(board_map_layer);

        board_grid_width_and_height = BoardLibrary.GetWidthAndHeight(board_map_layer);
    }

    public void SetSelectorTilePos(GameObject target_obj)
    {
        Transform target_transform = target_obj.transform;
        if (selected_tile_instance == null)
        {
            selected_tile_instance = Instantiate(selected_tile_prefab, board_obj.transform);
            RectTransform instance_transform = selected_tile_instance.GetComponent<RectTransform>();
            instance_transform.localScale = Vector3.one;
            selected_tile_instance.transform.localPosition = target_transform.localPosition;

            SetCurrentPosSel(instance_transform.localPosition);
            return;
        }

        if (target_transform.localPosition == selected_tile_instance.transform.localPosition)
        {
            Destroy(selected_tile_instance);
            return;
        }

        GameObject player_obj = BoardLibrary.FindChildWithTag(target_obj.transform, "Entity");
        if (player_obj != null)
        {
            MoveObject(player_obj, target_obj.transform);
        }
        else
        {
            selected_tile_instance.transform.localPosition = target_transform.localPosition;

            RectTransform instance_transform = selected_tile_instance.GetComponent<RectTransform>();
            SetCurrentPosSel(instance_transform.localPosition);
        }
    }

    private void SetCurrentPosSel(Vector3 position)
    {
        (int, int) normalized_current_pos = BoardLibrary.NormalizeStepValue(
            (position.x, position.y),
            board_rect_width_and_height,
            board_grid_width_and_height
        );

        current_selection_coord = normalized_current_pos;
        (current_selection_coord_x, current_selection_coord_y) = current_selection_coord;
    }

    private void MoveObject(GameObject entity, Transform target)
    {
        RectTransform board_rect = board_obj.GetComponent<RectTransform>();
        float size_x = board_rect.rect.width;
        float size_y = board_rect.rect.height;

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

        List<(int, int)> path = AIScanner
            .FindPossiblePaths(
                origin_x_y_normalized,
                target_x_y_normalized,
                board_grid_width_and_height
            )
            .ToList();

        StartCoroutine(MoveObjectCoroutine(entity.transform, path, board_grid_width_and_height));
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
        RectTransform rect_transform = board_obj.GetComponent<RectTransform>();

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
}
