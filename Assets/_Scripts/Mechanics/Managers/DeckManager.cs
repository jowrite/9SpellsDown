using NUnit.Framework;
using System.Collections.Generic;   
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager dm;
    public Transform playerHandArea;

    [Header ("Card Prefabs By Element")]
    public GameObject fireCardPrefab;
    public GameObject waterCardPrefab;
    public GameObject earthCardPrefab;
    public GameObject airCardPrefab;

    [Header("Deck Source")]
    public List<CardData> fullDeck;
    private List<CardData> currentDeck = new List<CardData>();

    [Header("Dealt Hands")]
    public List<CardData> wildMagicHand = new List<CardData>();
    public List<PlayerData> players;

    private void Awake()
    {
        if (dm == null) dm = this;
        else Destroy(gameObject);

        LoadDeck();
    }

    public void ShuffleAndDeal()
    {
        //Clone and shuffle current deck
        currentDeck = new List<CardData>(fullDeck);
        Shuffle(currentDeck);

        //clear previous hands
        foreach (PlayerData player in players)
        {
            player.hand.Clear();
        }
        wildMagicHand.Clear();

        //Deal 13 cards per player
        for (int i = 0; i < 13; i++)
        {
            foreach (PlayerData player in players)
            {
                player.hand.Add(currentDeck[0]);
                currentDeck.RemoveAt(0);
            }
        }

        //Remaing 13 cards go to wild magic hand
        wildMagicHand.AddRange(currentDeck);
        Debug.Log("Shuffling complete. Wild Magic Hand is ready.");
    }

    private void Shuffle(List<CardData> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }

    private void LoadDeck()
    {
        fullDeck = new List<CardData>(Resources.LoadAll<CardData>("Cards"));

        if (fullDeck.Count != 52)
            Debug.LogWarning($"Loaded {fullDeck.Count} cards instead of 52. Check for missing assets.");
        else
            Debug.Log("Deck loaded successfully.");
    }

    public GameObject GetCardPrefab(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire:
                return fireCardPrefab;
            case ElementType.Water:
                return waterCardPrefab;
            case ElementType.Earth:
                return earthCardPrefab;
            case ElementType.Air:
                return airCardPrefab;
            default:
                Debug.LogError("Invalid element type.");
                return null;
        }
    }

    public void DealCardToPlayer(PlayerData player, CardData card)
    {
        GameObject cardPrefab = GetCardPrefab(card.element);
        if (cardPrefab != null) return;

        GameObject cardGO = Instantiate(cardPrefab, playerHandArea);
        CardUI cardUI = cardGO.GetComponent<CardUI>();

        cardUI.cd = card;
        cardUI.owner = player;

        cardUI.SetCardVisuals();

    }

}
