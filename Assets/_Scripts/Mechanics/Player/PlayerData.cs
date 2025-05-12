using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string playerName;
    public int curseLevel = 9;
    public int spellCastsThisRound;

    public List<CardData> hand = new List<CardData>();

    public void TakeTurn()
    {
        if (isHuman)
        {
            //Enable UI to select card
        }
        else
        {
            //AI logic to pick card & call TrickManager.instance.PlayCard(this, selectedCard);
        }
    }

    public bool isHuman = false; // Set this based on player type (human or AI)

    public void ResetRoundStats()
    {
        spellCastsThisRound = 0;
    }
}
