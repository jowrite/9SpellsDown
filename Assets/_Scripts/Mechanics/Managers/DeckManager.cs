using NUnit.Framework;
using System.Collections.Generic;   
using UnityEngine;
using DG.Tweening;

public class DeckManager : MonoBehaviour
{
    public static DeckManager dm;
    public Transform playerHandArea;
    public Transform hiddenZone; //For AI player cards that are not visible to human players
    public Transform deckTransform; //For card animations

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

    private void Start()
    {
        hiddenZone.gameObject.SetActive(false); // Hide AI hand area by default
    }

    public void ShuffleAndDeal()
    {
        if (dm == null)
        {
            Debug.LogError("DeckManager instance is not initialized.");
            return;
        }

        // Clone and shuffle current deck
        currentDeck = new(fullDeck);
        Shuffle(currentDeck);

        // Clear previous hands
        foreach (PlayerData player in players)
        {
            player.hand.Clear();
        }
        wildMagicHand.Clear();

        // Deal 13 cards per player
        for (int i = 0; i < 13; i++)
        {
            foreach (PlayerData player in players)
            {
                CardData card = currentDeck[0];
                player.hand.Add(card);
                currentDeck.RemoveAt(0);

                DealCardToPlayer(player, card);
            }
        }

        // Remaining 13 cards go to wild magic hand
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
            deckTransform.DOShakePosition(0.5f, 20f, 10, 90).SetEase(Ease.InOutSine); //Test visuals to see if it mimicks a shuffle
        }
    }

    private void LoadDeck()
    {
        fullDeck = new(Resources.LoadAll<CardData>("Cards"));

        // Corrected the issue by separating the tweening logic from the Vector3 creation
        deckTransform.DOLocalMove(new Vector3(Screen.width / 2, Screen.height / 2, 1f), 1f)
            .SetEase(Ease.InOutSine);

        if (fullDeck.Count != 52)
            Debug.LogWarning($"Loaded {fullDeck.Count} cards instead of 52. Check for missing assets.");
        else
            Debug.Log("Deck loaded successfully.");
    }

    public GameObject GetCardPrefab(ElementType element)
    {
        Debug.Log($"Looking for prefab of: {element} | " +
             $"Fire: {fireCardPrefab != null} | " +
             $"Water: {waterCardPrefab != null} | " +
             $"Earth: {earthCardPrefab != null} | " +
             $"Air: {airCardPrefab != null}");
        
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
        if(player.isHuman)
        {
            Debug.Log($"Dealing card: {card.name} | Element: {card.element}");

            GameObject cardPrefab = GetCardPrefab(card.element);
            if (cardPrefab == null)
            {
                Debug.LogError($"Card prefab not found for element {card.element}");
                return;
            }

            GameObject cardGO = Instantiate(cardPrefab, deckTransform.position, Quaternion.identity, deckTransform.parent);
            //Debug: add visibility check
            cardGO.transform.localScale = Vector3.one; //Override any tween starting values
            Debug.Log($"Card spawned at: {cardGO.transform.position} | Active: {cardGO.activeInHierarchy}");

            CardUI cardUI = cardGO.GetComponent<CardUI>();
            cardUI.cd = card;
            cardUI.owner = player;
            cardUI.SetCardVisuals();
            cardGO.transform.SetParent(playerHandArea, worldPositionStays: true); // Set parent to player's hand area

            //Animate to hand
            Vector2 randomOffset = new Vector2(Random.Range(-200f, 200f), 0f);
            cardUI.AnimToPosition(randomOffset, delay: 0.05f * player.hand.Count);
        }
        //For AI: just add to hand data (no instantiation)
        else if (player.isAI)
        {
            player.hand.Add(card);
            Debug.Log($"Dealt card to AI player: {player.playerName} | Card: {card.cardName}");
        }
        else
        {
            Debug.LogError("Player type not recognized. Cannot deal card.");
        }
    }

}
