using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string playerName;
    public int curseLevel = 9;
    public int spellCastsThisRound;

    public bool isHuman = false; // Set this based on player type (human or AI)
    public bool isAI = false; // Set this based on player type (human or AI)

    public List<CardData> hand = new List<CardData>();

    public void TakeTurn()
    {
        if (isHuman)
        {
            //Wait for UI Input = handles by Card tap/swipe logic
        }
        else
        {
            AIController.Instance.TakeAITurn(this); // AI logic to play a card
        }
    }

    public void ResetRoundStats()
    {
        spellCastsThisRound = 0;
    }
}
