using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    public List<PlayerData> aiPlayers;
    public static AIController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void TakeAITurn(PlayerData currentAI)
    {
        StartCoroutine(AIDelayPlay(currentAI));
    }

    private IEnumerator AIDelayPlay(PlayerData ai)
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.2f)); //simulate "thinking"

        if (ai.hand.Count == 0)
        {
            Debug.LogWarning($"{ai.playerName} has no cards left to play!");
            yield break; //No cards to play
        }
        
        
        CardData chosen = ChooseCard(ai.hand);
        ai.hand.Remove(chosen);

        // Instantiate AI card visual when played
        GameObject cardPrefab = DeckManager.dm.GetCardPrefab(chosen.element);
        if (cardPrefab == null)
        {
            Debug.LogError($"Card prefab not found for element {chosen.element}");
            yield break;
        }

        GameObject cardGO = Instantiate(
            cardPrefab,
            DeckManager.dm.hiddenZone.position,
            Quaternion.identity,
            TrickManager.tm.transform
        );

        CardUI cardUI = cardGO.GetComponent<CardUI>();
        cardUI.cd = chosen;
        cardUI.owner = ai;
        cardUI.SetCardVisuals();

        //Anim to play area
        cardUI.AnimToPosition(DeckManager.dm.deckTransform.position);

        TrickManager.tm.PlayCard(ai, chosen, cardGO);
    }

    private CardData ChooseCard(List<CardData> hand)
    {
        //Simple Ai logic: follows lead if possible, else lowest card
        ElementType lead = TrickManager.tm.leadElement;

        //First card played sets lead
        if (lead == ElementType.None)
        {
            return hand[Random.Range(0, hand.Count)]; //Randomly play any card if no lead
        }

        List<CardData> followSuit = hand.FindAll(card => card.element == lead);

        if (followSuit.Count > 0)
        {
            //Try to beat the current highest card on the table
            int highestSoFar = TrickManager.tm.GetHighestValueInTrick(lead);

            List<CardData> betterCards = followSuit.FindAll(card => card.value > highestSoFar);

            if (betterCards.Count > 0)
            {
                //Try to win the trick with the lowest possible winning card
                betterCards.Sort((a, b) => a.value.CompareTo(b.value));
                return betterCards[0];
            }

            //Can't beat current best, play the lowest following card
            followSuit.Sort((a, b) => a.value.CompareTo(b.value));
            return followSuit[Random.Range(0, followSuit.Count)];
        }

        //No matching suit, play lowest value card
        hand.Sort((a,b) => a.value.CompareTo(b.value));
        return hand[0];
    }
}
