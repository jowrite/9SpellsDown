using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class DropZone : MonoBehaviour
{
    //private Collider2D dropZoneCollider; no physics in cardgame dummy**
    RectTransform rt; //trying rect transform instead of collider
    public UnityEngine.UI.Image highlightImage; //Visual for debugging drop zone


    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (highlightImage) highlightImage.enabled = false; //Disable highlight by default
    }

    public bool IsValidDrop(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, screenPosition, Camera.main);
    }

    public void HighlightDropZone(bool highlight)
    {
        if (highlightImage) highlightImage.enabled = highlight;
    }

    public void OnCardPlayed(CardUI card)
    {
        Debug.Log($"Card {card.cd.cardName} played to zone");
    }
}
