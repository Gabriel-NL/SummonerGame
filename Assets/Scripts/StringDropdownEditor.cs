/*
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileInteraction))]
public class StringDropdownEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get reference to the target class
        TileInteraction example = (TileInteraction)target;

        // Display the default inspector
        DrawDefaultInspector();

        // Create a dropdown for selecting the string option
        example.selectedOptionIndex = EditorGUILayout.Popup("Select Option", example.selectedOptionIndex, example.options);
        
        // You can optionally show the selected option
        EditorGUILayout.LabelField("Selected Option", example.SelectedOption);
        
        // Optionally, add a space between properties
        GUILayout.Space(10);
    }
}
*/
