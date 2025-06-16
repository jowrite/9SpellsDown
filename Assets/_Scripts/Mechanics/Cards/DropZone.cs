using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class DropZone : MonoBehaviour
{
    private Collider2D dropZoneCollider;

    private void Awake()
    {
        dropZoneCollider = GetComponent<Collider2D>();
    }

    public void OnCardPlayed(CardUI card)
    {
        
        Debug.Log($"Card {card.cd.cardName} played to zone");
        TrickManager.tm.PlayCard(card.owner, card.cd, card.gameObject);
        //anim into place on zone
    }
}
