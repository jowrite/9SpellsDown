using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TrickManager : MonoBehaviour
{
    public static TrickManager tm;

    public ElementType leadElement;
    public List<PlayedCard> playedCards = new List<PlayedCard>();
    public PlayerData currentLeader;

    private void Awake()
    {
        if (tm == null) tm = this;
        else Destroy(gameObject);
    }

    public void PlayCard(PlayerData player, CardData card, GameObject cardGO)
    {
        if (card == null)
        {
            Debug.LogError($"Tried to play a NULL card from {player.playerName} using {cardGO.name}");
            return;
        }

        //Move card to play area
        cardGO.transform.SetParent(transform, worldPositionStays: true);

        playedCards.Add(new PlayedCard(player, card, cardGO));
        player.hand.Remove(card);       
        Debug.Log($"{player.playerName} played {card.cardName}");

        //If this is first card, establish lead element
        if (playedCards.Count == 1)
        {
            leadElement = card.element;
            currentLeader = player;
            Debug.Log($"[Element]{player.playerName} is leader with {card.cardName} ({leadElement})");
        }

        //Visual feedback for played card
        ArrangePlayedCards();

        //If full trick, resolve; otherwise advance turn
        if (playedCards.Count >= TurnManager.turn.playerOrder.Count)
        {
            Invoke(nameof(DetermineWinner), 2f); //Delay to allow anims to finish
        }
        else
        { 
            TurnManager.turn.EndPlayerTurn(); //End the current player's turn
        }
    }

    private void DetermineWinner()
    {
        //Attempting to fix the pause in game loop here
        if (playedCards.Count == 0)
        {
            Debug.LogError("No cards in trick to determine winner!");
            return;
        }

        // Fallback: if leader wasn’t set properly, just use first card played
        if (currentLeader == null)
        {
            currentLeader = playedCards[0].player;
            leadElement = playedCards[0].card.element;
            Debug.LogWarning($"Leader was null. Defaulting to {currentLeader.playerName} with {leadElement}");
        }

        PlayerData winner = currentLeader;
        CardData bestCard = playedCards.Find(pc => pc.player == winner).card;

        foreach (PlayedCard pc in playedCards)
        {
            if (pc.card.element == RoundManager.rm.foilElement && bestCard.element != RoundManager.rm.foilElement)
            {
                //Foil card beats non-foil
                bestCard = pc.card;
                winner = pc.player;
            }
            else if (pc.card.element == bestCard.element && pc.card.value > bestCard.value)
            {
                //Standard comparison
                bestCard = pc.card;
                winner = pc.player;
            }
        }

        Debug.Log($"{winner.playerName} wins the trick with {bestCard.cardName}");
        winner.spellCastsThisRound++;

        //Update scores UI
        ScoreManager.instance.OnScoresUpdated?.Invoke();

        foreach (PlayedCard pc in playedCards)
        {
            if (pc.cardObject != null)
                Destroy(pc.cardObject); // Clean up card objects
        }

        playedCards.Clear();
        GameManager.Instance.OnTrickResolved(winner);
        
    }

    public void ResetTrick()
    {
        playedCards.Clear();
        leadElement = ElementType.None;
    }

    public void ArrangePlayedCards()
    {
        float spacing = 2.5f;
        float startX = -(playedCards.Count - 1) * spacing * 0.5f; // Tweak as needed

        for (int i = 0; i < playedCards.Count; i++)
        {
            if (playedCards[i].cardObject == null) continue;

            Transform cardT = playedCards[i].cardObject.transform;

            //Only reposition human cards
            if (playedCards[i].player.isHuman)
            {
                Vector3 targetPos = new Vector3(startX + i * spacing, 0f, 0f);
                //Move smoothly so it looks nice
                cardT.DOLocalMove(targetPos, 0.3f).SetEase(Ease.OutCubic);
            }  
        }
    }

    public int GetHighestValueInTrick(ElementType element)
    {
        int highest = 0;
        foreach (var played in playedCards)
        {
            if (played.card.element == element && played.card.value > highest)
            {
                highest = played.card.value;
            }
        }
        return highest;
    }

    [System.Serializable]
    public class PlayedCard
    {
        public PlayerData player;
        public CardData card;
        public GameObject cardObject;

        public PlayedCard(PlayerData player, CardData card, GameObject cardObject)
        {
            this.player = player;
            this.card = card;
            this.cardObject = cardObject;
        }
    }
}