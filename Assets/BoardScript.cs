using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardScript : MonoBehaviour
{
    [SerializeField]
    private GameObject board_obj;

    [SerializeField]
    private EventSystem event_system;

    [SerializeField]
    private GameObject selected_tile_prefab;

    private static GameObject selected_tile_instance;

    private static (int,int) board_array_size;

    // Start is called before the first frame update
    void Start()
    {
        List<Transform> child_list = BoardLibrary.CollectChildren(board_obj.transform);
        BoardLibrary.ShowAllCoordinates(child_list,false);
        GameObject[,] board_array;
        board_array = BoardLibrary.ListTo2dGrid(child_list, true);
        board_array_size=BoardLibrary.GetWidthAndHeight(board_array);
        
    }


    // Update is called once per frame
    void Update() { }

    

    public void SetSelTilePos(Vector3 new_position)
    {
        
        if (selected_tile_instance == null)
        {
            selected_tile_instance = Instantiate(selected_tile_prefab, board_obj.transform);
            selected_tile_instance.transform.localScale = Vector3.one;
            selected_tile_instance.transform.localPosition = new_position;
            
        }
        else
        {
          
            if (new_position != selected_tile_instance.transform.localPosition)
            {

                Vector3 origin_pos = selected_tile_instance.transform.localPosition;
                
                
                //To do: Currently, the origin gets the current position and transits to it, use an abstraction of current grid for movement.
                (int, int) origin = ((int)origin_pos.x, (int)origin_pos.y);
                
                (int, int) normalized_origin=NormalizePos(origin);
                Debug.Log($"origin: {origin}");
                Debug.Log($"Normalized origin: {normalized_origin}");

                Vector3 target_pos = new_position;
                (int, int) target = ((int)target_pos.x, (int)target_pos.y);
                (int, int) normalized_target=NormalizePos(target);
                Debug.Log($"target: {target}");
                Debug.Log($"Normalized target: {normalized_target}");
                Debug.Log($"DeNormalized target: {ReverseNormalizePos(normalized_target)}");

                List<(int, int)> path = AIScanner.FindPossiblePaths(normalized_origin, normalized_target,board_array_size).ToList();
                //AIScanner.PrintPath(path);

                StartCoroutine(MoveObjectCoroutine(selected_tile_instance.transform, path));
                //HandleTileMovement(origin, target);
            }
            else
            {
                Destroy(selected_tile_instance);
            }
        }
    }

    private IEnumerator MoveObjectCoroutine(Transform tile, List<(int, int)> path)
    {
        Debug.Log("Start coroutine");
        event_system.enabled = false;
        
        Vector3 start_pos = tile.localPosition;
        Vector3 target_next_pos;
        float journeyLength;
        float speed = 100f;
        float startTime;

        for (int i = 1; i < path.Count; i++)
        {
            (int,int) reverse_normalize_path=ReverseNormalizePos(path[i]);
            target_next_pos= new Vector3(reverse_normalize_path.Item1,reverse_normalize_path.Item2,0);

            journeyLength= Vector3.Distance(start_pos, target_next_pos);
            startTime=Time.time;
            //Debug.Log($"Next pos: {target_next_pos}");

            // Continue moving until the object reaches the target position
            while (tile.localPosition != target_next_pos)
            {
                float distanceCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = distanceCovered / journeyLength;

                tile.localPosition = Vector3.Lerp(start_pos, target_next_pos, fractionOfJourney);

                yield return null; // Wait for the next frame
            }
            start_pos=target_next_pos;
            
        }
        event_system.enabled =true;
        Debug.Log("End coroutine");

    }

    

    public (int,int) NormalizePos((int,int) input){
        int min_value=8,step_size=16;

        int output_x=(input.Item1-min_value)/step_size;
        int output_y=(input.Item2-min_value)/step_size;
        
        (int,int) output=(output_x,output_y);
        return output;
    }
    public (int,int) ReverseNormalizePos((int,int) input){
        int min_value=8,step_size=16;

        int output_x=(input.Item1*step_size)+min_value;
        int output_y=(input.Item2*step_size)+min_value;
        (int,int) output=(output_x,output_y);
        return output;
    }

    public Vector3 GetSelTilePos()
    {
        if (selected_tile_instance == null)
        {
            selected_tile_instance = Instantiate(selected_tile_prefab, board_obj.transform);
        }

        return selected_tile_instance.transform.position;
    }

    }
