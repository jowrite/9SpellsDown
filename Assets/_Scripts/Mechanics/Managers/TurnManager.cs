using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static TurnManager turn;

    public List<PlayerData> playerOrder = new List<PlayerData>(); //rotating list of players
    [SerializeField] private List<PlayerHUD> playerHUDs;
    private int currentIndex = 0;

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
        UpdateTurnHighlight (playerOrder[0]);

        currentIndex = 0;
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
        PlayerData currentPlayer = playerOrder[currentIndex];
        Debug.Log($"{currentPlayer.playerName}'s turn!");

        //Update HUD highlight
        for (int i = 0; i < playerOrder.Count; i++)
        {
            bool isActive = (playerOrder[i] == currentPlayer);
            playerHUDs[i].SetTurnHighlight(isActive);
        }

        currentPlayer.TakeTurn(); //Calls player or AI logic
    }

    //Might not need this tbd
    public PlayerData GetNextPlayer()
    {
        int next = (currentIndex + 1) % playerOrder.Count;
        return playerOrder[next]; // Get the next player in the order
    }

    public PlayerData GetCurrentPlayer()
    {
        if (playerOrder == null || playerOrder.Count == 0) return null;
        return playerOrder[currentIndex]; // Get the current player
    }

    public void EndPlayerTurn()
    {
        currentIndex++;

        if (currentIndex < playerOrder.Count)
        {
            StartPlayerTurn();
        }
        //TrickManager handles resolution
    }

    //Visual shader highlight for the leader
    private void UpdateTurnHighlight(PlayerData active)
    {
        for (int i = 0; i < playerOrder.Count; i++)
        {
            bool isActive = (playerOrder[i] == active);
            playerHUDs[i].SetTurnHighlight(isActive);
        }
    }
}
