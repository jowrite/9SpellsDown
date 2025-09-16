using UnityEngine;
using System.Collections.Generic;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int startingCurse = 9;

    //Event to notify curse updates
    public System.Action OnScoresUpdated;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void InitializeScores(List<PlayerData> players)
    {
        foreach (var p in players)
        {
            p.curseLevel = startingCurse;
            p.spellCastsThisRound = 0;
        }

        OnScoresUpdated?.Invoke();
    }

    public void ResolveRound(List<PlayerData> players)
    {
        foreach (PlayerData player in players)
        {
            int spellcasts = player.spellCastsThisRound;

            int curseDelta = 4 - spellcasts;
            player.curseLevel += curseDelta;
            player.curseLevel = Mathf.Max(0, player.curseLevel);

            Debug.Log($"{player.playerName} won {spellcasts} spellcasts. Curse change: {curseDelta}. \nCurrent curse level:{player.curseLevel}");

            player.spellCastsThisRound = 0; //Reset for next round

        }

        CheckGameEnd(players);

        //Notify UI
        OnScoresUpdated?.Invoke();
    }

    
    public bool CheckGameEnd(List<PlayerData> players)
    {
        foreach (var p in players)
        {
            if (p.curseLevel <= 0)
            {
                Debug.Log($"{p.playerName} has dispelled their curse!");
                return true; // Game ends if any player reaches curse level 0
            }
        }
        return false; // Game continues if no player has dispelled their curse
    }
}