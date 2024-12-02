using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Players_script : MonoBehaviour
{
    // References to the labels
    public GameObject player_prefab;
    public TextMeshProUGUI InstructionsLabel;
    public int active_user_id;
    private GameObject[] players= new GameObject[2];
    private int counter = 10; // Counter to be displayed
     private SelectionTile sel_tile_script;

    private void Start()
    {
        // Randomly select the initial active label
       
        StartingPlayer();
        
        sel_tile_script = GameObject
            .FindWithTag(Constants.selection_object_tag)
            .GetComponent<SelectionTile>();
         InstructionsLabel.text=$"Active player:{active_user_id}";
        InitializePlayers();
        SetUser(active_user_id);
    }

    void Update()
    {
        // Check if the user presses the Enter key
        if (Input.GetKeyDown(KeyCode.Return)) // KeyCode.Return is for Enter
        {
            Debug.Log("Switching user");
            // Perform the desired action
            SwitchUser();
        }
    }    

    private void InitializePlayers(){
        BoardScript_V2 board_script= gameObject.GetComponent<BoardScript_V2>();
        GameObject player;

        player=Instantiate(player_prefab);
        player.tag=Constants.player_1_tag;
        players[0]=player;
        board_script.AddToEntityTable((0,4),players[0]);

        player=Instantiate(player_prefab);
        player.tag=Constants.player_2_tag;
        players[1]=player;
        board_script.AddToEntityTable((8,4),players[1]);
    }
    private void StartingPlayer(){
        active_user_id = (Random.value > 0.5f) ? 1 : 2;
    }
    private void SwitchUser(){
        sel_tile_script.SetParent();
        if (active_user_id==2)
        {
            active_user_id=1;
        }else
        {
            active_user_id=2;
        }
        SetUser(active_user_id);
        InstructionsLabel.text=$"Active player:{active_user_id}";
    }
    private void SetUser(int id){


        if (id==1)
        {
            active_user_id=1;
            UpdatePermissions(Constants.player_1_tag,true);
            UpdatePermissions(Constants.player_2_tag,false);
            
        }else
        {
            active_user_id=2;
            UpdatePermissions(Constants.player_2_tag,true);
            UpdatePermissions(Constants.player_1_tag,false);
        }

        
    }
    public void UpdatePermissions(string tag,bool status){
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        // Iterate through each object

        foreach (GameObject obj in objectsWithTag)
        {
            try
            {
                // Get the EventTrigger component attached to the object
            EntityInteraction entity =obj.GetComponent<EntityInteraction>();
            if (entity)
            {
                entity.SetPermission(status);
            }else
            {
                EventTrigger eventTrigger = obj.GetComponent<EventTrigger>();

            eventTrigger.enabled = status;
            }
            
            }
            catch (System.Exception error)
            {
                Debug.Log($"Error: {error}");
                 Debug.Log($"Obj: {obj.name}");
                throw;
            }
            
            
        }
    }

}
