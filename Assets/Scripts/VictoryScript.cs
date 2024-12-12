using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryScript : MonoBehaviour
{
    
    public TextMeshProUGUI victorious;
    void Start()
    {
        victorious.text = PlayerPrefs.GetString("Winner", "No Winner");
    }

}
