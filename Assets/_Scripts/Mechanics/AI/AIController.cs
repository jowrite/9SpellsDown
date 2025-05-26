using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    public List<PlayerData> aiPlayers;
    public static AIController Instance;

    public void TakeAITurn(PlayerData currentAI)
    {
        StartCoroutine(AIDelayPlay(currentAI));
    }

    private IEnumerator AIDelayPlay(PlayerData ai)
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.2f)); //simulate "thinking"

        CardData chosen = ChooseCard(ai.hand);
        ai.hand.Remove(chosen);

        TrickManager.tm.PlayCard(ai, chosen);
    }

    private CardData ChooseCard(List<CardData> hand)
    {
        //Simple Ai logic: follows lead if possible, else lowest card
        ElementType lead = TrickManager.tm.leadElement;

        List<CardData> followSuit = hand.FindAll(card => card.element == lead);

        if (followSuit.Count > 0)
        {
            return followSuit[Random.Range(0, followSuit.Count)];
        }

        //No matching suit, play lowest value card
        hand.Sort((a,b) => a.value.CompareTo(b.value));
        return hand[0];
    }
}
