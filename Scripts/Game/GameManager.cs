using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    private bool hasGameEnded;

    private void Start()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(player);
        }
    }

    private void Update()
    {
        if (hasGameEnded)
        {
            Debug.Log(players[0].name + " WON!");
        }
    }


    public void EndGame(GameObject loser)
    {
        players.Remove(loser);
        hasGameEnded = true;
    }
}
