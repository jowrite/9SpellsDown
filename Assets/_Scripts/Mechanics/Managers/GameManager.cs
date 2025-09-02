using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using static TrickManager;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentRound = 1;
    public int maxRounds = 13;
    private int totalCurseStart = 27;
    private int tricksPlayed = 0;

    private PlayerData trickLeader;
    public List<PlayerData> players; //Populate in Inspector
    public List<PlayerHUD> playerHUDs; //Populate in Inspector
    public List<PlayedCard> playedCards = new List<PlayedCard>();
    public List<PlayerData> playerOrder => TurnManager.turn.playerOrder;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //Initialize scores
        ScoreManager.instance.InitializeScores(players);

        StartRound();
        DOTween.Init(); //Ensures DOTween is initialized
        //SimulateRound();
    }

    public void StartRound()
    {
        Debug.Log($"Round {currentRound} starts!");
        tricksPlayed = 0;

        DeckManager.dm.ShuffleAndDeal();

        //Only fan out cards for human player
        PlayerHand humanHand = Object.FindFirstObjectByType<PlayerHand>();
        if (humanHand != null && humanHand.playerData.isHuman)
        {
            humanHand.FanOutCards();
        }

        //Ensure playerOrder is populated
        if (TurnManager.turn.playerOrder == null || TurnManager.turn.playerOrder.Count == 0)
            TurnManager.turn.playerOrder = new List<PlayerData>(players);

        //Start with whoever won the last trick
        PlayerData startingLeader = trickLeader != null ? trickLeader : players[0];
        TurnManager.turn.StartNewTrick(startingLeader);

        Debug.Log($"Trick leader: {TurnManager.turn.playerOrder[0].playerName}");

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
        ScoreManager.instance.ResolveRound(players);

        ////Update HUDs
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

    public void OnTrickResolved(PlayerData winner)
    {
        trickLeader = winner; //This should carry into next round
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

    //SIMULATE ROUNDS FOR TESTING
    #region
    //public void SimulateRound()
    //{
    //    Debug.Log("Simulating round with test cards...");

    //    DeckManager.dm.ShuffleAndDeal();

    //    foreach (PlayerData player in players)
    //    {
    //        player.spellCastsThisRound = 0;
    //    }

    //    for (int trick = 0; trick < 13; trick++)
    //    {
    //        SimulateTrick();
    //    }

    //    EndRound();
    //}

    //private void SimulateTrick()
    //{
    //    Debug.Log("Simulating trick");

    //    List<PlayedCard> trick = new List<PlayedCard>();
    //    ElementType leadElement = ElementType.None;

    //    foreach (PlayerData player in players)
    //    {

    //        if (player.hand.Count == 0) 
    //        {
    //            Debug.LogWarning($"{player.playerName} has no cards left to play!");
    //            continue;
    //        }

    //        CardData selectedCard = null;

    //        //First player sets the lead element
    //        if (leadElement == ElementType.None)
    //        {
    //            selectedCard = player.hand[Random.Range(0, player.hand.Count)];
    //            leadElement = selectedCard.element;
    //        }
    //        else
    //        {
    //            //Subsequent players must follow suit if possible
    //            List<CardData> followElement = player.hand.Where(c => c.element == leadElement).ToList();

    //            if (followElement.Count > 0)
    //            {
    //                int highestInTrick = trick
    //                    .Where(playedCards => playedCards.card.element == leadElement)
    //                    .Max(playedCards => playedCards.card.value);

    //                List<CardData> betterCards = followElement
    //                    .Where(c => c.value > highestInTrick)
    //                    .OrderBy(c => c.value) //prefer lowest winning card
    //                    .ToList();

    //                if(betterCards.Count > 0)
    //                {
    //                    selectedCard = betterCards[0];
    //                }
    //                else
    //                {
    //                    //Can't win, play lowest card in element
    //                    selectedCard = followElement.OrderBy(c => c.value).First();
    //                }
    //            }
    //            else
    //            {
    //                //No cards in matching suit, discard lowest card
    //                selectedCard = player.hand.OrderBy(c => c.value).First();
    //            }
    //        }

    //        player.hand.Remove(selectedCard);
    //        trick.Add(new PlayedCard(player, selectedCard));
    //        Debug.Log($"{player.playerName} plays {selectedCard.cardName}");
    //    }

    //    //Determine trick winner       
    //    CardData bestCard = trick[0].card;
    //    PlayerData winner = trick[0].player;

    //    for (int i = 1; i < trick.Count; i++)
    //    {
    //        CardData card = trick[i].card;
    //        if (card.element == leadElement && card.value > bestCard.value)
    //        {
    //            bestCard = card;
    //            winner = trick[i].player;
    //        }
    //    }

    //    winner.spellCastsThisRound++;
    //    Debug.Log($"{winner.playerName} wins the trick with {bestCard.cardName}");

    //}
    #endregion


}
