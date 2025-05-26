using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public Transform handArea;
    public PlayerData playerData;
    public GameObject cardPrefab;

    public void PopulateHand(List<CardData> hand)
    {
        ClearHand();

        foreach (CardData card in hand)
        {
            GameObject cardGO = Instantiate(cardPrefab, handArea);
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
}
