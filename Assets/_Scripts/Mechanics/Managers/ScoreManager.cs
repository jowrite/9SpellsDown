using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int startingCurse = 9;

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
    }

    public void ResolveRound(List<PlayerData> players)
    {
        foreach (var p in players)
        {
            int difference = p.spellCastsThisRound - 4;

            if (difference > 0)
            {
                p.curseLevel -= difference;
            }
            else if (difference < 0)
            {
                p.curseLevel += Mathf.Abs(difference); //cap increase at 4
            }

            p.spellCastsThisRound = 0; // Reset spell casts for next round
        }

        //Apply magic law balance check if needed here
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