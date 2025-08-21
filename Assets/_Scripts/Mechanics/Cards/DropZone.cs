using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class DropZone : MonoBehaviour
{
    //private Collider2D dropZoneCollider; no physics in cardgame dummy**
    RectTransform rt; //trying rect transform instead of collider

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public bool IsValidDrop(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, screenPosition, Camera.main);
    }

    public void OnCardPlayed(CardUI card)
    {
        Debug.Log($"Card {card.cd.cardName} played to zone");
    }
}
