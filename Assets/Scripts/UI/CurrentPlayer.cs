using System.Collections;
using System.Collections.Generic;
using TMPro;
using Topebox.Tankwars;
using UnityEngine;

public class CurrentPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI currentPlayer;
    
    void Update()
    {
        currentPlayer.text = "Player " + GameObject.Find("Board").GetComponent<GameState>().CurrentPlayer + " move";
        
    }
}
