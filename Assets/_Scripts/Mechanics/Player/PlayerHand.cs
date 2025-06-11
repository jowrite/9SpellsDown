using NUnit.Framework;
using System.Collections.Generic;
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
