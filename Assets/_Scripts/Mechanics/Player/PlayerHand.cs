using NUnit.Framework;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public Transform handArea;
    public PlayerData playerData;

    [Header("Card Prefabs by Element")]
    public GameObject fireCardPrefab;
    public GameObject waterCardPrefab;
    public GameObject earthCardPrefab;
    public GameObject airCardPrefab;

    public void PopulateHand(List<CardData> hand)
    {
        ClearHand();

        foreach (CardData card in hand)
        {
            
            GameObject prefab = GetCardPrefab(card.element);
            if (prefab == null)
            {
                Debug.LogError($"No prefab found for element: {card.element}");
                continue;
            }
            GameObject cardGO = Instantiate(prefab, handArea);
            cardGO.transform.SetParent(handArea, worldPositionStays: false); // Set parent without world position change
            cardGO.transform.localScale = Vector3.one; // Reset scale
            CardUI ui = cardGO.GetComponent<CardUI>();
            ui.cd = card;
            ui.owner = playerData;
            ui.SetCardVisuals();
        }

    }

    public void FanOutCards()
    {
        float spacing = 200f; // Adjust spacing between cards
        float startX = -((playerData.hand.Count - 1) * spacing) / 2f; // Center the fan

        for (int i = 0; i < handArea.childCount; i++)
        {
           RectTransform card = handArea.GetChild(i).GetComponent<RectTransform>();
            if (card != null) continue;

            Vector2 targetPos = new Vector2(startX + 2 * spacing, 0f);
            float delay = i * 0.03f; //Delay for each card to create a fan effect

            card.DOKill(); //prevent overlapping tweens
            card.DOAnchorPos(targetPos, 2f)
                .SetEase(Ease.OutCubic)
                .SetDelay(delay);
        }
    }

    private void ClearHand()
    {
        foreach (Transform child in handArea)
        {
            Destroy(child.gameObject);
        }
    }

    public void SortHand()
    {
        playerData.hand.Sort((a, b) =>
        {
            int elementComparison = a.element.CompareTo(b.element);
            return elementComparison != 0 ? elementComparison : a.value.CompareTo(b.value);
        });

        PopulateHand(playerData.hand);
    }

    private GameObject GetCardPrefab(ElementType element)
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
                Debug.LogError("Unknown element type: " + element);
                return null;
        }
    }
}
