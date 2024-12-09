using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prefabs : MonoBehaviour
{
    // Singleton instance
    private static Prefabs _instance;

    // Public static property to access the instance
    public static Prefabs Instance
    {
        get
        {
            if (_instance == null)
            {
                // Ensure the instance is created and managed by Unity
                _instance = FindObjectOfType<Prefabs>();

                // Ensure there's only one instance in the scene
                if (_instance == null)
                {
                    // If not found, create a new game object with Prefabs component
                    GameObject singletonObject = new GameObject(nameof(Prefabs));
                    _instance = singletonObject.AddComponent<Prefabs>();
                }
            }
            return _instance;
        }
    }

    // Prefabs
    [SerializeField]
    public GameObject sel_tile_prefab;

    [SerializeField]
    public GameObject entity_prefab;
    public GameObject tile_prefab;
    public GameObject dice_prefab;
    public Sprite[] images;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Ensuring there is only one instance
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
}
