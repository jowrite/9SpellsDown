using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class AIController : MonoBehaviour
{
    public List<PlayerData> aiPlayers;
    public static AIController Instance;

    [Header("AI Card Positions")]
    [SerializeField] private Vector2 ai1Position = new Vector2(-40f, 80f);
    [SerializeField] private Vector2 ai2Position = new Vector2(40f, 65f);

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

        //Create AI card visual but don't add to hand, add to play area
        GameObject cardPrefab = DeckManager.dm.GetCardPrefab(chosen.element);
        if (cardPrefab == null)
        {
            Debug.LogError($"Card prefab not found for element {chosen.element}");
            yield break;
        }

        //Instantiate as a child of the TrickManager area
        GameObject cardGO = Instantiate(cardPrefab, TrickManager.tm.transform);
        //Store original scale from prefab
        Vector3 originalScale = cardGO.transform.localScale;

        CardUI cardUI = cardGO.GetComponent<CardUI>();
        cardUI.cd = chosen;
        cardUI.owner = ai;
        cardUI.SetCardVisuals();

        //Calculate position in AI area
        Vector3 targetPosition = GetAICardPosition(ai);

        //Start off screen then animate into play area
        cardGO.transform.localScale = Vector3.one * 3f;
        cardGO.transform.localPosition = new Vector3(1000f, 0, 0f);

        cardGO.transform.DOLocalMove(targetPosition, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                TrickManager.tm.PlayCard(ai, chosen, cardGO);

            });

        //Remove from AI hand
        ai.hand.Remove(chosen);
    }

    private Vector3 GetAICardPosition(PlayerData ai)
    {
        if (TurnManager.turn == null || TurnManager.turn.playerOrder == null)
            return Vector3.zero;

        int index = TurnManager.turn.playerOrder.IndexOf(ai);

        switch (index)
        {
            case 0: return new Vector3(ai1Position.x, ai1Position.y, 0f);
            case 1: return new Vector3(ai2Position.x, ai2Position.y, 0f);
            default: return Vector3.zero;
        }
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
