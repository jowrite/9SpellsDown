using UnityEngine;

public class DropZone : MonoBehaviour
{
    public void OnCardPlayed(CardUI card)
    {
        Debug.Log($"Card {card.cd.cardName} played to zone");
        TrickManager.tm.PlayCard(card.owner, card.cd);
        //anim into place on zone
    }
}
