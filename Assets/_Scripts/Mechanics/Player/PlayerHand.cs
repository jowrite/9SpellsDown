using NUnit.Framework;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public Transform handArea;
    public PlayerData playerData;

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
        float cardWidth = 120f; //Will need to adjust based on prefab size
        float maxSpread = Mathf.Min(cardWidth * playerData.hand.Count, 800f); //Max spread to prevent overflow
        float spacing = maxSpread / playerData.hand.Count;
        float startX = -maxSpread / 2f;

        for (int i = 0; i < handArea.childCount; i++)
        {
            RectTransform card = handArea.GetChild(i).GetComponent<RectTransform>();
            if (card == null) continue; // Skip if no RectTransform found

            Vector2 targetPos = new Vector2(startX + i * spacing, 0f);
            float delay = i * 0.05f;

            card.DOKill();
            card.DOAnchorPos(targetPos, 0.3f)
                .SetEase(Ease.OutQuad)
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
        return DeckManager.dm.GetCardPrefab(element);
    }
}
