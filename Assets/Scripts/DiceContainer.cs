using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceContainer : MonoBehaviour
{
    private int active_player;
    private Queue<int> dice_values_queue;

    public GameObject dice_prefab;
    public GameObject dice_container_1,
        dice_container_2;

    private float[] dice_positions = new float[3] { -330, 0, 330 };
    private List<GameObject> dice_list_1 = new List<GameObject>();
    private List<GameObject> dice_list_2 = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        InitializeDiceQueue();
        StartCoroutine(GenerateDicesWithCooldown());
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

    private IEnumerator GenerateDicesWithCooldown()
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
        GameObject dice = Instantiate(dice_prefab, parent);
        dice.GetComponent<DiceInteraction>().SetValue(GetNextNumber());

        dice.transform.localPosition = new Vector3(0, 1000, 0); // Start at y=1000

        int dice_pos_index = parent.childCount - 1;
        float dice_pos = dice_positions[dice_pos_index];

        // Start the movement of the dice
        StartCoroutine(MoveDiceToPosition(dice, new Vector3(0, dice_pos, 0)));
        return dice;
    }

    private IEnumerator MoveDiceToPosition(GameObject dice, Vector3 targetPosition)
    {
        float initialSpeed = 1000f; // Initial speed
        float timeToReach = 4f; // Total time to reach the destination
        float timeElapsed = 0f;

        // Loop until we reach the target position
        while (timeElapsed < timeToReach)
        {
            // Calculate a smooth move towards the target
            float moveSpeed = Mathf.Lerp(initialSpeed, 0f, timeElapsed / timeToReach); // Reduce speed over time
            dice.transform.localPosition = Vector3.MoveTowards(
                dice.transform.localPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Update the elapsed time
            timeElapsed += Time.deltaTime;

            // Yield until the next frame
            yield return null;
        }

        // Ensure the dice reaches the exact target position at the end
        dice.transform.localPosition = targetPosition;
    }
}
