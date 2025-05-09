using UnityEngine;

public class PlayerData
{
    public string playerName;
    public int curseLevel = 9;
    public int spellCastsThisRound = 0;

    public void ResetRoundStats()
    {
        spellCastsThisRound = 0;
    }
}
