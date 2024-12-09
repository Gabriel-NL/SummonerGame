using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardControllerV3 : MonoBehaviour
{
    //Elements
    [SerializeField]
    private GameObject board_obj;

    [SerializeField]
    private Image shadow;

    [SerializeField]
    private EventSystem event_system;

    [SerializeField]
    private TextMeshProUGUI instructions_label,
        values_Label;
    private GameObject dice_container_1,
        dice_container_2;

    //Scripts
    private UnityBoardClass board_instance;
    private DiceContainer dice_container_script;

    //Variables
    private (int, int) current_selected_coordinates_p1,
        current_selected_coordinates_p2;

    private int current_sel_dice_p1,
        current_sel_dice_p2;
    private GameObject player_1_sel_tile,
        player_2_sel_tile;
    private GameObject selected_entity = null;
    public bool debug_boolean;
    int active_user_id;
    private List<GameObject> p1_entities_list = new List<GameObject>();
    private List<GameObject> p2_entities_list = new List<GameObject>();
    private bool expecting_dice = false;
    private GameObject[] loaded_dice_p1 = new GameObject[3],
        loaded_dice_p2 = new GameObject[3];
    private int loaded_dice_index_p1,
        loaded_dice_index_p2;
    private int[] values_used = new int[2] { 0, 0 };

    private GameObject offensive_entity,
        passive_entity;

    void Awake()
    {
        InitializeBoard();
        Debug.Log("Board initialized");
        InitializeSelectors();
        HandleDiceInitialization();
        Debug.Log("Selectors initialized");
        DecidingWhoGetsFirstTurn();
        Debug.Log("First turn decided!");
        SwitchUser();
        InitializePlayers();
        Debug.Log("Players initialized!");
        UpdateEntityPermissions();
        Debug.Log("Permissions updated!");

        StartCoroutine(HandlePlayersInput());
    }

    private void InitializeBoard()
    {
        RectTransform board_rect = board_obj.GetComponent<RectTransform>();
        (float, float) board_rect_width_and_height = (
            board_rect.rect.width,
            board_rect.rect.height
        );

        RectTransform rect_tile = Prefabs.Instance.tile_prefab.GetComponent<RectTransform>();
        (float, float) tile_width_height = (rect_tile.rect.width, rect_tile.rect.height);

        Transform[] child_array = BoardLibrary.CollectChildren(board_obj.transform);
        board_instance = UnityBoardClass.Instance;
        board_instance.UseExistingBoard(
            child_array,
            tile_width_height,
            board_rect_width_and_height
        );
        //board_instance.CreateNewBoard(tile_prefab,board_obj.transform,board_rect_width_and_height);
        shadow.raycastTarget = false;
    }

    private void InitializeSelectors()
    {
        player_1_sel_tile = Instantiate(Prefabs.Instance.sel_tile_prefab);
        player_2_sel_tile = Instantiate(Prefabs.Instance.sel_tile_prefab);
        SetSelTileParent(player_1_sel_tile, 0, 0, 8);
        SetSelTileParent(player_2_sel_tile, 1, 8, 8);

        player_1_sel_tile.GetComponent<SelectionTile>().SetColor(Color.white);
        player_2_sel_tile.GetComponent<SelectionTile>().SetColor(Color.red);
    }

    private void InitializePlayers()
    {
        GameObject entity;

        entity = Instantiate(Prefabs.Instance.entity_prefab);
        entity.name = "Player 1";
        entity.tag = Constants.player_1_tag;
        entity.GetComponent<EntityInteraction>().InitializeAttributes("Necromancer");
        entity.GetComponent<Image>().color = Color.white;
        p1_entities_list.Add(entity);
        AddToEntityTable((0, 4), entity);

        entity = Instantiate(Prefabs.Instance.entity_prefab);
        entity.name = "Wendigo 1";
        entity.tag = Constants.player_1_tag;
        entity.GetComponent<EntityInteraction>().InitializeAttributes("Wendigo");
        entity.GetComponent<Image>().color = Color.white;
        p1_entities_list.Add(entity);
        AddToEntityTable((0, 5), entity);

        entity = Instantiate(Prefabs.Instance.entity_prefab);
        entity.name = "Wendigo 2";
        entity.tag = Constants.player_1_tag;
        entity.GetComponent<EntityInteraction>().InitializeAttributes("Wendigo");
        entity.GetComponent<Image>().color = Color.white;
        p1_entities_list.Add(entity);
        AddToEntityTable((1, 4), entity);

        entity = Instantiate(Prefabs.Instance.entity_prefab);
        entity.name = "Wendigo 3";
        entity.tag = Constants.player_1_tag;
        entity.GetComponent<EntityInteraction>().InitializeAttributes("Wendigo");
        entity.GetComponent<Image>().color = Color.white;
        p1_entities_list.Add(entity);
        AddToEntityTable((0, 3), entity);

        entity = Instantiate(Prefabs.Instance.entity_prefab);
        entity.name = "Player 2";
        entity.tag = Constants.player_2_tag;
        entity.GetComponent<EntityInteraction>().InitializeAttributes("Necromancer");
        entity.GetComponent<Image>().color = Color.red;
        p2_entities_list.Add(entity);
        AddToEntityTable((8, 4), entity);

        entity = Instantiate(Prefabs.Instance.entity_prefab);
        entity.name = "Wendigo 4";
        entity.tag = Constants.player_2_tag;
        entity.GetComponent<EntityInteraction>().InitializeAttributes("Wendigo");
        entity.GetComponent<Image>().color = Color.red;
        p2_entities_list.Add(entity);
        AddToEntityTable((8, 5), entity);

        entity = Instantiate(Prefabs.Instance.entity_prefab);
        entity.name = "Wendigo 5";
        entity.tag = Constants.player_2_tag;
        entity.GetComponent<EntityInteraction>().InitializeAttributes("Wendigo");
        entity.GetComponent<Image>().color = Color.red;
        p2_entities_list.Add(entity);
        AddToEntityTable((7, 4), entity);

        entity = Instantiate(Prefabs.Instance.entity_prefab);
        entity.name = "Wendigo 6";
        entity.tag = Constants.player_2_tag;
        entity.GetComponent<EntityInteraction>().InitializeAttributes("Wendigo");
        entity.GetComponent<Image>().color = Color.red;
        p2_entities_list.Add(entity);
        AddToEntityTable((8, 3), entity);
    }

    private void DecidingWhoGetsFirstTurn()
    {
        active_user_id = (Random.value > 0.5f) ? 0 : 1;
        instructions_label.text = $"Press enter to switch player";
        values_Label.text = $"Active player:{active_user_id + 1}";
    }

    private void HandleDiceInitialization()
    {
        dice_container_script = gameObject.GetComponent<DiceContainer>();
        dice_container_1 = dice_container_script.dice_container_1.gameObject;
        dice_container_2 = dice_container_script.dice_container_2.gameObject;
    }

    private IEnumerator HandlePlayersInput()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.anyKeyDown);

            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveSelTile(player_1_sel_tile, 0, (0, 1));
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                MoveSelTile(player_1_sel_tile, 0, (-1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                MoveSelTile(player_1_sel_tile, 0, (0, -1));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                MoveSelTile(player_1_sel_tile, 0, (1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                HandleControlButton(0);
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                HandleShiftButton(0);
            }
            // Check which key was pressed
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveSelTile(player_2_sel_tile, 1, (0, 1));
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveSelTile(player_2_sel_tile, 1, (-1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveSelTile(player_2_sel_tile, 1, (0, -1));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveSelTile(player_2_sel_tile, 1, (1, 0));
            }
            else if (Input.GetKeyDown(KeyCode.RightShift))
            {
                HandleShiftButton(1);
            }
            else if (Input.GetKeyDown(KeyCode.RightControl))
            {
                HandleControlButton(1);
            }

            // Wait for the next frame to prevent processing the same key press again
            yield return null;
        }
    }

    private void SetSelTileParent(GameObject sel, int id, int x, int y)
    {
        Transform trans = sel.transform;
        trans.SetParent(board_instance.GetObjectOnMapLayer(x, y).transform);
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;
        BoardLibrary.MoveChildToBottom(sel.transform);
        RectTransform rect = sel.GetComponent<RectTransform>();
        rect.sizeDelta = Vector2.zero;
        sel.GetComponent<Image>().enabled = true;

        if (id == 0)
        {
            current_selected_coordinates_p1 = (x, y);
        }
        if (id == 1)
        {
            current_selected_coordinates_p2 = (x, y);
        }
    }

    private void SetDiceParent(GameObject sel, int id, int y)
    {
        Transform trans = sel.transform;
        GameObject[] loaded_dice;
        switch (id)
        {
            case 0:
                loaded_dice = loaded_dice_p1;
                loaded_dice_index_p1 = y;
                break;
            case 1:
                loaded_dice = loaded_dice_p2;
                loaded_dice_index_p2 = y;
                break;
            default:
                Debug.LogError("Invalid id on setDiceParent");
                return;
        }

        trans.SetParent(loaded_dice[y].transform);
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;
        RectTransform rect = sel.GetComponent<RectTransform>();
        rect.sizeDelta = Vector2.zero;
    }

    public void MoveSelTile(GameObject sel, int id, (int, int) direction)
    {
        if (expecting_dice)
        {
            DiceMovement(sel, id, direction.Item2);
        }
        else
        {
            NormalMovement(sel, id, direction);
        }
    }

    private void NormalMovement(GameObject sel, int id, (int, int) direction)
    {
        int modified_x;
        int modified_y;
        switch (id)
        {
            case 0:
                modified_x = current_selected_coordinates_p1.Item1 + direction.Item1;
                modified_y = current_selected_coordinates_p1.Item2 + direction.Item2;
                break;
            case 1:
                modified_x = current_selected_coordinates_p2.Item1 + direction.Item1;
                modified_y = current_selected_coordinates_p2.Item2 + direction.Item2;
                break;
            default:
                return;
        }

        (int, int) limits = board_instance.GetGridWidthHeight();

        bool x_in_limit = modified_x > -1 && modified_x < limits.Item1;
        bool y_in_limit = modified_y > -1 && modified_y < limits.Item2;
        if (x_in_limit && y_in_limit)
        {
            SetSelTileParent(sel, id, modified_x, modified_y);
        }
    }

    private void DiceMovement(GameObject sel, int id, int direction_y)
    {
        int modified_y;
        switch (id)
        {
            case 0:
                modified_y = loaded_dice_index_p1 + direction_y;
                break;
            case 1:
                modified_y = loaded_dice_index_p2 + direction_y;
                break;
            default:
                return;
        }

        bool y_in_limit = modified_y > -1 && modified_y < 3;
        if (y_in_limit)
        {
            SetDiceParent(sel, id, modified_y);
        }
    }

    public void HandleShiftButton(int player_id)
    {
        Debug.Log("Interacting with tile");
        bool entity_selected = selected_entity != null;
        (int, int) target_coordinates;
        switch (player_id)
        {
            case 0:
                target_coordinates = current_selected_coordinates_p1;
                break;
            case 1:
                target_coordinates = current_selected_coordinates_p2;
                break;
            default:
                Debug.LogError("Invalid id on shift handler");
                return;
        }
        if (expecting_dice)
        {
            HandleDiceSelection(player_id);
            return;
        }
        if (entity_selected)
        {
            HandlePlayerInteraction(target_coordinates.Item1, target_coordinates.Item2);
        }
        else
        {
            HandleSelection(target_coordinates.Item1, target_coordinates.Item2);
        }
    }

    private void HandleDiceSelection(int id)
    {
        int value;
        switch (id)
        {
            case 0:
                value = dice_container_script.GetSelDiceValue(id, loaded_dice_index_p1);
                EditDiceValues(0, value);
                break;
            case 1:
                value = dice_container_script.GetSelDiceValue(id, loaded_dice_index_p2);
                EditDiceValues(1, value);
                break;
            default:
                Debug.LogError("Invalid id on handle selection");
                return;
        }
    }

    public void HandleControlButton(int player_id)
    {
        if (expecting_dice)
        {
            return;
        }
        Debug.Log("Handling Turn change");
        if (player_id != active_user_id)
        {
            Debug.Log("Not your turn");
            return;
        }

        if (player_id == 0)
        {
            active_user_id = 1;
            dice_container_1.GetComponent<Image>().color = Color.white;
            dice_container_2.GetComponent<Image>().color = Color.red;
        }
        else
        {
            active_user_id = 0;
            dice_container_1.GetComponent<Image>().color = Color.blue;
            dice_container_2.GetComponent<Image>().color = Color.white;
        }
        values_Label.text = $"Active player:{active_user_id + 1}";
        UpdateEntityPermissions();
        UnityBoardClass.Instance.RemoveHighlight();
    }

    private void SwitchUser()
    {
        if (dice_container_1 == null)
        {
            Debug.Log("Something is wrong");
        }
        if (active_user_id == 0)
        {
            active_user_id = 1;
            dice_container_1.GetComponent<Image>().color = Color.white;
            dice_container_2.GetComponent<Image>().color = Color.red;
        }
        else
        {
            active_user_id = 0;
            dice_container_1.GetComponent<Image>().color = Color.blue;
            dice_container_2.GetComponent<Image>().color = Color.white;
        }
        values_Label.text = $"Active player:{active_user_id + 1}";
        UpdateEntityPermissions();
        UnityBoardClass.Instance.RemoveHighlight();
    }

    public void UpdateSelTilePosition()
    {
        selected_entity = null;
        board_instance.RemoveHighlight();
    }

    public void HandleSelection(int x, int y)
    {
        Debug.Log("Handling entity selection");
        GameObject entity_obj = board_instance.GetObjectOnEntityLayer(x, y);
        if (entity_obj == null)
        {
            SeeLogs("No entity object here");
            return;
        }
        EntityInteraction entity_script = entity_obj.GetComponent<EntityInteraction>();
        if (entity_script == null)
        {
            SeeLogs("No component EntityInteraction");
            return;
        }

        if (entity_script.GetPermission() == false)
        {
            SeeLogs("No allowed to interact");
            return;
        }
        selected_entity = entity_obj;
        SeeLogs("Entity found and selected");
        int radius = entity_script.GetRadius();
        string tag = entity_script.GetTag();
        board_instance.HighlightAround(tag, radius, x, y);
    }

    public void HandlePlayerInteraction(int x, int y)
    {
        Debug.Log("Handling player interaction");
        GameObject target_obj = board_instance.GetObjectOnMapLayer(x, y);
        bool not_allowed_to_walk =
            target_obj.GetComponent<TileInteraction>().GetWalkableState() == false;

        if (not_allowed_to_walk)
        {
            SeeLogs($"Cant walk into tile: {target_obj.name}");
        }
        else
        {
            GameObject entity_obj = board_instance.GetObjectOnEntityLayer(x, y);
            if (entity_obj != null)
            {
                Debug.Log("Another entity detected: " + entity_obj.name);
                SeeLogs("Attacking object");
                offensive_entity = selected_entity;
                passive_entity = entity_obj;
                SetVisibilityshadow(true);
            }
            else
            {
                SeeLogs("Moving object");
                MoveObject(selected_entity, target_obj.transform);
            }
        }
        selected_entity = null;
        board_instance.RemoveHighlight();
    }

    private void MoveObject(GameObject entity, Transform target, int nearest_distance = 0)
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
            board_instance.GetBoardLimits()
        );

        // Remove the last `nearest_distance` elements
        if (nearest_distance > 0 && path.Length > nearest_distance)
        {
            path = path.Take(path.Length - nearest_distance).ToArray();
        }

        StartCoroutine(
            MoveObjectCoroutine(entity.transform, path, board_instance.GetGridWidthHeight())
        );
        // Get the first element
        (int, int) firstElement = path[0];

        // Get the last element
        (int, int) lastElement = path[path.Length - 1];

        board_instance.MoveEntity(firstElement, lastElement);
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
        float speed = 300f;
        float startTime;
        int path_x,
            path_y;
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
        SwitchUser();
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
        Debug.Log($"Entity added: {entity.name}");
    }

    public void ExpectingDice(bool state)
    {
        expecting_dice = state;
        if (state)
        {
            // Get all children
            for (int i = 0; i < dice_container_1.transform.childCount; i++)
            {
                loaded_dice_p1[i] = dice_container_1.transform.GetChild(i).gameObject;
            }
            for (int i = 0; i < dice_container_2.transform.childCount; i++)
            {
                loaded_dice_p2[i] = dice_container_2.transform.GetChild(i).gameObject;
            }

            dice_container_script.SetDicesPermission(true);
            loaded_dice_index_p1 = 0;
            loaded_dice_index_p2 = 0;
            SetDiceParent(player_1_sel_tile, 0, loaded_dice_index_p1);
            SetDiceParent(player_2_sel_tile, 1, loaded_dice_index_p2);
            instructions_label.text =
                $"Player {active_user_id} acting. Press enter when both selected dices";
            values_Label.text = $"{values_used[0]} vs {values_used[1]}";
        }
        else
        {
            values_used[0] = 0;
            values_used[1] = 0;
            offensive_entity = null;
            passive_entity = null;
            SwitchUser();
            SetSelTileParent(player_1_sel_tile, 0, 0, 8);
            SetSelTileParent(player_2_sel_tile, 1, 8, 8);
            dice_container_script.SetDicesPermission(false);
            instructions_label.text = $"Press enter to switch player";
            values_Label.text = $"Active player:{active_user_id + 1}";
        }
    }

    public void EditDiceValues(int id, int value)
    {
        values_used[id] = value;
        if (values_used[0] > 0 && values_used[1] > 0)
        {
            dice_container_script.ConsumeDice();
            EntityInteraction offensive = offensive_entity.GetComponent<EntityInteraction>();
            EntityInteraction defensive = passive_entity.GetComponent<EntityInteraction>();
            HandleAttack(offensive, defensive);

            Debug.Log("Attack Made!");
            SetVisibilityshadow(false);
        }
        else
        {
            values_Label.text = $"{values_used[0]} vs {values_used[1]}";
        }
    }

    private void HandleAttack(EntityInteraction atk, EntityInteraction def)
    {
        
        bool p1_wins_p2=active_user_id == 0 && values_used[0] > values_used[1];
        bool p2_wins_p1=active_user_id == 1 && values_used[1] > values_used[0];
        
        if (p1_wins_p2|| p2_wins_p1) {
            float damage,current_hp;
            damage=atk.GetDamage();
            current_hp=def.GetHP();
            current_hp=current_hp-damage;
            Debug.Log($"Name: {def.name}HP after:{current_hp}");
            if (current_hp<=0)
            {
                
                if (p1_wins_p2)
                {
                    p2_entities_list.Remove(def.gameObject);   
                }
                if (p2_wins_p1)
                {
                    p1_entities_list.Remove(def.gameObject); 
                }
                (int,int)pos =board_instance.GetNormalizedPos((def.gameObject.transform.localPosition.x,def.gameObject.transform.localPosition.y));
                Debug.Log($"Pos death: {pos}");
                    board_instance.DelValueEntityTable(pos);
                Destroy(def.gameObject);

            }else
            {
                def.SetHP(current_hp);
            }

         }
    }

    private void UpdateEntityPermissions()
    {
        List<GameObject> sel_list,
            unsel_list;

        switch (active_user_id)
        {
            case 0:

                sel_list = p1_entities_list;
                unsel_list = p2_entities_list;
                break;
            case 1:
                sel_list = p2_entities_list;
                unsel_list = p1_entities_list;
                break;
            default:
                return;
        }

        if (sel_list.Count > 0)
        {
            foreach (var entity in sel_list)
            {
                entity.GetComponent<EntityInteraction>().SetPermission(true);
            }
        }
        if (unsel_list.Count > 0)
        {
            foreach (var entity in unsel_list)
            {
                entity.GetComponent<EntityInteraction>().SetPermission(false);
            }
        }
        Debug.Log("Entity permissions updated");
    }

    public GameObject getBoardObj()
    {
        return board_obj;
    }

    public void SeeLogs(string message)
    {
        if (debug_boolean)
        {
            Debug.Log(message);
        }
    }

    public void SetVisibilityshadow(bool state)
    {
        if (state)
        {
            BoardLibrary.SetColorAlpha(shadow, 0.5f);
            shadow.raycastTarget = true;
            ExpectingDice(true);
        }
        else
        {
            BoardLibrary.SetColorAlpha(shadow, 0);
            shadow.raycastTarget = false;
            ExpectingDice(false);
        }
    }
}
