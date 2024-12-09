using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceContainer : MonoBehaviour
{
    private Queue<int> dice_values_queue;

    
    public Transform dice_container_1,
        dice_container_2;

    private float[] dice_positions = new float[3] { -330, 0, 330 };
    private List<GameObject> dice_list_1 = new List<GameObject>();
    private List<GameObject> dice_list_2 = new List<GameObject>();

    private Transform[] dice_selected = new Transform[2];

    public bool debug_this_script = true;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeDiceQueue();
        StartCoroutine(InitializeDicesWithCooldown());
        Debug.Log("Dice container script initialized");
        SetDicesPermission(false);
    }

    private void InitializeDiceQueue()
    {
        // Create a list with numbers from 1 to 20
        List<int> numbers = new List<int>();

        for (int i = 1; i < 21; i++)
        {
            numbers.Add(i);
        }

        System.Random random = new System.Random();
        // Shuffle the list randomly
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            // Swap the current element with the randomly chosen one
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        // Initialize the queue with the shuffled numbers
        dice_values_queue = new Queue<int>(numbers);
    }

    public int GetNextNumber()
    {
        // Get the number at the top of the queue
        int nextNumber = dice_values_queue.Dequeue();
        // Move the number to the bottom of the queue
        dice_values_queue.Enqueue(nextNumber);
        // Return the number
        return nextNumber;
    }

    private IEnumerator InitializeDicesWithCooldown()
    {
        bool switch_container = true;
        GameObject dice;
        for (int i = 0; i < 6; i++)
        {
            if (switch_container)
            {
                dice = InstantiateDice(dice_container_1.transform);
                dice.tag = Constants.player_1_tag;
                dice_list_1.Add(dice);
            }
            else
            {
                dice = InstantiateDice(dice_container_2.transform);
                dice.tag = Constants.player_2_tag;
                dice_list_2.Add(dice);
            }
            switch_container = !switch_container;

            // Wait for 0.2 seconds before generating the next dice
            yield return new WaitForSeconds(0.3f);
        }
    }

    private GameObject InstantiateDice(Transform parent)
    {
        GameObject dice = Instantiate(Prefabs.Instance.dice_prefab, parent);
        dice.GetComponent<DiceInteraction>().SetValue(GetNextNumber());

        dice.transform.localPosition = new Vector3(0, 1000, 0); // Start at y=1000

        int dice_pos_index = parent.childCount - 1;
        float dice_pos = dice_positions[dice_pos_index];

        // Start the movement of the dice
        StartCoroutine(MoveDiceToPosition(dice, new Vector3(0, dice_pos, 0)));
        return dice;
    }

    public void ConsumeDice()
    {
        // Consume the first selected dice
        int index;
        GameObject consumedDice;

        // Handle dice from dice_list_1
        if (dice_selected[0] != null)
        {
            index = dice_list_1.IndexOf(dice_selected[0].gameObject);
            if (index != -1)
            {
                consumedDice = dice_list_1[index];
                consumedDice.transform.SetParent(null);
                dice_list_1.RemoveAt(index);
                Destroy(consumedDice);

                // Move remaining dice downward in dice_list_1
                for (int i = index; i < dice_list_1.Count; i++)
                {
                    GameObject dice = dice_list_1[i];
                    float newYPosition = dice_positions[i]; // Update position based on the index
                    StartCoroutine(MoveDiceToPosition(dice, new Vector3(0, newYPosition, 0)));
                }

                // Instantiate a new dice at the top
                GameObject newDice = InstantiateDice(dice_container_1);
                dice_list_1.Add(newDice);
            }
        }

        // Handle dice from dice_list_2
        if (dice_selected[1] != null)
        {
            index = dice_list_2.IndexOf(dice_selected[1].gameObject);
            if (index != -1)
            {
                consumedDice = dice_list_2[index];
                consumedDice.transform.SetParent(null);
                dice_list_2.RemoveAt(index);
                Destroy(consumedDice);

                // Move remaining dice downward in dice_list_2
                for (int i = index; i < dice_list_2.Count; i++)
                {
                    GameObject dice = dice_list_2[i];
                    float newYPosition = dice_positions[i]; // Update position based on the index
                    StartCoroutine(MoveDiceToPosition(dice, new Vector3(0, newYPosition, 0)));
                }

                // Instantiate a new dice at the top
                GameObject newDice = InstantiateDice(dice_container_2);
                dice_list_2.Add(newDice);
            }
        }
    }

    private IEnumerator MoveDiceToPosition(GameObject dice, Vector3 targetPosition)
    {
        float gravity = 9.8f; // Acceleration due to gravity (units/second^2)
        float timeToReach = 3f; // Total time to reach the destination
        float timeElapsed = 0f;

        // Loop until we reach the target position
        while (timeElapsed < timeToReach)
        {
            // Calculate the distance to move based on gravity
            float displacement = 0.5f * gravity * Mathf.Pow(timeElapsed, 2); // s = 0.5 * g * t^2
            Vector3 newPosition = dice.transform.localPosition + Vector3.down * displacement;

            // Check if the new position is above the target position
            if (newPosition.y <= targetPosition.y)
            {
                dice.transform.localPosition = targetPosition;
                break;
            }

            dice.transform.localPosition = newPosition;

            // Update the elapsed time
            timeElapsed += Time.deltaTime;

            // Yield until the next frame
            yield return null;
        }

        // Ensure the dice reaches the exact target position at the end
        dice.transform.localPosition = targetPosition;
    }

    public void SetDicesPermission(int player_id)
    {
        List<GameObject> sel_list,
            unsel_list;
        switch (player_id)
        {
            case 0:
                sel_list = dice_list_1;
                unsel_list = dice_list_2;
                break;
            case 1:
                sel_list = dice_list_2;
                unsel_list = dice_list_1;
                break;

            default:
                Debug.Log("Player id invalid");
                unsel_list = new List<GameObject>();
                unsel_list.AddRange(dice_list_1);
                unsel_list.AddRange(dice_list_2);
                return;
        }
        DiceInteraction dice_script;
        if (sel_list.Count > 0)
        {
            foreach (var dice in sel_list)
            {
                dice_script = dice.GetComponent<DiceInteraction>();
                dice_script.SetInteractable(true);
            }
        }
        if (unsel_list.Count > 0)
        {
            foreach (var dice in unsel_list)
            {
                dice_script = dice.GetComponent<DiceInteraction>();
                dice_script.SetInteractable(false);
            }
        }
    }

    public void SetDicesPermission(bool state = false)
    {
        List<GameObject> sel_list = new List<GameObject>();
        sel_list.AddRange(dice_list_1);
        sel_list.AddRange(dice_list_2);
        DiceInteraction dice_script;
        foreach (var dice in sel_list)
        {
            dice_script = dice.GetComponent<DiceInteraction>();
            dice_script.SetInteractable(state);
        }
    }

    public GameObject GetSelDice(int id, int index)
    {
        return dice_selected[id].gameObject;
    }

    public int GetSelDiceValue(int id, int index)
    {
        switch (id)
        {
            case 0:
                if (dice_selected[0] != null)
                {
                    dice_selected[0].GetComponent<DiceInteraction>().HighLightDice(true);
                }
                dice_selected[0] = dice_list_1[index].transform;
                return dice_selected[0].GetComponent<DiceInteraction>().GetValue();
            case 1:
                if (dice_selected[0] != null)
                {
                    dice_selected[0].GetComponent<DiceInteraction>().HighLightDice(true);
                }
                dice_selected[1] = dice_list_2[index].transform;
                return dice_list_2[index].GetComponent<DiceInteraction>().GetValue();
            default:
                throw new Exception("Invalid id on getSelValue");
        }
    }
}
