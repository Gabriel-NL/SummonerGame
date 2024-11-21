using System.Collections;
using TMPro;
using UnityEngine;

public class Players_script : MonoBehaviour
{
    // References to the labels
    public TextMeshProUGUI counter_1_label, counter_2_label;

    private int counter = 10; // Counter to be displayed
    private TextMeshProUGUI activeLabel; // Currently active label

    private void Start()
    {
        // Randomly select the initial active label
        activeLabel = (Random.value > 0.5f) ? counter_1_label : counter_2_label;

        Debug.Log("Initial Active Label: " + activeLabel.name);

        StartCoroutine(IncrementCounter());
        StartCoroutine(SwitchLabelEvery10Seconds());
    }

    private IEnumerator IncrementCounter()
    {
        while (true) // Run indefinitely
        {
            counter--; // Decrement the counter
            if (activeLabel != null) // Update only the active label
            {
                activeLabel.text = $"{counter}";
            }
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }
    }

    private IEnumerator SwitchLabelEvery10Seconds()
    {
        while (true) // Run indefinitely
        {
            counter = 10; // Reset the counter
            yield return new WaitForSeconds(10f); // Wait for 10 seconds
            SwitchLabel(); // Switch the active label
        }
    }

    private void SwitchLabel()
    {
        // Toggle the active label between counter_1_label and counter_2_label
        activeLabel = (activeLabel == counter_1_label) ? counter_2_label : counter_1_label;

        // Optionally, clear the text of the other label
        if (activeLabel == counter_1_label && counter_2_label != null)
            counter_2_label.text = "";
        else if (activeLabel == counter_2_label && counter_1_label != null)
            counter_1_label.text = "";

        Debug.Log("Switched Active Label to: " + activeLabel.name);
    }
}
