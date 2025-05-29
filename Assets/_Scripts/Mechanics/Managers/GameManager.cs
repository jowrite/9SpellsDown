using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TrickManager;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentRound = 1;
    public int maxRounds = 13;
    private int totalCurseStart = 27;
    private int tricksPlayed = 0;

    public List<PlayerData> players; //Populate in Inspector
    public List<PlayerHUD> playerHUDs; //Populate in Inspector
    public List<PlayedCard> playedCards = new List<PlayedCard>();


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //StartRound();
        SimulateRound();
    }

    public void StartRound()
    {
        Debug.Log($"Round {currentRound} starts!");
        tricksPlayed = 0;

        foreach (PlayerData player in players)
        {
            player.ResetRoundStats();
        }

        DeckManager.dm.ShuffleAndDeal();
        TurnManager.turn.playerOrder = new List<PlayerData>(players);

        TurnManager.turn.StartNewTrick(players[0]); //Start with the first player in the list

        //Shuffle, deal, trick-taking logic will live here
    }

    public void EndRound()
    {
        Debug.Log($"Round {currentRound} has ended!");

        foreach (PlayerData player in players)
        {
            int spellcasts = player.spellCastsThisRound;

            int curseDelta = 4 - spellcasts;
            player.curseLevel += curseDelta;
            player.curseLevel = Mathf.Max(0, player.curseLevel);

            Debug.Log($"{player.playerName} won {spellcasts} spellcasts. Curse change: {curseDelta}. \nCurrent curse level:{player.curseLevel}");

        }

        //Update HUDs
        //foreach (PlayerHUD hud in playerHUDs)
        //{
        //    hud.UpdateHUD(players[playerHUDs.IndexOf(hud)]);
        //}

        CheckMagicLaw();
        currentRound++;

        if (currentRound > maxRounds || players.Any(p => p.curseLevel <= 0))
        {
            EndGame();
        }
        else
        {
            // Add delay or wait for player input before starting next round
            StartRound();
        }

    }

    private void CheckMagicLaw()
    {
        int expectedTotal = totalCurseStart - currentRound;
        int actualTotal = players.Sum(p => p.curseLevel);

        if (actualTotal != expectedTotal)
        {
            Debug.LogWarning($"Magic Law violation! Expected total curse level: {expectedTotal}, Actual total curse level: {actualTotal}");
            // Handle magic law violation (e.g., apply penalties, notify players, etc.)
        }
        else
        {
            Debug.Log("Magic Law is upheld!");
        }

    }

    private void EndGame()
    {
        Debug.Log("Game Over!");
        var winners = players.Where(p => p.curseLevel == 0).ToList();

        if(winners.Count > 0)
        {
            foreach(var w in winners)
            {
                Debug.Log($"{w.playerName} diespelled their curse!");
            }
        }
        else
        {
            Debug.Log("No one dispelled their curse on time!");
        }

    }

    //private void UpdateAllHUDs()
    //{
    //    for (int i = 0; i < players.Count; i++)
    //    {
    //        playerHUDs[i].UpdateHUD(players[i]);
    //        playerHUDs[i].SetLeaderHighlight(i == currentRound % players.Count);
    //    }
    //}

    //SIMULATE ROUNDS FOR TESTING

    public void SimulateRound()
    {
        Debug.Log("Simulating round with test cards...");

        DeckManager.dm.ShuffleAndDeal();

        foreach (PlayerData player in players)
        {
            player.spellCastsThisRound = 0;
        }

        for (int trick = 0; trick < 13; trick++)
        {
            SimulateTrick();
        }

        EndRound();
    }

    private void SimulateTrick()
    {
        Debug.Log("Simulating trick");

        List<PlayedCard> trick = new List<PlayedCard>();

        foreach (PlayerData player in players)
        {
            //Simulate picking a random card from hand
            if (player.hand.Count == 0) continue;

            CardData card = player.hand[Random.Range(0, player.hand.Count)];
            player.hand.Remove(card);

            trick.Add(new PlayedCard(player, card));
            Debug.Log($"{player.playerName} plays {card.cardName}");
        }

        //Determine trick winner
        ElementType leadElement = trick[0].card.element;
        CardData bestCard = trick[0].card;
        PlayerData winner = trick[0].player;

        for (int i = 1; i < trick.Count; i++)
        {
            CardData card = trick[i].card;
            if (card.element == leadElement && card.value > bestCard.value)
            {
                bestCard = card;
                winner = trick[i].player;
            }
        }

        winner.spellCastsThisRound++;
        Debug.Log($"{winner.playerName} wins the trick with {bestCard.cardName}");

    }

    public void OnTrickResolved(PlayerData winner)
    {
        tricksPlayed++;

        if (tricksPlayed >= 13)
        {
            EndRound();
        }
        else
        {
            TurnManager.turn.StartNewTrick(winner);
        }
    }
}
