using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static TurnManager turn;

    public List<PlayerData> playerOrder = new List<PlayerData>(); //rotating list of players
    [SerializeField] private List<PlayerHUD> playerHUDs;
    public int currentPlayerIndex = 0;

    private void Awake()
    {
        if (turn == null) turn = this;
        else Destroy(gameObject);
    }

    public void StartNewTrick(PlayerData winner)
    {
        //Reorder the player list so winner goes first
        ReorderPlayerList(winner);
        
        //Highlight the new leader visually
        UpdateLeaderHighlight (playerOrder[0]);

        currentPlayerIndex = 0;
        TrickManager.tm.ResetTrick();
        StartPlayerTurn();
    }

    //Ensures the winner is first in the next player order
    private void ReorderPlayerList(PlayerData winner)
    {
        if (playerOrder[0] != winner)
        {
            int winnerIndex = playerOrder.IndexOf(winner);
            if (winnerIndex >= 0)
            {
                List<PlayerData> reordered = new List<PlayerData>();
                for (int i = 0; i < playerOrder.Count; i++)
                {
                    int index = (winnerIndex + i) % playerOrder.Count;
                    reordered.Add(playerOrder[index]);
                }

                playerOrder = reordered;
            }
        }
    }

    public void StartPlayerTurn()
    {
        PlayerData currentPlayer = playerOrder[currentPlayerIndex];
        Debug.Log($"{currentPlayer.playerName}'s turn!");

        currentPlayer.TakeTurn(); //Calls player or AI logic
    }

    public PlayerData GetNextPlayer()
    {
        return playerOrder[currentPlayerIndex];
    }

    public void EndPlayerTurn()
    {
        currentPlayerIndex++;

        if (currentPlayerIndex < playerOrder.Count)
        {
            StartPlayerTurn();
        }
        //TrickManager handles resolution
    }

    //Visual shader highlight for the leader
    private void UpdateLeaderHighlight(PlayerData leader)
    {
        for (int i = 0; i < playerOrder.Count; i++)
        {
            bool isLeader = (playerOrder[i] == leader);
            playerHUDs[i].SetLeaderHighlight(isLeader);
        }
    }
}
