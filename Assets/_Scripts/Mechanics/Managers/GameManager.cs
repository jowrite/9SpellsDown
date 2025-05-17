using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentRound = 1;
    public int maxRounds = 13;
    private int totalCurseStart = 27;

    public List<PlayerData> players; //Populate in Inspector
    public List<PlayerHUD> playerHUDs; //Populate in Inspector


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartRound()
    {
        Debug.Log($"Round {currentRound} starts!");
        foreach (PlayerData player in players)
        {
            player.ResetRoundStats();
        }

        //Shuffle, deal, trick-taking logic will live here
    }

    public void EndRound()
    {
        Debug.Log($"Round {currentRound} has ended!");

        foreach (PlayerData player in players)
        {
            int spellcasts = player.spellCastsThisRound;

            int curseDelta = 4 - spellcasts;
            player.curseLevel += curseDelta;
            player.curseLevel = Mathf.Clamp(player.curseLevel, 0, 9);

            Debug.Log($"{player.playerName} won {spellcasts} spellcasts. Curse change: {curseDelta}. \nCurrent curse level:{player.curseLevel}");

        }

        //Update HUDs
        foreach (PlayerHUD hud in playerHUDs)
        {
            hud.UpdateHUD(players[playerHUDs.IndexOf(hud)]);
        }

        CheckMagicLaw();
        currentRound++;

        if (currentRound > maxRounds || players.Any(p => p.curseLevel <= 0))
        {
            EndGame();
        }
        else
        {
            // Add delay or wait for player input before starting next round
            StartRound();
        }

    }

    private void CheckMagicLaw()
    {
        int expectedTotal = totalCurseStart - currentRound + 1;
        int actualTotal = players.Sum(p => p.curseLevel);

        if (actualTotal != expectedTotal)
        {
            Debug.LogWarning($"Magic Law violation! Expected total curse level: {expectedTotal}, Actual total curse level: {actualTotal}");
            // Handle magic law violation (e.g., apply penalties, notify players, etc.)
        }
        else
        {
            Debug.Log("Magic Law is upheld!");
        }

    }

    private void EndGame()
    {
        Debug.Log("Game Over!");
        var winners = players.Where(p => p.curseLevel == 0).ToList();

        if(winners.Count > 0)
        {
            foreach(var w in winners)
            {
                Debug.Log($"{w.playerName} diespelled their curse!");
            }
        }
        else
        {
            Debug.Log("No one dispelled their curse on time!");
        }

    }

    private void UpdateAllHUDs()
    {
        for (int i = 0; i < players.Count; i++)
        {
            playerHUDs[i].UpdateHUD(players[i]);
            playerHUDs[i].SetLeaderHighlight(i == currentRound % players.Count);
        }
    }


}
