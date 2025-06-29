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
        if (playedCards.Count == 0)
        {
            leadElement = card.element;
            currentLeader = player;
        }

        //Move card to play area
        cardGO.transform.SetParent(transform);
        playedCards.Add(new PlayedCard(player, card, cardGO));
        player.hand.Remove(card);
        Debug.Log($"{player.playerName} played {card.cardName}");

        if (playedCards.Count >= 3)
        {
            DetermineWinner();
        }
        else
        { 
            TurnManager.turn.EndPlayerTurn(); //End the current player's turn
        }
    }

    private void DetermineWinner()
    {
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

        playedCards.Clear();
        GameManager.Instance.OnTrickResolved(winner);
        
    }

    public void ResetTrick()
    {
        playedCards.Clear();
        leadElement = ElementType.None;
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